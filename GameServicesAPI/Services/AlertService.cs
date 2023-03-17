namespace GameServicesAPI.Services;

public class AlertService : IAlertService
{
    private readonly HttpClient _httpClient;

    public AlertService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://alertapi.treize.cloud/");
    }

    public Task SendAlertAsync(string method)
    {
        return _httpClient.PostAsync("alert/" + method, null);
    }
}