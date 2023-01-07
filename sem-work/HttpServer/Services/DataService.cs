using System.Text.RegularExpressions;

namespace HttpServer.Services;

public class DataService
{
    public DataResult CheckData(string email, string password, string name, string surname)
    {
        var regex1 = new Regex(@"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)");
        var isEmailCorrect = regex1.IsMatch(email);

        var regex2 = new Regex(@"^[a-zA-Z][a-zA-Z0-9-_\.]{1,20}$");
        var isNameCorrect = regex2.IsMatch(name);
        var isSurnameCorrect = regex2.IsMatch(surname);

        var regex3 = new Regex(@"^[a-zA-Z0-9]{8,20}$");
        var isPasswordCorrect = regex3.IsMatch(password);

        return new DataResult(true,isEmailCorrect, isPasswordCorrect, isNameCorrect, isSurnameCorrect);
    }
}