using HttpServer.Interfaces;

namespace HttpServer.MyORM;

public class ExpositionDAO: IDAO<Exposition>
{
    private static string _connectionStr;
    private List<Exposition> expositions;

    public ExpositionDAO(string connectionString)
    {
        _connectionStr = connectionString;
        expositions = GetAll().ToList();
    }

    public Exposition GetById(object id)
    {
        return new Database(_connectionStr).Select<Exposition>((int)id);
    }

    public IEnumerable<Exposition> GetAll()
    {
        return new Database(_connectionStr).Select<Exposition>();
    }
    
    
    public int Insert(Exposition entity)
    {
        return new Database(_connectionStr).Insert(entity);
    }

    public int Delete(Exposition entity)
    {
        return new Database(_connectionStr).Delete<Exposition>(entity);
    }

    public int Delete(object id)
    {
        return new Database(_connectionStr).Delete<Exposition>((int)id);
    }

    public int Update(Exposition entity)
    {
        return new Database(_connectionStr).Update<Exposition>(entity);
    }
}