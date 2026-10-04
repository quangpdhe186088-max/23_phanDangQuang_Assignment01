using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using _23_phanDangQuang_Assignment01_FrontEnd.Models;

namespace _23_phanDangQuang_Assignment01_FrontEnd.Services;

public sealed record ApiResult<T>(HttpStatusCode Status, T? Value, string? Error = null)
{
    public bool IsSuccess => (int)Status is >= 200 and < 300 && (Value is not null || Status == HttpStatusCode.NoContent);
}

public sealed class BackendApiClient(IHttpClientFactory clients, LoginSession session, ILogger<BackendApiClient> logger)
{
    public Task<ApiResult<LoginResponse>> LoginAsync(LoginViewModel model, CancellationToken cancellationToken) =>
        SendAsync<LoginResponse>(HttpMethod.Post, "api/auth/login", false,
            new { model.Email, model.Password }, cancellationToken);

    public Task<ApiResult<T>> GetAsync<T>(string path, CancellationToken cancellationToken) =>
        SendAsync<T>(HttpMethod.Get, path, true, null, cancellationToken);

    public Task<ApiResult<T>> GetPublicAsync<T>(string path, CancellationToken cancellationToken) =>
        SendAsync<T>(HttpMethod.Get, path, false, null, cancellationToken);

    public Task<ApiResult<T>> PostAsync<T>(string path, object body, CancellationToken ct) =>
        SendAsync<T>(HttpMethod.Post, path, true, body, ct);
    public Task<ApiResult<T>> PutAsync<T>(string path, object body, CancellationToken ct) =>
        SendAsync<T>(HttpMethod.Put, path, true, body, ct);
    public Task<ApiResult<object>> DeleteAsync(string path, CancellationToken ct) =>
        SendAsync<object>(HttpMethod.Delete, path, true, null, ct);

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string path, bool authenticated,
        object? body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        if (authenticated)
        {
            var token = session.AccessToken;
            if (token is null) return new(HttpStatusCode.Unauthorized, default);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        if (body is not null) request.Content = JsonContent.Create(body);
        try
        {
            using var client = clients.CreateClient("Backend");
            using var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new(response.StatusCode, default, await ReadErrorAsync(response, cancellationToken));
            if (response.StatusCode == HttpStatusCode.NoContent) return new(response.StatusCode, default);
            return new(response.StatusCode, await response.Content.ReadFromJsonAsync<T>(cancellationToken));
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "Cannot reach the Backend API.");
            return new(HttpStatusCode.ServiceUnavailable, default);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new(HttpStatusCode.ServiceUnavailable, default);
        }
        catch (JsonException exception)
        {
            logger.LogWarning(exception, "Backend API returned an invalid response.");
            return new(HttpStatusCode.BadGateway, default);
        }
    }
    private static async Task<string?> ReadErrorAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var error = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            if (error.TryGetProperty("message", out var message)) return message.GetString();
            if (error.TryGetProperty("errors", out var errors))
                return string.Join(" ", errors.EnumerateObject().SelectMany(field =>
                    field.Value.EnumerateArray().Select(value => value.GetString())));
        }
        catch (JsonException) { }
        catch (NotSupportedException) { }
        return null;
    }
}
