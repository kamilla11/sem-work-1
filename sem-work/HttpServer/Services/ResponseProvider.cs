using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using HttpServer.Attributes;
using HttpServer.Controllers;
using HttpServer.MyORM;

namespace HttpServer;

public class ResponseProvider
{
    private readonly HttpListenerContext _listenerContext;
    private readonly string _connectionStr = GlobalSettings.ConnectionString;
    private readonly SessionManager _sessionManager;
    private readonly ServerSettings _serverSettings;

    public ResponseProvider(ServerSettings serverSettings, HttpListenerContext listenerContext,
        SessionManager sessionManager)
    {
        _listenerContext = listenerContext;
        _sessionManager = sessionManager;
        _serverSettings = serverSettings;
    }

    public bool FilesHandler(out byte[]? buffer)
    {
        buffer = null;
        if (Directory.Exists(_serverSettings.Path))
        {
            buffer = LoadFile(_serverSettings.Path);
        }

        if (buffer is null) return false;
        return true;
    }


    private byte[]? LoadFile(string path)
    {
        var rawUrl = _listenerContext.Request.RawUrl.Replace("%20", " ");
        byte[]? localBuffer = null;
        var filePath = path + rawUrl;

        if (Directory.Exists(filePath))
        {
            filePath = filePath + "/index.html";
            if (File.Exists(filePath))
            {
                _listenerContext.Response.Headers.Set("Content-Type", "text/html");
                localBuffer = File.ReadAllBytes(filePath);
            }
        }
        else if (File.Exists(filePath))
        {
            var contentType = Mime.GetMimeType(filePath);
            _listenerContext.Response.Headers.Set("Content-Type", contentType);
            localBuffer = File.ReadAllBytes(filePath);
        }

        return localBuffer;
    }

    public byte[] NotFound()
    {
        _listenerContext.Response.Headers.Set("Content-Type", "text/plain");
        _listenerContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
        string err = "404 - not found";
        return Encoding.UTF8.GetBytes(err);
    }


    public bool MethodHandler(out byte[]? buffer)
    {
        buffer = null;
        HttpListenerRequest request = _listenerContext.Request;
        HttpListenerResponse response = _listenerContext.Response;

        if (request.Url!.Segments.Length < 2)
        {
            FilesHandler(out buffer);
            return true;
        }

        string controllerName = request.Url.Segments[1].Replace("/", "");

        if (!TryGetController(out var controller, controllerName)) return false;

        var isMethodController = request.Url.Segments.Length < 3;
        var method = TryGetLastMethodInController(isMethodController, controller, out var methodName,
            out var queryParams, controllerName, request);
        if (method is null) return false;

        var cookie = request.Cookies["SessionId"];
        var convertedParams = GenerateQueryParams(cookie, queryParams, request, method);

        var ret = method.Invoke(Activator.CreateInstance(controller), convertedParams);

        response.ContentType = "text/html";
        if (ret is not null) buffer = Encoding.UTF8.GetBytes(ret.ToString()!);

        ProcessMethodsAfter(ref buffer, methodName, ret, response, cookie);

        return true;
    }

    private void ProcessMethodsAfter(ref byte[] buffer, string methodName, object? ret, HttpListenerResponse response,
        Cookie cookie)
    {
        switch (methodName)
        {
            case "quite":
            {
                var cook = new Cookie("SessionId", Guid.Parse(cookie!.Value).ToString());
                cook.Expires = DateTime.Now.AddDays(-1d);
                cook.Path = "/";
                response.Cookies.Add(cook);
                _sessionManager.DeleteSession(Guid.Parse(cookie.Value));
                break;
            }
            case "saveAccount":
            {
                var res = ((bool, Account, string))ret!;
                var acc = res.Item2;
                if (res.Item1)
                {
                    var guid = _sessionManager.CreateSession(acc.Id, DateTime.Now);
                    var cook = new Cookie("SessionId", guid.ToString());
                    cook.Expires = DateTime.Now.AddDays(1);
                    cook.Path = "/";
                    response.Cookies.Add(cook);
                    buffer = Encoding.UTF8.GetBytes(res.Item3);
                }
                else
                {
                    buffer = Encoding.UTF8.GetBytes(res.Item3);
                }

                break;
            }

            case "login":
            {
                var res = ((bool, int?, bool))ret!;
                if (res.Item1)
                {
                    AccountDAO accountDao = new(_connectionStr);
                    var userId = res.Item2!.Value;
                    var account = accountDao.GetById(userId);
                    var guid = (res.Item3)
                        ? _sessionManager.CreateSession(userId,
                            DateTime.Now, true)
                        : _sessionManager.CreateSession(userId,
                            DateTime.Now);
                    var cook = new Cookie("SessionId", guid.ToString());
                    cook.Expires = (res.Item3) ? DateTime.Now.AddYears(1) : DateTime.Now.AddDays(1);
                    cook.Path = "/";
                    response.Cookies.Add(cook);
                    var path = "./site/profil.html";
                    buffer = Encoding.UTF8.GetBytes(Controller.CreateHtmlCode(path,
                        new { Account = account, Show = true }));
                }
                else
                {
                    var path = "./site/login.html";
                    buffer = Encoding.UTF8.GetBytes(Controller.CreateHtmlCode(path,
                        new
                        {
                            IsUserExist = false, IsEmailCorrect = true, IsPasswordCorrect = true, IsNameCorrect = true,
                            IsSurnameCorrect = true
                        }));
                }

                break;
            }
        }
    }

