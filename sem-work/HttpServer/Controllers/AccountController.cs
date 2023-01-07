using System.Text;
using System.Text.RegularExpressions;
using HttpServer.Attributes;
using HttpServer.MyORM;

namespace HttpServer.Controllers;

[HttpController("accounts")]
internal class AccountsController : Controller
{
    private static string _connectionStr = "Server=localhost;Database=museum;Port=5432;SSLMode=Prefer";

    private static AccountDAO _accountDao = new AccountDAO(_connectionStr);

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
    public void quite(string path, int userId)
    {
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


        var res = _accountDao.Insert(new Account()
            { Email = email, Password = password, Name = name, Surname = surname, Gender = gender });

        var path = "./site/profil.html";
        var account = _accountDao.GetAccount(email, password);
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

    private string accountsWithError(bool isUserExist, bool isEmailCorrect, bool isPasswordCorrect, bool isNameCorrect,
        bool isSurnameCorrect)
    {
        return CreateHtmlCode("./site/login.html",
            new
            {
                IsUserExist = isUserExist, IsEmailCorrect = isEmailCorrect, IsPasswordCorrect = isPasswordCorrect,
                IsNameCorrect = isNameCorrect, IsSurnameCorrect = isSurnameCorrect
            });
    }

    private string? CheckRegistrationData(string email, string password, string name, string surname)
    {
        var regex1 = new Regex(@"([a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)");
        var isEmailCorrect = regex1.IsMatch(email);

        var regex2 = new Regex(@"^[a-zA-Z][a-zA-Z0-9-_\.]{1,20}$");
        var isNameCorrect = regex2.IsMatch(name);
        var isSurnameCorrect = regex2.IsMatch(surname);

        var regex3 = new Regex(@"^[a-zA-Z0-9]{8,20}$");
        var isPasswordCorrect = regex3.IsMatch(password);

        if (!isEmailCorrect || !isNameCorrect || !isSurnameCorrect || !isPasswordCorrect)
            return accountsWithError(true, isEmailCorrect, isPasswordCorrect, isNameCorrect, isSurnameCorrect);

        return null;
    }
}