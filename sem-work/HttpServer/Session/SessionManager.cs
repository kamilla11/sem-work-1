using Microsoft.Extensions.Caching.Memory;

namespace HttpServer;

public class SessionManager
{
    private MemoryCache _cache;
    private SessionManager()
    {
        _cache = new(new MemoryCacheOptions());
    }

    private static readonly Lazy<SessionManager> Lazy =
        new(() => new SessionManager());

    public static SessionManager Instance => Lazy.Value;

    

    public Guid CreateSession(int accountId, string login, DateTime createDateTime)
    {
        var guid = Guid.NewGuid();
        var session = new Session(guid, accountId, login, createDateTime);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            // Храним в кэше в течении этого времени, сбрасываем время при обращении.
            .SetSlidingExpiration(TimeSpan.FromDays(1));

        // Сохраняем данные в кэше.
        _cache.Set(guid, session, cacheEntryOptions);
        return guid;
    }


    public bool IsSessionExist(object key)
    {
        return _cache.TryGetValue(key, out var session);
    }

    public Session? GetSessionInfo(object key)
    {
        _cache.TryGetValue(key, out Session? session);
        return session;
    }
    
    public void DeleteSession(object key)
    {
        _cache.Remove(key);
    }
}