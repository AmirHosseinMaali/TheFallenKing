using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<InventoryItem> inventoryItems;
    public Dictionary<ItemData, InventoryItem> inventoryDict;

    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;

    private Ui_ItemSlot[] itemSlots;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        inventoryItems = new List<InventoryItem>();
        inventoryDict = new Dictionary<ItemData, InventoryItem>();

        itemSlots = inventorySlotParent.GetComponentsInChildren<Ui_ItemSlot>();
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            itemSlots[i].UpdateSlot(inventoryItems[i]);
        }
    }

    public void AddItem(ItemData _item)
    {
        if (inventoryDict.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventoryItems.Add(newItem);
            inventoryDict.Add(_item, newItem);
        }

        UpdateSlotUI();
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDict.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)
            {
                inventoryItems.Remove(value);
                inventoryDict.Remove(_item);
            }
        }
        else
        {
            value.RemoveStack();
        }

        UpdateSlotUI() ;
    }
}
