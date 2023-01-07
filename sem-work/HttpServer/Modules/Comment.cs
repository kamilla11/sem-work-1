namespace HttpServer;

public class Comment: EntityBase
{
    public string? Email { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string? Text { get; set; }
}