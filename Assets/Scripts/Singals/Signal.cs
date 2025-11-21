using System;

public abstract class Signal
{
    private static event Action _listeners;

    public static void AddListener(Action listener)
    {
        _listeners += listener;
    }

    public static void RemoveListener(Action listener)
    {
        _listeners -= listener;
    }

    public static void Invoke()
    {
        _listeners?.Invoke();
    }
} 

public abstract class Signal<T>
{
    private static event Action<T> _listeners;

    public static void AddListener(Action<T> listener)
    {
        _listeners += listener;
    }

    public static void RemoveListener(Action<T> listener)
    {
        _listeners -= listener;
    }

    public static void Invoke(T value)
    {
        _listeners?.Invoke(value);
    }
}
