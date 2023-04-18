using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<string> items = new List<string>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
    }

    public bool RemoveItem(string itemName)
    {
        return items.Remove(itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public int ItemCount()
    {
        return items.Count;
    }
}
