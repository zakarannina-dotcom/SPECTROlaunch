using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows;

namespace CS2Launcher.Services;

/// <summary>
/// Сервис авторизации через Steam OpenID.
/// Реализует полноценный OpenID 2.0 flow с запуском локального HttpListener.
/// </summary>
public class SteamAuthService
{
    private const string SteamOpenIdUrl = "https://steamcommunity.com/openid/login";
    private const string ReturnUrl = "http://localhost:8080/auth/steam/callback";
    private const string Realm = "http://localhost:8080/";
    private const int CallbackPort = 8080;

    private HttpListener? _httpListener;
    private TaskCompletionSource<string>? _loginTcs;

    /// <summary>
    /// Событие успешной авторизации (передаёт SteamID64).
    /// </summary>
    public event Action<string>? LoginSuccess;

    /// <summary>
    /// Событие ошибки авторизации.
    /// </summary>
    public event Action<string>? LoginError;

    /// <summary>
    /// Событие отмены авторизации.
    /// </summary>
    public event Action? LoginCancelled;

    /// <summary>
    /// Запускает процесс авторизации: открывает Steam-страницу в браузере,
    /// параллельно поднимая локальный HttpListener для приёма callback'а.
    /// </summary>
    public async Task StartLoginAsync()
    {
        _loginTcs = new TaskCompletionSource<string>();

        try
        {
            StartHttpListener();
            var authUrl = BuildSteamAuthUrl();
            Process.Start(new ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });

            // Ждём callback с таймаутом 5 минут
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            cts.Token.Register(() =>
            {
                if (!_loginTcs.Task.IsCompleted)
                {
                    _loginTcs.TrySetCanceled();
                    LoginCancelled?.Invoke();
                }
            });

            await _loginTcs.Task;
        }
        catch (OperationCanceledException)
        {
            LoginCancelled?.Invoke();
        }
        catch (Exception ex)
        {
            LoginError?.Invoke($"Login failed: {ex.Message}");
        }
        finally
        {
            StopHttpListener();
        }
    }

    private void StartHttpListener()
    {
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add($"http://localhost:{CallbackPort}/");
        _httpListener.Start();

        _ = Task.Run(async () =>
        {
            try
            {
                while (_httpListener.IsListening)
                {
                    var context = await _httpListener.GetContextAsync();
                    HandleCallback(context);
                }
            }
            catch (HttpListenerException)
            {
                // listener остановлен
            }
        });
    }

    private void StopHttpListener()
    {
        try
        {
            _httpListener?.Stop();
            _httpListener?.Close();
        }
        catch { /* ignore */ }
    }

    private void HandleCallback(HttpListenerContext context)
    {
        var query = HttpUtility.ParseQueryString(context.Request.Url!.Query);

        try
        {
            var steamId = ValidateAndExtractSteamId(query);

            // HTML-ответ пользователю
            var html = steamId != null
                ? $@"<html><body style='background:#1a1a1a;color:#ff3b3b;font-family:Segoe UI,Arial;text-align:center;padding:60px'>
                    <h1>✓ Авторизация успешна</h1>
                    <p>SteamID: <code>{steamId}</code></p>
                    <p style='color:#888'>Можете закрыть это окно и вернуться в лаунчер.</p>
                  </body></html>"
                : $@"<html><body style='background:#1a1a1a;color:#ff3b3b;font-family:Segoe UI,Arial;text-align:center;padding:60px'>
                    <h1>✗ Ошибка авторизации</h1>
                    <p>Steam не подтвердил личность. Попробуйте снова.</p>
                  </body></html>";

            var buffer = System.Text.Encoding.UTF8.GetBytes(html);
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();

            if (steamId != null)
            {
                _loginTcs?.TrySetResult(steamId);
                LoginSuccess?.Invoke(steamId);
            }
            else
            {
                _loginTcs?.TrySetCanceled();
                LoginError?.Invoke("Steam OpenID validation failed.");
            }
        }
        catch (Exception ex)
        {
            _loginTcs?.TrySetException(ex);
            LoginError?.Invoke(ex.Message);
        }
    }

    /// <summary>
    /// Steam OpenID возвращает openid.identity = https://steamcommunity.com/openid/id/{steamid}.
    /// Серверная валидация через assertions требует обратного POST в Steam,
    /// здесь мы доверяем callback'у (для прод-использования добавьте server-side проверку).
    /// </summary>
    private string? ValidateAndExtractSteamId(NameValueCollection query)
    {
        var mode = query.Get("openid.mode");
        if (mode != "id_res")
            return null;

        var identity = query.Get("openid.identity") ?? string.Empty;
        var match = Regex.Match(identity, @"openid/id/(\d+)");
        return match.Success ? match.Groups[1].Value : null;
    }

    private string BuildSteamAuthUrl()
    {
        var parameters = new Dictionary<string, string>
        {
            { "openid.ns", "http://specs.openid.net/auth/2.0" },
            { "openid.mode", "checkid_setup" },
            { "openid.return_to", ReturnUrl },
            { "openid.realm", Realm },
            { "openid.identity", "http://specs.openid.net/auth/2.0/identifier_select" },
            { "openid.claimed_id", "http://specs.openid.net/auth/2.0/identifier_select" }
        };

        var query = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        return $"{SteamOpenIdUrl}?{query}";
    }

    public void Logout()
    {
        StopHttpListener();
    }
}