    private object[] GenerateQueryParams(Cookie? cookie, object[] queryParams, HttpListenerRequest request,
        MethodInfo method)
    {
        if (cookie is not null && _sessionManager.IsSessionExist(Guid.Parse(cookie.Value)))
        {
            var session = _sessionManager.GetSessionInfo(Guid.Parse(cookie.Value));
            queryParams = queryParams.Concat(new[] { session!.AccountId.ToString() }).ToArray();
        }
        else
        {
            queryParams = queryParams.Concat(new[] { "-1" }).ToArray();
        }

        if (request.Url.Segments.Length > 3 && _listenerContext.Request.HttpMethod == HttpMethod.Get.ToString() &&
            queryParams.Length == 2)
        {
            var strParams = request.Url
                .Segments
                .Skip(3)
                .Select(s => s.Replace("/", ""))
                .ToArray();

            queryParams = queryParams.Concat(strParams).Select(x => x.ToString()).ToArray();
        }

        queryParams = method.GetParameters()
            .Select((p, i) => Convert.ChangeType(queryParams[i], p.ParameterType))
            .ToArray();
        return queryParams;
    }

    private MethodInfo? TryGetLastMethodInController(bool isMethodController, Type controller, out string methodName,
        out object[]? queryParams,
        string controllerName, HttpListenerRequest request)
    {
        methodName = "";
        queryParams = Array.Empty<Object>();
        MethodInfo? method = null;
        if (isMethodController)
        {
            methodName = controllerName;
            method = TryGetMethod(controller, methodName, out queryParams, controllerName);
        }
        else
        {
            var index = request.Url.Segments.Length - 1;
            var isSuccess = false;
            while (index != 1 && !isSuccess)
            {
                var currentName = request.Url.Segments[index].Replace("/", "");
                index--;
                method = TryGetMethod(controller, currentName, out queryParams, controllerName);
                if (method is null) continue;
                isSuccess = true;
                methodName = currentName;
            }
        }

        return method;
    }

    private bool TryGetController(out Type controller, string controllerName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        controller = assembly.GetTypes()
            .FirstOrDefault(c => Attribute.IsDefined(c, typeof(HttpController)) &&
                                 (((HttpController)c.GetCustomAttribute(typeof(HttpController))).ControllerName ==
                                  controllerName.ToLower() ||
                                  c.Name.ToLower() == controllerName.ToLower()));
        if (controller is null)
            return false;
        return true;
    }

    private MethodInfo? TryGetMethod(Type controller, string methodName, out object[]? queryParams,
        string controllerName)
    {
        MethodInfo? method;
        var methodType = $"Http{_listenerContext.Request.HttpMethod}";
        queryParams = new object[] { };
        switch (methodType)
        {
            case "HttpGET":
                method = controller.GetMethods()
                    .FirstOrDefault(c => Attribute.IsDefined(c, typeof(HttpGET)) &&
                                         (((HttpGET)c.GetCustomAttribute(typeof(HttpGET))).MethodURI ==
                                          methodName.ToLower() ||
                                          c.Name.ToLower() == methodName.ToLower()));
                string path;
                if (methodName == "main") path = _serverSettings.Path + "/index.html";
                else path = _serverSettings.Path + "/" + controllerName.ToLower() + ".html";
                queryParams = new[] { path };
                break;
            case "HttpPOST":
                method = controller.GetMethods()
                    .FirstOrDefault(c => Attribute.IsDefined(c, typeof(HttpPOST)) &&
                                         (((HttpPOST)c.GetCustomAttribute(typeof(HttpPOST))).MethodURI ==
                                          methodName.ToLower() ||
                                          c.Name.ToLower() == methodName.ToLower()));
                queryParams = GetParamsPostData(_listenerContext.Request);
                break;
            default:
                method = null;
                break;
        }

        return method;
    }

    private static string[]? GetParamsPostData(HttpListenerRequest request)
    {
        if (!request.HasEntityBody)
        {
            return null;
        }

        using Stream body = request.InputStream;
        using var reader = new StreamReader(body, Encoding.UTF8);
        var query = reader.ReadToEnd();
        var queryParams = query.Split('&')
            .Select(pair => pair.Split('='))
            .Select(pair => HttpUtility.UrlDecode(pair[1]))
            .ToArray();
        return queryParams;
    }
}