using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Console;
using static WPF_CraftingSystem.Library;

namespace WPF_CraftingSystem
{
    public class Engine
    {
        private string Name = "Nonsense Crafting!";
        public string SearchTerm;
        private List<Recipe> Recipes = new List<Recipe>();
        Person Player = new Person("Anonymous", 300);
        Vendor Vendor = new Vendor("Alan The Vendor", 250) { ImagePath = "Vendor.bmp" };

        public Person GetPlayer()
        {
            return Player;
        }

        public Vendor GetVendor()
        {
            return Vendor;
        }
        public void Start()
        {
            //string PlayerInvFile = GetDataFilePath() + "PlayerInventory.xml";
            //if (!File.Exists(PlayerInvFile))
            //{
            //    XmlSerializer serializer = new XmlSerializer(typeof(List<Item>));
            //    serializer.Serialize(File.Create(PlayerInvFile), Player.Inventory);
            //}
            // Temp Data
            /*Recipes.Add(new Recipe() { Amount = 1, RecipeName = "Nonsense Pie", Price = 10,
                ItemRequirements = new List<Item>()
                {
                    new Item() { ItemName = "Soup", Amount = 1 },
                    new Item() { ItemName = "Fish Tank", Amount = 1 },
                    new Item() { ItemName = "Shrimp", Amount = 5 },
                },
                CraftedItem = new Item() { Amount= 1,ItemName = "Nonsense Pie", ItemValue = 100, ItemDescription = "A pie that makes no sense!"}
            });*/


            Vendor.Inventory.Add(new Food() { ItemName = "Soup", Amount = 30, ItemValue = 5, ItemDescription = "mmm, delicious chicken soup!" });
            Vendor.Inventory.Add(new Item() { ItemName = "Fish Tank", Amount = 2, ItemValue = 20, ItemDescription = "A big fish tank complete with architectural decorations and a small yellow submarine", Type = ItemType.Ingredient });
            Vendor.Inventory.Add(new Food() { ItemName = "Shrimp", Amount = 50, ItemValue = 1 });

            //Player.Inventory = LoadItems();
            //Vendor.Inventory.Add(new Item() { Amount = 0, ItemDescription = "This should not show up!" });

            //
            //Title = Name;

            //SetPlayerName();
            //Print(WelcomeTxtFile()+ $" {Player.PersonName}.\n");
            Recipes = LoadRecipes(GetDataFilePath() + "Recipes.xml");
            //ReadKey();
            //Menu();
        }

        public string GetName()
        {
            return Name;
        }
        public List<Recipe> GetRecipes()
        {
            return Recipes;
        }
       private void SetPlayerName()
        {
            ConsoleKey Input;
            do
            {
                ClearScreen();
                Print("Would you like to set your name? y or n");
                ConsoleKeyInfo keyInfo = ReadKey();
                Input = keyInfo.Key;
                if (Input == ConsoleKey.Y)
                {
                    Print("\nEnter your name: ");
                    Player.PersonName = ReadLine();
                    break;
                }
                if (Input == ConsoleKey.N)
                {
                    WriteLine("\n");
                    break;
                } 
            }
            while (Input != ConsoleKey.Enter);
        }

        public bool CheckItemAmount(List<Item> items, Item itemB)
        {
            double CurrentAmount = 0;
            foreach (Item item in items) 
            {
                if (item.ItemName.ToLower() == itemB.ItemName.ToLower())
                {
                    if (item.Amount >= itemB.Amount)
                        return true;
                    else
                        CurrentAmount = item.Amount;
                }
            }
            Print($"Required Amount not found for {itemB.ItemName}! ({CurrentAmount}/{itemB.Amount})");
            return false;
        }
        public bool RequiredItemsInInventory(Recipe recipe)
        {
            bool bPass = true;
            string Output = "Missing:";
            foreach (Item item in recipe.ItemRequirements)
            {
                if (CheckItemAmount(Player.Inventory, item))
                    continue;
                else
                {
                    Output += $"\n* {item.ItemName}";
                    bPass = false;
                }
            }
            if (!bPass)
                Print(Output);
            return bPass;
        }

        public string ViewRecipes()
        {
            string output = $"Recipes:\n";
            int i = 1;
            foreach (Recipe item in Recipes)
            {
                output += $"{i}. {item.RecipeName}\n";
                i++;
            }
            return output;
        }

        public string ShowRecipe()
        {
            int io;
            string Prompt;
            string output = "Error!";
            bool Exiting = false;
            do
            {
                ClearScreen();
                Print(ViewRecipes());
                Print("Type the index number of a recipe that you want to view: ");
                Prompt = ReadLine();
                if (int.TryParse(Prompt, out io) && io <= Recipes.Count && io > 0)
                {
                    output = Recipes[io - 1].ViewRecipe();
                    Exiting = true;
                }
                else
                {
                    Print("Please enter a valid number");
                    ReadKey();
                }
            }
            while (Exiting == false);

            Print(output);
            return output;
        }

        public Item MakeItem(Recipe recipe)
        {
            List<Item> cacheditems = new List<Item>(); // Store a list of items to possibly remove
            //if all the required elements for the recipe are in the player's inventory
            // AND the amounts of those elements are equal to or greater than what is specified in the recipe
            // then remove the ingredient items from the player's inventory or modify the amounts of the items
            // and return an instance of the Item class that matches the recipe
            if (Player.Inventory != null && RequiredItemsInInventory(recipe))
            {
                foreach (Item recipeitem in recipe.ItemRequirements)
                {
                    foreach (Item item in Player.Inventory)
                    {
                        if (recipeitem.ItemName.ToLower() == item.ItemName.ToLower())
                        {
                            cacheditems.Add(item);
                            item.Amount -= recipeitem.Amount;
                        }
                    }
                }
                foreach(Item item in cacheditems)
                {
                    if (item.Amount <= 0)
                        Player.RemoveFromCollectionByName(item);
                }
                Print("Crafting Successful!");
            }
            else
            {
                Print("Item Requirements not met!");
                return null;
            }
            return recipe.CraftedItem;
        }

