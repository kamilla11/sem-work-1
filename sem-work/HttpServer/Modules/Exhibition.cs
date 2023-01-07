namespace HttpServer;

public class Exhibition: EntityBase
{
    public string? Name { get; set; }
    public string? Place { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public string? ImgPath { get; set; }
}