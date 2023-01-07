using HttpServer.Interfaces;
using HttpServer.Services;

namespace HttpServer.MyORM;

public class AccountDAO: IDAO<Account>
{
    private static string _connectionStr;
    private List<Account> Accounts => GetAll().ToList();

    public AccountDAO(string connectionString)
    {
        _connectionStr = connectionString;
    }

    public Account GetById(object id)
    {
        return new Database(_connectionStr).Select<Account>((int)id);
    }

    public Account GetAccount(string login, string password)
    {
        var account = Accounts.Find(a => a.Email == login && a.Password == password);
        return account;
    }
    
    
    public IEnumerable<Account> GetAll()
    {
        return new Database(_connectionStr).Select<Account>();
    }
    
    
    public int Insert(Account entity)
    {
        return new Database(_connectionStr).Insert(entity);
    }

    public int Delete(object id)
    {
        return new Database(_connectionStr).Delete<Account>((int)id);
    }

    public int Delete(Account entity)
    {
        return new Database(_connectionStr).Delete<Account>(entity);
    }

    public int Update(Account entity)
    {
        return new Database(_connectionStr).Update(entity);
    }

    public (bool, int?) VerifyLoginAndPassword(string login, string password)
    {
        var account = Accounts.Find(a => a.Email == login);
        if (account is null)  return (false, null);
        var isValid = Hash.IsPasswordValid(password, account.Password!);
        if(isValid) return (true, account.Id);
        return (false, null);
    }
}