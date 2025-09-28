 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionUtils
{
    ///<summary>Fisher-Yates shuffle algorithm to shuffle lists</summary>
    public static List<T> Shuffle<T>(this List<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = (int)Mathf.Floor(Random.value * ( i + 1 ));
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }
}

public class PairedDictionary<T1, T2> : IEnumerable<T1>
{
    private Dictionary<T1, T2> d1 = new Dictionary<T1, T2>();
    private Dictionary<T2, T1> d2 = new Dictionary<T2, T1>();
    
    public void Add(T1 t1, T2 t2)
    {
        d1.Add(t1, t2);
        d2.Add(t2, t1);
    }

    public void Add(T2 t2, T1 t1)
    {
        d1.Add(t1, t2);
        d2.Add(t2, t1);
    }

    public void Remove(T1 t)
    {
        if(d1.ContainsKey(t))
        {
            d1.TryGetValue(t, out T2 t2);
            d1.Remove(t);
            d2.Remove(t2);
        }
    }

    public void Remove(T2 t)
    {
        if (d2.ContainsKey(t))
        {
            d2.TryGetValue(t, out T1 t2);
            d2.Remove(t);
            d1.Remove(t2);
        }
    }

    public bool Contains(T1 t) => d1.ContainsKey(t);
    public bool Contains(T2 t) => d2.ContainsKey(t);

    public void Clear()
    {
        d1.Clear();
        d2.Clear();
    }

    public bool TryGetPair(T1 t, out T2 t2)
    {
        return d1.TryGetValue(t, out t2);
    }

    public bool TryGetPair(T2 t, out T1 t2)
    {
        return d2.TryGetValue(t, out t2);
    }

    public IEnumerator<T1> GetEnumerator()
    {
        return d1.Keys.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return d1.Keys.GetEnumerator();
    }
}

