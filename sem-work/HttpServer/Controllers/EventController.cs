using System.Text;
using HttpServer.Attributes;
using HttpServer.MyORM;

namespace HttpServer.Controllers;

[HttpController("events")]
public class EventController : Controller
{
    private static string _connectionStr = GlobalSettings.ConnectionString;

    private static EventDAO _eventDao = new(_connectionStr);
    private static AccountDAO _accountDao = new(_connectionStr);
    private static CommentDAO _commentDao = new(_connectionStr);

    [HttpGET("events")]
    public string events(string path, int userId)
    {
        var events = _eventDao.GetAll();
        if (events is null) return "Events not found";
        return CreateHtmlCode(path,
            new { Events = events, UserId = userId });
    }


    [HttpGET("events/openEvent")]
    public string openEvent(string path, int userId, int eventId)
    {
        var singleEvent = _eventDao.GetById(eventId);
        var comments = _commentDao.GetAllByEventId(eventId);
        if (singleEvent is null) return "Event not found";
        path = "./site/single-event.html";
        return CreateHtmlCode(path, new { Event = singleEvent, Comments = comments, Id = userId });
    }

    // [HttpGET("events/deleteComment")]
    // public string deleteComment(string path, int userId, int eventId, int commentId)
    // {
    //     _commentDao.Delete(commentId);
    //     return openEvent("./site/single-event.html", userId, eventId);
    // }

    

    // [HttpPOST("events/saveComment")]
    // public string saveComment(int userId, int eventId, string text)
    // {
    //     var account = _accountDao.GetById(userId);
    //
    //     var res = _commentDao.Insert(new() { UserId = userId, Email = account.Email, EventId = eventId, Text = text });
    //     if (res == 0)
    //     {
    //         return "Error while saving data";
    //     }
    //
    //     return null;
    //     // return openEvent("./site/single-event.html", userId, eventId);
    // }
    //
    // [HttpPOST("events/saveCommentUpdates")]
    // public string saveCommentUpdates(int userId, int eventId, int commentId, string text)
    // {
    //     var comment = _commentDao.GetById(commentId);
    //     comment.Text = text;
    //     var res = _commentDao.Update(comment);
    //     if (res == 0)
    //     {
    //         return "Error while saving data";
    //     }
    //
    //     return null;
    //     //  return openEvent("./site/single-event.html", userId, eventId);
    // }

    [HttpPOST("events/deleteComment")]
    public string deleteComment(int userId, int eventId, int commentId)
    {
        _commentDao.Delete(commentId);
        var singleEvent = _eventDao.GetById(eventId);
        var comments = _commentDao.GetAllByEventId(eventId);

        var path = "./site/comments.html";
        return CreateHtmlCode(path, new { Event = singleEvent, Comments = comments, Id = userId });
    }

    [HttpPOST("events/saveComment")]
    public string saveComment(int userId, int eventId, string text)
    {
        var account = _accountDao.GetById(userId);

        var res = _commentDao.Insert(new() { UserId = userId, Email = account.Email, EventId = eventId, Text = text });
        if (res == 0)
        {
            return "Error while saving data";
        }
        var singleEvent = _eventDao.GetById(eventId);
        var comments = _commentDao.GetAllByEventId(eventId);

        var path = "./site/comments.html";
        return CreateHtmlCode(path, new { Event = singleEvent, Comments = comments, Id = userId });
    }

    [HttpPOST("events/saveCommentUpdates")]
    public string saveCommentUpdates(int userId, int eventId, int commentId, string text)
    {
        var comment = _commentDao.GetById(commentId);
        comment.Text = text;
        var res = _commentDao.Update(comment);
        if (res == 0)
        {
            return "Error while saving data";
        }
        var singleEvent = _eventDao.GetById(eventId);
        var comments = _commentDao.GetAllByEventId(eventId);

        var path = "./site/comments.html";
        return CreateHtmlCode(path, new { Event = singleEvent, Comments = comments, Id = userId });
    }
}