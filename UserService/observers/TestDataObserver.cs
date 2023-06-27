

namespace UserService.Observers;

public class TestDataObserver : BaseObserver, IObserver<string>
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
        _listOfIds.TryAdd(id, id);
    }

    public IDictionary<string, string> GetAllIds()
    {
        return _listOfIds;
    }
}