using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<string> keys = new List<string>();

    public void AddKey(string key)
    {
        keys.Add(key);
    }

    public bool HasKey(string key)
    {
        return keys.Contains(key);
    }
}
