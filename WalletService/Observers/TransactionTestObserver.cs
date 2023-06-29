using UserService.Observers;

namespace WalletService.observers;

public class TransactionTestObserver : BaseObserver, IObserver<string>
{
    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(string id)
    {
        ListOfIds.Remove(id, out id);
    }
    
    public IDictionary<string, string> GetAllIds()
    {
        return ListOfIds;
    }
}