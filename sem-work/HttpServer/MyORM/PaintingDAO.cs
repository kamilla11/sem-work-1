using HttpServer.Interfaces;

namespace HttpServer.MyORM;

public class PaintingDAO: IDAO<Painting>
{
    private static string _connectionStr;
    private List<Painting> paintings;

    public PaintingDAO(string connectionString)
    {
        _connectionStr = connectionString;
        paintings = GetAll().ToList();
    }

    public Painting GetById(object id)
    {
        return new Database(_connectionStr).Select<Painting>((int)id);
    }
    
    public IEnumerable<Painting> GetByExpositionId(int id)
    {
        return paintings.Where(p => p.Exposition == id);
    }

    public IEnumerable<Painting> GetAll()
    {
        return new Database(_connectionStr).Select<Painting>();
    }
    
    
    public int Insert(Painting entity)
    {
        return new Database(_connectionStr).Insert(entity);
    }

    public int Delete(object id)
    {
        return new Database(_connectionStr).Delete<Painting>((int)id);
    }

    public int Delete(Painting entity)
    {
        return new Database(_connectionStr).Delete<Painting>(entity);
    }

    public int Update(Painting entity)
    {
        return new Database(_connectionStr).Update<Painting>(entity);
    }
}