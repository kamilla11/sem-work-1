namespace HttpServer;

public class Painting: EntityBase
{
    public string? Name { get; set; }
    public string? Artist { get; set; }
    public int Exposition { get; set; }
    public string? Year { get; set; }
    public string? ImgPath { get; set; }
}