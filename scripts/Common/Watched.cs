using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    private T[] _content;
    
    public T[] Content => _content;

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var element in _content)
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
        get => _content[index];
        set
        {
            var oldValue = _content[index];
            _content[index] = value;
            if (!oldValue.Equals(value))
            {
                OnElementChanged?.Invoke(index, oldValue, value);
            }
        }
    }
    public Action<int, T, T> OnElementChanged;
    public WatchedArray(T[] value)
    {
        _content = value;
    }
    
    public WatchedArray(int size)
    {
        _content = new T[size];
    }

    public void Fill(T value)
    {
        System.Array.Fill(_content, value);
    }
    
    public int Length => _content.Length;
}


public class WatchedBitArray
{
    private BitArray _content;
    public BitArray Content => _content;
    
    public bool this[int index]
    {
        get => _content[index];
        set
        {
            var oldValue = _content[index];
            _content[index] = value;
            if (!oldValue.Equals(value))
            {
                OnElementChanged?.Invoke(index, oldValue, value);
            }
        }
    }
    
    public Action<int, bool, bool> OnElementChanged;
    public WatchedBitArray(BitArray value)
    {
        _content = value;
    }
    
    public WatchedBitArray(int size)
    {
        _content = new BitArray(size);
    }

    public void Fill(bool value)
    {
        for (int i = 0; i < _content.Length; i++)
        {
            _content[i] = value;
        }
    }
    
    public int Length => _content.Length;
}


public class WatchedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TValue : IEquatable<TValue>
{
    private Dictionary<TKey, TValue> _dict;
    
    public Dictionary<TKey, TValue>.KeyCollection Keys => _dict.Keys;
    public Dictionary<TKey, TValue>.ValueCollection Values => _dict.Values;
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public TValue this[TKey key]
    {
        get => _dict[key];
        set
        {
            TValue oldValue = ContainsKey(key) ? _dict[key] : default;
            _dict[key] = value;
            if (!Utils.GenericEquals(value, oldValue))
            {
                OnElementChanged?.Invoke(key, oldValue, value);
            }
        }
    }
    
    public Action<TKey, TValue, TValue> OnElementChanged;
    
    public WatchedDictionary(Dictionary<TKey, TValue> value)
    {
        _dict = value;
    }
    
    public WatchedDictionary()
    {
        _dict = new Dictionary<TKey, TValue>();
    }
    
    public void Add(TKey key, TValue value)
    {
        _dict.Add(key, value);
        OnElementChanged?.Invoke(key, default, value);
    }
    
    public void Remove(TKey key)
    {
        var value = _dict[key];
        _dict.Remove(key);
        OnElementChanged?.Invoke(key, value, default);
    }
    
    public bool ContainsKey(TKey key)
    {
        return _dict.ContainsKey(key);
    }
    
    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dict.TryGetValue(key, out value);
    }
    
}