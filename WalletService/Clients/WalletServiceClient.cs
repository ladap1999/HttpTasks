using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WalletService.models.request;
using WalletService.Utils;

namespace WalletService.Clients;

public class WalletServiceClient
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
}