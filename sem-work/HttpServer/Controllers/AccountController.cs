using System.Text;
using System.Text.RegularExpressions;
using HttpServer.Attributes;
using HttpServer.MyORM;
using HttpServer.Services;

namespace HttpServer.Controllers;

[HttpController("accounts")]
internal class AccountsController : Controller
{
    private static string _connectionStr = GlobalSettings.ConnectionString;

    private static AccountDAO _accountDao = new AccountDAO(_connectionStr);

    private static DataService _dataService = new DataService();

    [HttpGET("accounts")]
    public string accounts(string path, int userId)
    {
        if (userId != -1) return getAccountById(path, userId);
        path = "./site/login.html";
        return CreateHtmlCode(path,
            new
            {
                IsUserExist = true, IsEmailCorrect = true, IsPasswordCorrect = true, IsNameCorrect = true,
                IsSurnameCorrect = true
            });
    }

    [HttpGET("accounts/getAccounts")]
    public string getAccounts()
    {
        var accounts = _accountDao.GetAll();
        if (accounts is null) return "Accounts not found";
        var accountList = new StringBuilder();
        foreach (var user in accounts)
        {
            accountList.Append($"User with id = {user.Id}, login =  {user.Email}, password = {user.Password}   ");
        }

        return accountList.ToString();
    }


    [HttpGET("accounts/getAccountById")]
    public string getAccountById(string path, int userId)
    {
        var account = _accountDao.GetById(userId);
        if (account is null) return "User not found";
        path = "./site/profil.html";
        return CreateHtmlCode(path, new { Account = account, Show = true });
    }

    [HttpGET("accounts/quite")]
    public string quite(string path, int userId)
    {
        TicketDAO ticketDao = new(_connectionStr);
        var tickets = ticketDao.GetAll();
        if (tickets is null) return "Tickets not found";
        return CreateHtmlCode("./site/index.html",
            new { Tickets = tickets });
    }

    [HttpPOST("accounts/saveAccount")]
    public (bool, Account, string) saveAccount(string email, string password, string name, string surname,
        string gender)
    {
        var result = CheckRegistrationData(email, password, name, surname);
        if (result is not null) return (false, null, result);

        switch (gender)
        {
            case "female":
                gender = "Женщина";
                break;
            case "male":
                gender = "Мужчина";
                break;
            default:
                gender = "Небинарная личность";
                break;
        }

        var hashPassword = Hash.ComputePasswordHash(password);

        var res = _accountDao.Insert(new Account()
            { Email = email, Password = hashPassword, Name = name, Surname = surname, Gender = gender });

        var path = "./site/profil.html";
        var account = _accountDao.GetAccount(email, hashPassword);
        var stringResult = CreateHtmlCode(path, new { Account = account, Show = true });
        return (true, account, stringResult);
    }


    [HttpPOST("accounts/updateAccount")]
    public string updateAccount(string id, string email, string password, string name, string surname, string gender)
    {
        var intId = int.Parse(id);
        switch (gender)
        {
            case "female":
                gender = "Женщина";
                break;
            case "male":
                gender = "Мужчина";
                break;
            default:
                gender = "Небинарная личность";
                break;
        }

        var account = new Account()
            { Id = intId, Email = email, Password = password, Name = name, Surname = surname, Gender = gender };
        var res = _accountDao.Update(account);
        if (res == 0)
        {
            Console.WriteLine("Error while saving data");
            return "Error while saving data";
        }

        var path = "./site/profil.html";
        return CreateHtmlCode(path, new { Account = account, Show = true });
    }

    [HttpGET("accounts/editAccount")]
    public string editAccount(string path, int userId)
    {
        path = "./site/profil.html";
        var account = _accountDao.GetById(userId);
        return CreateHtmlCode(path, new { Account = account, Show = false });
    }

    [HttpPOST("accounts/login")]
    public (bool, int?, bool) login(string login, string password, string remember = null)
    {
        bool rememb = remember == "on";
        var items = _accountDao.VerifyLoginAndPassword(login, password);

        return (items.Item1, items.Item2, rememb);
    }

    private string? CheckRegistrationData(string email, string password, string name, string surname)
    {
        var result = _dataService.CheckData(email, password, name, surname);
        return (result.IsAllCorrect) ? null : CreateHtmlCode("./site/login.html", result);
    }
}