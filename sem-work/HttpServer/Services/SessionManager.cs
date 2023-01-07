using HttpServer.MyORM;
using Microsoft.Extensions.Caching.Memory;

namespace HttpServer;

public class SessionManager
{
    private static string _connectionStr = GlobalSettings.ConnectionString;
    private MemoryCache _cache;
    private SessionDAO _sessionDao;

    private SessionManager()
    {
        _sessionDao = new SessionDAO(_connectionStr);
        _cache = new(new MemoryCacheOptions());
    }

    private static readonly Lazy<SessionManager> Lazy =
        new(() => new SessionManager());

    public static SessionManager Instance => Lazy.Value;



    public Guid CreateSession(int accountId, DateTime createDateTime, bool remember = false)
    {
        var guid = Guid.NewGuid();
        var session = new Session(guid, accountId, createDateTime);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Храним в кэше в течении этого времени, сбрасываем время при обращении.
            .SetSlidingExpiration(TimeSpan.FromHours(1));

        // Сохраняем данные в кэше.
        _cache.Set(guid, session, cacheEntryOptions);
        if (remember) _sessionDao.Insert(session);
        return guid;
    }


    public bool IsSessionExist(object key)
    {
        _cache.TryGetValue(key, out var cacheSession);
        var dbSession = _sessionDao.GetById(key);
        UpdateSessionDate(dbSession);
        
        return cacheSession is not null || dbSession is not null && (DateTime.Now - dbSession.CreateDateTime).TotalDays < 365;
    }

    public Session? GetSessionInfo(object key)
    {
        _cache.TryGetValue(key, out Session? session);
        if (session is null)
        {
            session = _sessionDao.GetById(key);
            UpdateSessionDate(session);
        }
        return session;
    }
    
    public void DeleteSession(object key)
    {
        _cache.Remove(key);
        if (_sessionDao.GetById(key) is not null) _sessionDao.Delete(key);
    }

    private void UpdateSessionDate(Session? session)
    {
        if (session is not null)
        {
            session.CreateDateTime = DateTime.Now;
            _sessionDao.Update(session);
        }
    }
}