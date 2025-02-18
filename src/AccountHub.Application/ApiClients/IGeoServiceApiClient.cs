using Refit;

namespace AccountHub.Application.ApiClients
{
    public interface IGeoServiceApiClient
    {
        [Get("/manager/api/geo/get-all")]
        Task<List<string>> GetAvailableCountries();
    }
}
