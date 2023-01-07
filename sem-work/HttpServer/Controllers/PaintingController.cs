using HttpServer.Attributes;
using HttpServer.MyORM;

namespace HttpServer.Controllers;

[HttpController("paintings")]
public class PaintingController : Controller
{
    private static string _connectionStr = GlobalSettings.ConnectionString;

    private static PaintingDAO _paintingsDao = new(_connectionStr);

    // [HttpGET("paintings")]
    // public string paintings(string path)
    // {
    //     ExpositionDAO expositionDao = new(_connectionStr);
    //     
    //     var paintings = _paintingsDao.GetByExpositionId(id);
    //     var exposition = expositionDao.GetById(id);
    //     if (paintings is null) return "paintings not found";
    //     return CreateHtmlCode(path,
    //         new {Exhibition = exposition, Paintings = paintings });
    // }
    
    [HttpGET("paintings/getPainting")]
    public string getPainting(string path, int userId, int id)
    {
        path = "./site/single.html";
        ExpositionDAO expositionDao = new(_connectionStr);
        
        var paintings = _paintingsDao.GetByExpositionId(id);
        var exposition = expositionDao.GetById(id);
        if (paintings is null) return "paintings not found";
        return CreateHtmlCode(path,
            new {Exposition = exposition, Paintings = paintings });
    }
}