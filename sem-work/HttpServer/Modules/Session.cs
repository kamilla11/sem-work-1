using Npgsql.Replication;

namespace HttpServer;

public class Session
{
    public Guid Id { get; set; }
    public int AccountId { get; set; }
    public DateTime CreateDateTime { get; set; }

    public Session()
    {}
    
    public Session(Guid id, int accountId, DateTime createDateTime)
    {
        Id = id;
        AccountId = accountId;
        CreateDateTime = createDateTime;
    }
}