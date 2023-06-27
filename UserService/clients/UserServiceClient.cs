using System.Text;
using Newtonsoft.Json;
using UserService.Extensions;
using UserService.Utils;

namespace UserService.Clients;

public class UserServiceClient : BaseClient, IObservable<string>
{
    private readonly HttpClient _client = new HttpClient();
    private static  readonly Lazy<UserServiceClient> _instance = new Lazy<UserServiceClient>(() => new UserServiceClient());
    
    public static UserServiceClient Instance => _instance.Value;
    public async Task<HttpResponseMessage> RegisterUser(object request)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(UserServiceEndpoints.register_user),
            Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
        if (response.IsSuccessStatusCode)
        {
            foreach (var observer in observers)
            {
                if (observer.Value.GetType().Name == "DeleteTestObserver")
                {
                    Detach(observer.Value);
                    NotifyAllObservers(response.GetStringValue());
                    Subscribe(observer.Value);
                }
                
                if (observer.Value.GetType().Name == "TransactionTestObserver")
                {
                    Detach(observer.Value);
                    NotifyAllObservers(response.GetStringValue());
                    Subscribe(observer.Value);
                }
            }
        }

        return response;
    }

    public async Task<HttpResponseMessage> DeleteUser(int id)
    {
        var deleteUserRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(($"{UserServiceEndpoints.delete_user}?userId={id}"))
        };

        HttpResponseMessage response = await _client.SendAsync(deleteUserRequest);
        if (response.IsSuccessStatusCode)
        {
            foreach (var observer in observers)
            {
                if (observer.Value.GetType().Name == "TestDataObserver")
                {
                    Detach(observer.Value);
                    NotifyAllObservers(Convert.ToString(id));
                    Subscribe(observer.Value);
                }
                
                if (observer.Value.GetType().Name == "TransactionTestObserver")
                {
                    Detach(observer.Value);
                    NotifyAllObservers(response.GetStringValue());
                    Subscribe(observer.Value);
                }
            }
        }
        return response;
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

    public async Task<HttpResponseMessage> SetUserStatus(int id, bool isActive)
    {
        var setUserStatusRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri($"{UserServiceEndpoints.set_user_status}?userId={id}&newStatus={isActive}")
        };

        HttpResponseMessage response = await _client.SendAsync(setUserStatusRequest);
        return response;
    }

    public IDisposable Subscribe(IObserver<string> observer)
    {
       observers.TryAdd(observer.GetType().Name, observer);
       
       return null;
    }
    
    public void Detach(IObserver<string> observer)
    {
       observers.Remove(observer.GetType().Name, out observer);
    }
    
    public void NotifyAllObservers(string id)
    {
        foreach (var observer in observers)
        {
            observer.Value.OnNext(id);
        }
    }
}