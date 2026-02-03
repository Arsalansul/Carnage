using UnityEngine;

namespace Ui.Models
{
    public enum ItemType
    {
        None = 0,
        Mp5 = 1,
        RocketGun = 2,
        M4 = 3,
        Benelli = 4,
        M249 = 5
    }
    
    public struct InventoryItem
    {
        public ItemType Type { get; private set; }
        public Sprite Sprite { get; private set; }
        public int Amount { get; private set; }

        public InventoryItem(ItemType type, Sprite sprite, int amount)
        {
            Type = type;
            Sprite = sprite;
            Amount = amount;
        }
    }
}