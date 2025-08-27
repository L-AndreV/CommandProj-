using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

public class TokenManager
{
    private readonly ConcurrentDictionary<string, TokenInfo> _ClientTokens = new();
    private readonly ConcurrentDictionary<string, TokenInfo> _EmployeeTokens = new();
    private readonly Timer _cleanupTimer;

    public TokenManager()
    {
        _cleanupTimer = new Timer(CleanupExpired, null,
            TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));
    }

    public string CreateClientToken(int userId, TimeSpan lifetime)
    {
        var token = Guid.NewGuid().ToString("N");
        _ClientTokens[token] = new TokenInfo
        {
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.Add(lifetime)
        };
        return token;
    }
    public string CreateEmployeeToken(int userId, TimeSpan lifetime)
    {
        var token = Guid.NewGuid().ToString("N");
        _EmployeeTokens[token] = new TokenInfo
        {
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.Add(lifetime)
        };
        return token;
    }
    public int? GetClientId(string token)
    {
        if (_ClientTokens.TryGetValue(token, out var info))
        {
            if (info.ExpiresAt > DateTime.UtcNow)
            {
                info.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
                return info.UserId;
            }
            else
            {
                _ClientTokens.TryRemove(token, out _);
            }
        }
        return null;
    }
    public int? GetEmployeeId(string token)
    {
        if (_EmployeeTokens.TryGetValue(token, out var info))
        {
            if (info.ExpiresAt > DateTime.UtcNow)
            {
                info.ExpiresAt = DateTime.UtcNow.AddMinutes(10);
                return info.UserId;
            }
            else
            {
                _EmployeeTokens.TryRemove(token, out _);
            }
        }
        return null;
    }
    private void CleanupExpired(object state)
    {
        var now = DateTime.UtcNow;
        var expired = _ClientTokens.Where(kvp => kvp.Value.ExpiresAt < now)
                            .Select(kvp => kvp.Key)
                            .ToList();
        foreach (var token in expired)
            _ClientTokens.TryRemove(token, out _);

        expired = _EmployeeTokens.Where(kvp => kvp.Value.ExpiresAt < now)
                            .Select(kvp => kvp.Key)
                            .ToList();
        foreach (var token in expired)
            _EmployeeTokens.TryRemove(token, out _);
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}

public class TokenInfo
{
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
}