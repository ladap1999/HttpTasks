using System.Net;
using System.Text;
using Newtonsoft.Json;
using UserService.Utils;

namespace UserService.Clients;

public class UserServiceClient
{
    private readonly HttpClient _client = new HttpClient();

    public async Task<HttpResponseMessage> RegisterUser(object request)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(UserServiceEndpoints.register_user),
            Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);

        return response;
    }

    public async Task<HttpStatusCode> DeleteUser(int id)
    {
        var deleteUserRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(($"{UserServiceEndpoints.delete_user}?userId={id}"))
        };

        HttpResponseMessage response = await _client.SendAsync(deleteUserRequest);
        return response.StatusCode;
    }

    public async Task<HttpResponseMessage> GetUserStatus(int id)
    {
        var getUserStatusRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{UserServiceEndpoints.get_user_status}?userId={id}")
        };

        HttpResponseMessage response = await _client.SendAsync(getUserStatusRequest);
        return response;
    }

    public async Task<HttpStatusCode> SetUserStatus(int id, bool isActive)
    {
        var setUserStatusRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{UserServiceEndpoints.set_user_status}?userId={id}&newStatus={isActive}")
        };

        HttpResponseMessage response = await _client.SendAsync(setUserStatusRequest);
        return response.StatusCode;
    }
}