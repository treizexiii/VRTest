using Shared.Models;

namespace GameServicesAPI.Services;

public interface IAlertService
{
    Task SendAlertAsync(string method, Exception e);
}