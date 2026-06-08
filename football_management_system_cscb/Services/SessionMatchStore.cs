using System.Text.Json;
using football_management_system_cscb.Models;

public class SessionMatchStore
{
    private readonly IHttpContextAccessor _http;

    public SessionMatchStore(IHttpContextAccessor http)
    {
        _http = http;
    }

    private ISession Session => _http.HttpContext!.Session;

    public void Save(MatchState state)
    {
        Session.SetString("match", JsonSerializer.Serialize(state));
    }

    public MatchState? Load()
    {
        var data = Session.GetString("match");
        return data == null ? null : JsonSerializer.Deserialize<MatchState>(data);
    }
}