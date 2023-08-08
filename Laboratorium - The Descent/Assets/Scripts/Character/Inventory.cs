using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Inventory stores strings (keys) and can check if player has
    // a key whose code matches a locked door
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
