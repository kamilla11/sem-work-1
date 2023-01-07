using HttpServer.Interfaces;

namespace HttpServer.MyORM;

public class ExhibitionDAO: IDAO<Exhibition>
{
    private static string _connectionStr;
    private List<Exhibition> exhibitions;

    public ExhibitionDAO(string connectionString)
    {
        _connectionStr = connectionString;
        exhibitions = GetAll().ToList();
    }

    public Exhibition GetById(object id)
    {
        return new Database(_connectionStr).Select<Exhibition>((int)id);
    }

    public IEnumerable<Exhibition> GetAll()
    {
        return new Database(_connectionStr).Select<Exhibition>();
    }
    
    
    public int Insert(Exhibition entity)
    {
        return new Database(_connectionStr).Insert(entity);
    }

    public int Delete(object id)
    {
        return new Database(_connectionStr).Delete<Exhibition>((int)id);
    }

    public int Delete(Exhibition entity)
    {
        return new Database(_connectionStr).Delete<Exhibition>(entity);
    }

    public int Update(Exhibition entity)
    {
        return new Database(_connectionStr).Update<Exhibition>(entity);
    }
}