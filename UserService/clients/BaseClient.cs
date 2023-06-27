using System.Collections.Concurrent;

namespace UserService.Clients;

public class BaseClient
{
    protected static ConcurrentDictionary<string, IObserver<string>> observers = new ConcurrentDictionary<string, IObserver<string>>();
}