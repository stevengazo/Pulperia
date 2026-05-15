
using Supabase;
using Supabase.Gotrue;

namespace Pulperia.Services;


public class AppSessionService
{
    private readonly Supabase.Client _supabase;



    public AppSessionService(Supabase.Client supabase)
    {
        _supabase = supabase;
    }


    public User? CurrentUser { get; private set; }

    public event Action? OnChange;

    public bool IsAuthenticated => CurrentUser != null;

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _supabase.Auth.SignIn(email, password);

        if (response?.User == null)
            return false;

        CurrentUser = response.User;

        Notify();
        return true;
    }

    public async Task LogoutAsync()
    {
        await _supabase.Auth.SignOut();

        CurrentUser = null;
        Notify();
    }

    public void LoadFromSupabase()
    {
        CurrentUser = _supabase.Auth.CurrentUser;
        Notify();
    }

    private void Notify() => OnChange?.Invoke();
}