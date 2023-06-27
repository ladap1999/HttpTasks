using NUnit.Framework;
using UserService.Clients;
using UserService.Observers;

namespace UserService.Tests;
[SetUpFixture]
public class BaseTest
{
    private TestDataObserver _observer;
    private DeleteTestObserver _observerForDelete;
 
    [OneTimeSetUp]
    public void SetUp()
    {
        _observer = new TestDataObserver();
        _observerForDelete= new DeleteTestObserver();
        UserServiceClient.Instance.Subscribe(_observer);
        UserServiceClient.Instance.Subscribe(_observerForDelete);
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        UserServiceClient client = UserServiceClient.Instance;

        var tasks = _observer.GetAllIds()
            .Select(id => client.DeleteUser(Convert.ToInt32(id.Value)));
        
        await Task.WhenAll(tasks);
    }
    
}