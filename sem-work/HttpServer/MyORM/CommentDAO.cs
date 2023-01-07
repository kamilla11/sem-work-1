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

    public Comment GetById(int id)
    {
        return new Database(_connectionStr).Select<Comment>(id);
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

    public int Delete(int id)
    {
        return new Database(_connectionStr).Delete<Comment>(id);
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