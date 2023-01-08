using HttpServer.Interfaces;

namespace HttpServer.MyORM;

public class TicketDAO : IDAO<Ticket>
{
    private static string _connectionStr;
    private List<Ticket> tickets;

    public TicketDAO(string connectionString)
    {
        _connectionStr = connectionString;
        tickets = GetAll().ToList();
    }

    public Ticket GetById(object id)
    {
        return new Database(_connectionStr).Select<Ticket>((int)id);
    }

    public IEnumerable<Ticket> GetAll()
    {
        return new Database(_connectionStr).Select<Ticket>();
    }


    public int Insert(Ticket entity)
    {
        return new Database(_connectionStr).Insert(entity);
    }

    public int Delete(object id)
    {
        return new Database(_connectionStr).Delete<Ticket>((int)id);
    }

    public int Delete(Ticket entity)
    {
        return new Database(_connectionStr).Delete<Ticket>(entity);
    }

    public int Update(Ticket entity)
    {
        return new Database(_connectionStr).Update<Ticket>(entity);
    }
}