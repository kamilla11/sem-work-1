namespace HttpServer;

public class Event: EntityBase
{
    public string? Name { get; set; }
    public string? Speaker { get; set; }
    public string? Date { get; set; }
    public string? Place { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
}