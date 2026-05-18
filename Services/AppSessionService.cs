using Microsoft.JSInterop;
using Supabase;
using Supabase.Gotrue;
using System.Text.Json;

namespace Pulperia.Services;

public class AppSessionService
{
    private readonly Supabase.Client _supabase;
    private readonly IServiceProvider _serviceProvider;

    private IJSRuntime JS => _serviceProvider.GetRequiredService<IJSRuntime>();

    private const string SessionKey = "pulperia_session";
    private static readonly TimeSpan SessionDuration = TimeSpan.FromDays(7);

    public AppSessionService(Supabase.Client supabase, IServiceProvider serviceProvider)
    {
        _supabase = supabase;
        _serviceProvider = serviceProvider;
    }

    public User? CurrentUser { get; private set; }

    public bool IsAuthenticated => CurrentUser != null;

    // 🔑 IMPORTANTE: estado de carga inicial
    public bool IsInitialized { get; private set; }

    public event Action? OnChange;

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _supabase.Auth.SignIn(email, password);

        if (response?.User == null)
            return false;

        CurrentUser = response.User;

        var saved = new SavedSession
        {
            AccessToken = response.AccessToken,
            RefreshToken = response.RefreshToken,
            ExpiresAt = DateTime.UtcNow.Add(SessionDuration)
        };

        await JS.InvokeVoidAsync("appStorage.save", SessionKey,
            JsonSerializer.Serialize(saved));

        Notify();
        return true;
    }

    public async Task LogoutAsync()
    {
        await _supabase.Auth.SignOut();
        await JS.InvokeVoidAsync("appStorage.remove", SessionKey);

        CurrentUser = null;
        Notify();
    }

    /// <summary>
    /// Debe llamarse una sola vez al iniciar la app (App.razor o MainLayout)
    /// </summary>
    public async Task TryRestoreSessionAsync()
    {
        try
        {
            var raw = await JS.InvokeAsync<string?>("appStorage.load", SessionKey);

            if (!string.IsNullOrEmpty(raw))
            {
                var saved = JsonSerializer.Deserialize<SavedSession>(raw);

                if (saved != null && DateTime.UtcNow <= saved.ExpiresAt)
                {
                    var response = await _supabase.Auth.SetSession(
                        saved.AccessToken,
                        saved.RefreshToken
                    );

                    if (response?.User != null)
                    {
                        CurrentUser = response.User;

                        // renovar expiración
                        saved.ExpiresAt = DateTime.UtcNow.Add(SessionDuration);

                        await JS.InvokeVoidAsync("appStorage.save", SessionKey,
                            JsonSerializer.Serialize(saved));
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("appStorage.remove", SessionKey);
                    }
                }
                else
                {
                    await JS.InvokeVoidAsync("appStorage.remove", SessionKey);
                }
            }
        }
        catch
        {
            // sesión inválida o storage no disponible
        }
        finally
        {
            // 🔑 clave del fix: SIEMPRE marcar inicializado
            IsInitialized = true;
            Notify();
        }
    }

    public void LoadFromSupabase()
    {
        CurrentUser = _supabase.Auth.CurrentUser;
        IsInitialized = true;
        Notify();
    }

    private void Notify() => OnChange?.Invoke();

    private class SavedSession
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
}