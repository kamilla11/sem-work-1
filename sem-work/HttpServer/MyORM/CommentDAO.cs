using HttpServer.Interfaces;

namespace HttpServer.MyORM;

public class CommentDAO : IDAO<Comment>
{
    private static string _connectionStr;

    private List<Comment> Comments => GetAll().ToList();

    public CommentDAO(string connectionString)
    {
        _connectionStr = connectionString;
    }

    public Comment GetById(object id)
    {
        return new Database(_connectionStr).Select<Comment>((int)id);
    }

    public IEnumerable<Comment> GetAll()
    {
        return new Database(_connectionStr).Select<Comment>();
    }
    
    public IEnumerable<Comment> GetAllByEventId(int id)
    {
        return Comments.Where(c => c.EventId == id);
    }


    public int Insert(Comment entity)
    {
        return new Database(_connectionStr).Insert(entity);
    }

    public int Delete(object id)
    {
        return new Database(_connectionStr).Delete<Comment>((int)id);
    }

    public int Delete(Comment entity)
    {
        return new Database(_connectionStr).Delete(entity);
    }

    public int Update(Comment entity)
    {
        return new Database(_connectionStr).Update(entity);
    }
}