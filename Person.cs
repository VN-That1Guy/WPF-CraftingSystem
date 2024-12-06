using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static WPF_CraftingSystem.Library;

namespace WPF_CraftingSystem
{
    public class Person
    {
        public string PersonName = "Anonymous";
        public double Currency = 10.5;
        public List<Item> Inventory = new List<Item>();
        public string ImagePath = "Player.bmp";

        public Person(string Name)
        {
            PersonName = Name;
            //TestData
            /*Inventory.Add(new Item() { ItemName = "Soup", Amount = 10, ItemValue = 5, ItemDescription = "mmm, delicious chicken soup!" });
            Inventory.Add(new Item() { ItemName = "Fish Tank", Amount = 1, ItemValue = 20, ItemDescription = "A big fish tank complete with architectural decorations and a small yellow submarine" });
            Inventory.Add(new Item() { ItemName = "Shrimp", Amount = 20, ItemValue = 1 });
            Inventory.Add(new Item() { Amount = 0, ItemDescription = "This should not show up!" });*/
            //

        }

        public Person(string Name, double CurrencyAmount)
        {
            PersonName = Name;
            Currency = CurrencyAmount;
        }

        public string About()
        {
            // interpolation
            string Output = $"{PersonName} {Currency.ToString("c")}";

            // concatenation
            // string output = PersonName + " " + Currency.ToString("c");

            // composite formatting
            // string Output = "{0}  {1.ToString("c")",PersonName, Currency;
            return Output;
        }
        public string ShowInventory()
        {
            string output = $"Inventory for {PersonName}:\n";

            foreach (Item item in Inventory)
            {
                if (item.Amount > 0)
                    output += $"{item.ItemName} ({item.ItemValue.ToString("c")})[x{item.Amount}]\n{item.ItemDescription}\n";
            }

            return output;
        }

        public void BuyItem(Item item)
        {
            if (item.Amount > 0)
            {
                Currency -= item.ItemValue;
                AddToCollectionByName(item);
            }
        }

        public void SellItem(Item item)
        {
            if (item.Amount > 0) 
            {
                Currency += item.ItemValue;
                RemoveFromCollectionByName(item);
            }
        }

        public void AddToCollectionByName(Item itemName)
        {
            if (itemName != null)
            {
                if (!SearchByItemName(itemName.ItemName))
                {
                    Inventory.Add(new Item() { ItemName = itemName.ItemName, ItemValue = itemName.ItemValue, ItemDescription = itemName.ItemDescription, Amount = 1 });
                }
                else
                {
                    foreach (Item item in Inventory)
                    {
                        if (item.ItemName == itemName.ItemName)
                        {
                            item.Amount++;
                            break;
                        }
                    }
                }
            }
        }

        public virtual void RemoveFromCollectionByName(Item itemName)
        {
            itemName.Amount--;
            if (itemName.Amount <= 0)
                Inventory.Remove(itemName);
        }

        public string FindItemInInventory(string itemName)
        {
            string output = "You";
            if (SearchByItemName(itemName))
            {
                foreach (Item item in Inventory)
                {
                    if (item.ItemName.ToLower() == itemName.ToLower())
                    {
                        if (item.Amount > 0)
                        {
                            output += " have";
                            if (item.Amount == 1)
                                output += " a";
                            else if (item.Amount > 10)
                                output += " a lot of";
                            else
                                output += " a couple of";
                        }
                        else
                            output += " do not have a";
                        output += $" {item.ItemName}";
                        if (item.Amount > 1)
                            output += "s";
                    }
                }
            }
            else
                output += " do not have a " + itemName;
            return output;
        }

        public bool SearchByItemName(string name)
        {
            //string output = "You";
            //Item item;
            foreach (Item item in Inventory)
            {
                if (item.ItemName.ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            /*if (Inventory != null)
            {
                item = Inventory.Find(x => x.ItemName.ToLower().Contains(name.ToLower()));
                if (item != null)
                {
                    if (item.Amount > 0)
                    {
                        output += " have";
                        if (item.Amount == 1)
                            output += " a";
                        else if (item.Amount > 10)
                            output += " a lot of";
                        else
                            output += " a couple of";
                    }
                    else
                        output += " do not have a";
                    output += $" {item.ItemName}";
                    if (item.Amount > 1)
                        output += "s";
                    Console.WriteLine(output);
                    return true;
                }
            }*/
            //Console.WriteLine("Item not Found");
            return false;
        }
    }

    public class Vendor : Person
    {
        public Vendor(string Name) : base(Name)
        {
        }

        public Vendor(string Name, double CurrencyAmount) : base(Name, CurrencyAmount)
        {
        }

        public override void RemoveFromCollectionByName(Item itemName)
        {
            itemName.Amount--;
        }
    }
}