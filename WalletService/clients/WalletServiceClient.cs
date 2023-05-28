using System.Text;
using Newtonsoft.Json;
using UserService.Clients;
using WalletService.models.request;
using WalletService.Utils;

namespace WalletService.Clients;

public class WalletServiceClient : BaseClient,IObservable<string>
{
    private readonly HttpClient _client = new HttpClient();
    private static  readonly Lazy<WalletServiceClient> _instance = new Lazy<WalletServiceClient>(() => new WalletServiceClient());
    
    public static WalletServiceClient Instance => _instance.Value;
    
    public async Task<HttpResponseMessage> GetBalance(int id)
    {
        var getBalanceRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(($"{WalletServiceEndpoints.get_balance}?userId={id}"))
        };

        HttpResponseMessage response = await _client.SendAsync(getBalanceRequest);
        return response;
    }
    
    public async Task<HttpResponseMessage> Charge(Charge request)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(WalletServiceEndpoints.charge),
            Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response = await _client.SendAsync(httpRequestMessage);
        if (response.IsSuccessStatusCode)
        {
            foreach (var observer in _observers)
            {
                if (observer.Value.GetType().Name == "TestDataObserver")
                {
                    Detach(observer.Value);
                    NotifyAllObservers(Convert.ToString(request.userId));
                    Subscribe(observer.Value);
                }
            }
        }
        return response;
    }
    
    public async Task<HttpResponseMessage> RevertTransaction(string id)
    {
        var revertTransactionRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Put,
            RequestUri = new Uri(($"{WalletServiceEndpoints.revert_transaction}?transactionId={id}"))
        };

        HttpResponseMessage response = await _client.SendAsync(revertTransactionRequest);
        return response;
    }
    
    public IDisposable Subscribe(IObserver<string> observer)
    {
        _observers.TryAdd(observer.GetType().Name, observer);
       
        return null;
    }
    
    public void Detach(IObserver<string> observer)
    {
        _observers.Remove(observer.GetType().Name, out observer);
    }
    
    public void NotifyAllObservers(string id)
    {
        foreach (var observer in _observers)
        {
            observer.Value.OnNext(id);
        }
    }
}