using System.Text;
using Newtonsoft.Json;
using UserService.Clients;
using WalletService.Models.Request;
using WalletService.Utils;

namespace WalletService.Clients;

public class WalletServiceClient : BaseClient, IObservable<string>
{
    private readonly HttpClient _client = new HttpClient();

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
            foreach (var observer in Observers)
            {
                if (observer.Value.GetType().Name == "TestDataObserver")
                {
                    Detach(observer.Value);
                    NotifyAllObservers(Convert.ToString(request.UserId));
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
        Observers.TryAdd(observer.GetType().Name, observer);
       
        return null;
    }
    
    public void Detach(IObserver<string> observer)
    {
        Observers.Remove(observer.GetType().Name, out observer);
    }
    
    public void NotifyAllObservers(string id)
    {
        foreach (var observer in Observers)
        {
            observer.Value.OnNext(id);
        }
    }
}