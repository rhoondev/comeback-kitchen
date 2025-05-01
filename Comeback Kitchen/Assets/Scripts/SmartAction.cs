using System;
using System.Collections.Generic;

public class SmartAction
{
    private readonly HashSet<Action> _handlers = new();
    private Action _invoker;

    // For plain Action (no parameters)
    public void Add(Action handler)
    {
        if (_handlers.Add(handler))
        {
            _invoker += handler;
        }
    }

    public void Remove(Action handler)
    {
        if (_handlers.Remove(handler))
        {
            _invoker -= handler;
        }
    }

    public void Invoke()
    {
        _invoker?.Invoke();
    }

    public void Clear()
    {
        _handlers.Clear();
        _invoker = null;
    }

    public bool Contains(Action handler)
    {
        return _handlers.Contains(handler);
    }

    public int Count => _handlers.Count;
}

public class SmartAction<T>
{
    private readonly HashSet<Action<T>> _handlers = new();
    private Action<T> _invoker;

    // For Action<T> (with a parameter)
    public void Add(Action<T> handler)
    {
        if (_handlers.Add(handler))
        {
            _invoker += handler;
        }
    }

    public void Remove(Action<T> handler)
    {
        if (_handlers.Remove(handler))
        {
            _invoker -= handler;
        }
    }

    public void Invoke(T arg)
    {
        _invoker?.Invoke(arg);
    }

    public void Clear()
    {
        _handlers.Clear();
        _invoker = null;
    }

    public bool Contains(Action<T> handler)
    {
        return _handlers.Contains(handler);
    }

    internal void Add(object v)
    {
        throw new NotImplementedException();
    }

    public int Count => _handlers.Count;
}

public class SmartAction<T1, T2> : SmartAction
{
    private readonly HashSet<Action<T1, T2>> _handlers = new();
    private Action<T1, T2> _invoker;

    public void Add(Action<T1, T2> handler)
    {
        if (_handlers.Add(handler))
        {
            _invoker += handler;
        }
    }

    public void Remove(Action<T1, T2> handler)
    {
        if (_handlers.Remove(handler))
        {
            _invoker -= handler;
        }
    }

    public void Invoke(T1 arg1, T2 arg2)
    {
        _invoker?.Invoke(arg1, arg2);
    }

    public bool Contains(Action<T1, T2> handler)
    {
        return _handlers.Contains(handler);
    }
}