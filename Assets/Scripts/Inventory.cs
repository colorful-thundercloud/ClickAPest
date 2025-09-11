using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Inventory : MonoBehaviourPunCallbacks
{
    private static Inventory _instance;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject itemSlotPrefab;
    private GameObject obj;
    private List<ItemData> items = new();
    public PhotonView view;

    private void Awake()
    {
        _instance = this;
    }

    public static bool HasEmptySlots()
    {
        return _instance.items.Count < 3;
    }

    public static void AddItem(ItemData item)
    {
        _instance.items.Add(item);
        _instance.obj = Instantiate(_instance.itemSlotPrefab, _instance.itemsParent);
        _instance.obj.GetComponent<InventoryItem>().SetData(item, _instance.items.Count);
    }

    public static void RemoveItem(ItemData item)
    {
        Destroy(_instance.itemsParent.GetChild(_instance.items.IndexOf(item)).gameObject);
        _instance.items.Remove(item);
    }
}
