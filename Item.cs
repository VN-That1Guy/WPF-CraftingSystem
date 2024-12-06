using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static WPF_CraftingSystem.Library;

namespace WPF_CraftingSystem
{
    public enum ItemType
    {
        Object,
        Consumable,
        Ingredient
    }
    public class Item
    {
        public string ItemName { get; set; }
        public double Amount = 0;
        public double ItemValue = 0;
        public string ItemDescription = "Alan please add detail";
        public ItemType Type = ItemType.Object;

        public Item() 
        { 
            if (ItemName == "")
            {
                ItemName = "???";
            }
        }
        public Item(string itemName, double amount, double itemValue, string itemDescription, ItemType type)
        {
            ItemName = itemName;
            Amount = amount;
            ItemValue = itemValue;
            ItemDescription = itemDescription;
            Type = type;
        }

        public void ShowDescription()
        {
            Print(GetDescription());
        }
        public string GetDescription()
        { 
            return $"Item Name:\n{ItemName} ({ItemValue.ToString("c")}) X{Amount}\n (Type: {Type.ToString()})\n\nDescription:\n{ItemDescription}";
        }
    }

    public class Food : Item
    {
        new ItemType Type = ItemType.Consumable;
    }

}