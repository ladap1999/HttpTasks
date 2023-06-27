using NUnit.Framework;
using UserService.Clients;
using UserService.Observers;
using WalletService.Clients;
using WalletService.observers;

namespace WalletService.Tests;
[SetUpFixture]
public class BaseTest
{
    private TestDataObserver _observer;
    private TransactionTestObserver _observerForTransaction;
    
    [OneTimeSetUp]
    public void SetUp()
    {
        _observerForTransaction = new TransactionTestObserver();
        _observer = new TestDataObserver();
        WalletServiceClient.Instance.Subscribe(_observerForTransaction);
        UserServiceClient.Instance.Subscribe(_observer);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        UserServiceClient client = UserServiceClient.Instance;

        var tasks = _observerForTransaction.GetAllIds()
            .Select(id => client.DeleteUser(Convert.ToInt32(id.Value)));
        
        await Task.WhenAll(tasks);
    }
}