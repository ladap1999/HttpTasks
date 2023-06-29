using System.Collections.Concurrent;

namespace UserService.Observers;

public class BaseObserver
{
    protected static readonly ConcurrentDictionary<string, string> ListOfIds = new ConcurrentDictionary<string, string>();
}