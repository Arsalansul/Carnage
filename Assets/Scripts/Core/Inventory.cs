using System.Collections.Generic;
using Ui.Controllers;
using Ui.Models;

namespace Core
{
    public interface IInventory
    {
        void AddItem(ItemType type);
        void RemoveItem(ItemType type);
        void RemoveSelectedItem();
        void SelectItem(ItemType type);
    }
    
    public class Inventory : IInventory
    {
        public Inventory(InventoryController inventoryController, List<WeaponSettings> weaponSettingsList, List<WeaponTypeToItemType> weaponTypeToItemTypesMap)
        {
            this.inventoryController = inventoryController;
            this.weaponSettingsList = weaponSettingsList;
            this.weaponTypeToItemTypesMap = weaponTypeToItemTypesMap;
        }
        
        private InventoryController inventoryController;
        private List<WeaponSettings> weaponSettingsList;
        private List<WeaponTypeToItemType> weaponTypeToItemTypesMap;
        
        private Dictionary<ItemType, InventoryItem> inventoryItems = new ();
        private ItemType selectedItem;
        
        public void AddItem(ItemType itemType)
        {
            if (inventoryItems.TryGetValue(itemType, out var item))
            {
                inventoryItems[itemType] = new InventoryItem(itemType, item.Sprite, item.Amount + 1);
            }
            else
            {
                var weaponType = weaponTypeToItemTypesMap.Find(x => x.itemType == itemType).weaponType;
                inventoryItems.Add(itemType, new InventoryItem(itemType, weaponSettingsList.Find(s => s.type == weaponType).sprite,1));
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

        public void RemoveSelectedItem()
        {
            if (selectedItem == ItemType.None) return;

            RemoveItem(selectedItem);
            selectedItem = ItemType.None;
        }

        public void SelectItem(ItemType itemType)
        {
            if (!inventoryItems.TryGetValue(itemType, out var item)) return;

            selectedItem = itemType;
            inventoryController.Select(itemType);
        }
    }
}