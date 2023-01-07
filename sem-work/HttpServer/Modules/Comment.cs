namespace HttpServer;

public class Comment
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public string? Text { get; set; }
}