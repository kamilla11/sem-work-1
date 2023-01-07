using HttpServer.Attributes;
using HttpServer.MyORM;

namespace HttpServer.Controllers;

[HttpController("main")]
public class MainController: Controller
{
    private static string _connectionStr = GlobalSettings.ConnectionString;

    private static TicketDAO _ticketDao = new(_connectionStr);

    [HttpGET("main")]
    public string main(string path, int userId)
    {
        var tickets = _ticketDao.GetAll();
        if (tickets is null) return "Events not found";
        return CreateHtmlCode(path,
            new { Tickets = tickets });
    }

}