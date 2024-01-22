using System;
using System.Collections;
using System.Collections.Generic;

namespace DSS.Common;

public class Watched<T> where T: IEquatable<T>
{
    private T _value;
    public T Value
    {
        get => _value;
        set
        {
            var oldValue = _value;
            _value = value;
            if (!oldValue.Equals(value))
            {
                OnChanged?.Invoke(oldValue, value);
            }
        }
    }
    public Action<T, T> OnChanged;
    public Watched(T value)
    {
        _value = value;
    }
}

public class WatchedArray<T>: IEnumerable<T> where T: IEquatable<T>
{
    private T[] _value;

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var element in _value)
        {
            yield return element;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public T this[int index]
    {
        get => _value[index];
        set
        {
            var oldValue = _value[index];
            _value[index] = value;
            if (!oldValue.Equals(value))
            {
                OnElementChanged?.Invoke(index, oldValue, value);
            }
        }
    }
    public Action<int, T, T> OnElementChanged;
    public WatchedArray(T[] value)
    {
        _value = value;
    }
    
    public WatchedArray(int size)
    {
        _value = new T[size];
    }

    public void Fill(T value)
    {
        for (int i = 0; i < _value.Length; i++)
        {
            _value[i] = value;
        }
    }
    
    public int Length => _value.Length;
}