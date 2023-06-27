﻿using System.Collections.Concurrent;

namespace UserService.Observers;

public class BaseObserver
{
    protected static readonly ConcurrentDictionary<string, string> _listOfIds= new ConcurrentDictionary<string, string>();
}