        private void Menu()
        {
            ClearScreen();
            Title = $"{Name} - {Player.PersonName} - Money: {Player.Currency.ToString("c")}";
            string[] MenuChoices = { "1. About yourself", "2. About the Vendor", "3. Show your inventory", "4. View all Recipes", "5. View Single Recipe", "6. Change Name", "7. Search for Item in Inventory", "8. View Vendor's items", "9. Craft an Item", "10. Go Shopping", "11. Sell Items", "0. Exit" };
            Print("Enter a Number from this list:");
            foreach (string choice in MenuChoices)
            {
                Print(choice);
            }
            switch(ReadLine())
            {
                case "1":
                    Print(Player.About());
                    break;
                case "2":
                    Print(Vendor.About());
                    break;
                case "3":
                    Print(ViewList(Player));
                    break;
                case "4":
                    Print(ViewRecipes());
                    break;
                case "5":
                    ShowRecipe();
                    break;
                case "6":
                    SetPlayerName();
                    break;
                case "7":
                    Print("Enter the name of the item you are searching for: ");
                    Print(Player.FindItemInInventory(ReadLine()));
                    break;
                case "8":
                    Print(ViewList(Vendor, true));
                    break;
                case "9":
                    Print(ViewRecipes());
                    Print("Enter an index number from the recipes list to craft:");
                    if (int.TryParse(ReadLine(), out int i) && i <= Recipes.Count && i > 0)
                    {
                        Player.AddToCollectionByName(MakeItem(Recipes[i - 1]));
                    }
                    else
                        Print("Enter a valid number.");
                    break;
                case "10":
                    ShowShopMenu(false);
                    break;
                case "11":
                    if (Player.Inventory.Count != 0)
                        ShowShopMenu(true);
                    else
                        Print("You do not have anything to sell!");
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
            }
            //SaveItems(Player.Inventory);
            Print("\nPress Any Button to Continue");
            ReadKey();
            Menu();            
        }
        public void ShowShopMenu(bool bSelling)
        {
            int i;
            string Prompt;
            bool Exiting = false;
            do
            {
                i = 0;
                ClearScreen();
                Print($"{Vendor.PersonName}'s Store: (You have {Player.Currency.ToString("c")})");
                if (!bSelling) 
                {
                    foreach (Item items in Vendor.Inventory)
                    //for(i=0;i < Vendor.Inventory.Count; i++)
                    {
                        i++;
                        Print($"{i}. {items.ItemName} ({items.ItemValue.ToString("c")})[x{items.Amount}]\n");
                        //Print($"{i+1}. {Vendor.Inventory[i].ItemName} ({Vendor.Inventory[i].ItemValue.ToString("c")})[x{Vendor.Inventory[i].Amount}]\n");
                    }
                }
                else
                {
                    foreach (Item items in Player.Inventory)
                    //for(i=0;i < Player.Inventory.Count; i++)
                    {
                        i++;
                        Print($"{i}. {items.ItemName} ({items.ItemValue.ToString("c")})[x{items.Amount}]\n");
                        //Print($"{i+1}. {Player.Inventory[i].ItemName} ({Player.Inventory[i].ItemValue.ToString("c")})[x{Player.Inventory[i].Amount}]\n");
                    }
                }
                Print("Type the index number of an item you want to buy/sell (or type exit to leave): ");
                Prompt = ReadLine();
                if (Prompt != null && Prompt.ToLower() == "exit")
                    Exiting = true;
                else if (int.TryParse(Prompt, out int io))
                {
                    io--;
                    switch (bSelling)
                    {
                        case true:
                            if (io >= Player.Inventory.Count)
                                Print("Please enter a valid number");
                            else if (Player.Inventory.Count <= 0)
                            {
                                Print("You don't have anything! Type exit to leave this menu");
                            }
                            else if (io < Player.Inventory.Count && io >= 0)
                            {
                                if (Player.Inventory[io].Amount >= 0)
                                {
                                    Vendor.BuyItem(Player.Inventory[io]);
                                    Player.SellItem(Player.Inventory[io]);
                                    Print("Item Sold!");
                                }
                                else if (Vendor.Currency <= Player.Inventory[io].ItemValue)
                                    Print("I cannot afford this, Stranger.");
                            }
                            break;
                        case false:
                            if (io > Vendor.Inventory.Count)
                                Print("Please enter a valid number");
                            else if (io < Vendor.Inventory.Count && io >= 0)
                            {
                                if (Vendor.Inventory[io].Amount <= 0)
                                    Print("I'm out of stock!");
                                else if (Player.Currency >= Vendor.Inventory[io].ItemValue)
                                {
                                    Player.BuyItem(Vendor.Inventory[io]);
                                    Vendor.SellItem(Vendor.Inventory[io]);
                                    Print("Purchase Successful!");
                                }
                                else
                                    Print("Not enough Cash! Stranger.");
                            }
                            break;
                    }
                    
                    ReadKey();
                }
            }
            while (Exiting == false);
            if (Exiting)
                Print("See you around, stranger.");
        }
    }
}