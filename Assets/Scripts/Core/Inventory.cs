using System.Collections.Generic;
using Ui.Controllers;
using Ui.Models;

namespace Core
{
    public interface IInventory
    {
        void AddItem(ItemType item);
        void RemoveItem(ItemType item);
    }
    
    public class Inventory : IInventory
    {
        public Inventory(InventoryController inventoryController, List<WeaponSettings> weaponSettingsList)
        {
            this.inventoryController = inventoryController;
            this.weaponSettingsList = weaponSettingsList;
        }
        
        private InventoryController inventoryController;
        private List<WeaponSettings> weaponSettingsList;
        private Dictionary<ItemType, InventoryItem> inventoryItems = new ();
        
        public void AddItem(ItemType itemType)
        {
            if (inventoryItems.TryGetValue(itemType, out var item))
            {
                inventoryItems[itemType] = new InventoryItem(itemType, item.Sprite, item.Amount + 1);
            }
            else
            {
                inventoryItems.Add(itemType, new InventoryItem(itemType, weaponSettingsList.Find(s => s.type == itemType).sprite,1));
            }
            
            inventoryController.SetItem(inventoryItems[itemType]);
        }

        public void RemoveItem(ItemType itemType)
        {
            if (!inventoryItems.TryGetValue(itemType, out var item)) return;
            if (item.Amount > 1)
            {
                inventoryItems[itemType] = new InventoryItem(itemType, item.Sprite, item.Amount - 1);
            
                inventoryController.SetItem(inventoryItems[itemType]);
            }
            else
            {
                inventoryItems.Remove(itemType);
                inventoryController.Remove(itemType);
            }
        }
    }
}