using System;
using System.Collections.Generic;
using Ui.Models;
using Ui.View;
using UnityEngine;
using Zenject;

namespace Ui.Controllers
{
    public class InventoryController : MonoBehaviour
    {
        [Inject] private InventorySlot.Pool inventorySlotPool;

        [SerializeField] private Transform inventoryParent;

        private Dictionary<ItemType, InventorySlot> items = new();
        
        public void SetItem(InventoryItem item)
        {
            if (items.TryGetValue(item.Type, out var slot))
            {
                slot.SetCount(item.Amount);
            }
            else
            {
                var newSlot = inventorySlotPool.Spawn(inventoryParent);
                newSlot.SetSprite(item.Sprite);
                newSlot.SetCount(item.Amount);
                items.Add(item.Type, newSlot);
            }
        }

        public void Remove(ItemType itemType)
        {
            items.Remove(itemType);
        }

        public void Select(ItemType itemType)
        {
            foreach (var item in items)
            {
                item.Value.SetSelected(itemType == item.Key);
            }
        }
    }
}