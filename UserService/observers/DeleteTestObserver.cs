namespace UserService.observers;

public class DeleteTestObserver : BaseObserver, IObserver<string>
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
        _listOfIds.Remove(id, out id);
    }
    
    public IDictionary<string, string> GetAllIds()
    {
        return _listOfIds;
    }
}