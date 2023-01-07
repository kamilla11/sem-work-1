using HttpServer.Interfaces;

namespace HttpServer.MyORM;

public class SessionDAO:IDAO<Session>
{
    private static string _connectionStr;
    private List<Session> sessions;

    public SessionDAO(string connectionString)
    {
        _connectionStr = connectionString;
        sessions = GetAll().ToList();
    }

    public Session? GetById(object id)
    {
        return new Database(_connectionStr).Select<Session>((Guid)id);
    }

    public IEnumerable<Session> GetAll()
    {
        return new Database(_connectionStr).Select<Session>();
    }
    
    
    public int Insert(Session entity)
    {
        return new Database(_connectionStr).Insert(entity);
    }
    
    
    public int Delete(object id)
    {
        return new Database(_connectionStr).Delete<Session>((Guid)id);
    }

    public int Delete(Session entity)
    {
        return new Database(_connectionStr).Delete(entity);
    }

    public int Update(Session entity)
    {
        return new Database(_connectionStr).Update(entity);
    }
    
}