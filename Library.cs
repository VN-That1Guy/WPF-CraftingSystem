using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static System.Console;

namespace WPF_CraftingSystem
{
    public class Library
    {
        public static string datafilepath = "../../../data/";
        
        static public void Print(string message)
        {
            WriteLine(message);
        }

        public static string GetInput(string input)
        {
            return input;
        }

        public static void ClearScreen()
        {
            Clear();
        }

        public static string ViewList(Person Player, bool bIsVendor = false)
        {
            string output = $"Items ({Player.PersonName}):\n";

            foreach (Item items in Player.Inventory)
            {
                if (items.Amount > 0)
                {
                    output += $"{items.ItemName} (x{items.Amount})[{items.ItemValue.ToString("c")}]\n";
                    if (!bIsVendor)
                    {
                        output += $"{items.ItemDescription}\n";
                    }
                }
            }
            return output;
        }

        public static string WelcomeTxtFile()
        {
            string input = "Error, File not found!";
            string filename = datafilepath + "welcome.txt";
            if (File.Exists(filename))
            {
                input = File.ReadAllText(filename);
                // string[] input ReadAllLines(string) can do multiple lines (New lines for each new line in a txt instead of one long text in ReadAllText())
                /*
                    Followed by a foreach (string s in input)
                    {
                        output += s + Environment.NewLine;
                    }
                 */
            }

            return input;
        }

        public static string GetDataFilePath()
        {
            return datafilepath;
        }

        public static List<Recipe> LoadRecipes(string fileName)
        {
            List<Recipe> temprecipes = new List<Recipe>();
            XmlDocument doc = new XmlDocument();
            if (File.Exists(fileName))
            {
                doc.Load(fileName);
                XmlNode root = doc.DocumentElement;
                XmlNodeList recipeList = root.SelectNodes("/Recipes/Recipe");
                XmlNodeList ingredientsList;

                foreach (XmlElement recipe in recipeList)
                {
                    Recipe recipeToAdd = new Recipe();
                    recipeToAdd = new Recipe();
                    recipeToAdd.RecipeName = recipe.GetAttribute("RecipeName");
                    string AmountDoubleToString = recipe.GetAttribute("Amount");
                    if (double.TryParse(AmountDoubleToString, out double valueA))
                    {
                        recipeToAdd.Amount = valueA;
                    }
                    AmountDoubleToString = recipe.GetAttribute("Price");
                    if (double.TryParse(AmountDoubleToString, out double valueB))
                    {
                        recipeToAdd.Price = valueB;
                    }

                    ingredientsList = recipe.ChildNodes;
                    //ingredientsList = root.SelectNodes("Recipes/Recipe/ItemRequirements");

                    foreach (XmlElement i in ingredientsList)
                    {
                        string iType = i.GetAttribute("Type");
                        if (i.Name == "ItemRequirements")
                        {
                            string ingredientName = i.GetAttribute("ItemName");
                            string ingredientAmountString = i.GetAttribute("Amount");
                            double ingredientAmount = 0;
                            if (double.TryParse(ingredientAmountString, out double e))
                            {
                                ingredientAmount = e;
                            }
                            if (Enum.TryParse(iType, out ItemType itemType))
                                recipeToAdd.ItemRequirements.Add(new Item() { ItemName = ingredientName, Amount = ingredientAmount, Type = itemType });
                            else
                                Print("Error! Item Type Enum missing!!! Recipe not added");
                        }
                        else if (i.Name == "CraftedItem")
                        {
                            recipeToAdd.CraftedItem.ItemName = i.GetAttribute("ItemName");
                            recipeToAdd.CraftedItem.ItemDescription = i.GetAttribute("ItemDescription");
                            AmountDoubleToString = i.GetAttribute("ItemValue");
                            if (double.TryParse(AmountDoubleToString, out double value))
                                recipeToAdd.CraftedItem.ItemValue = value;
                            AmountDoubleToString = i.GetAttribute("Amount");
                            if (double.TryParse(AmountDoubleToString, out value))
                                recipeToAdd.CraftedItem.Amount = value;
                            if (Enum.TryParse(iType, out ItemType itemType))
                                recipeToAdd.CraftedItem.Type = itemType;
                        }
                    }

                    temprecipes.Add(recipeToAdd);
                }
            }
            return temprecipes;
        }
        // Disabled: Does not like saving
        /*public static void SaveItems(List<Item> ItemsToSave)
        {
            string PlayerInvFile = GetDataFilePath() + "PlayerInventory.xml";
            if (File.Exists(PlayerInvFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(PlayerInvFile);
                XmlNode rootNode = doc.SelectSingleNode("ArrayOfItem");
                XmlNodeList inventorylist = rootNode.SelectNodes("/ArrayOfItem/Item");
                
                for (int i = 0; i < ItemsToSave.Count; i++)
                {
                    if (i >= inventorylist.Count)
                    {
                        XmlNode newNode = doc.CreateElement("Item");
                        XmlNode TempName        = doc.CreateElement("ItemName");
                        XmlNode TempAmount      = doc.CreateElement("Amount");
                        XmlNode TempValue       = doc.CreateElement("ItemValue");
                        XmlNode TempDescription = doc.CreateElement("ItemDescription");
                        XmlNode TempType        = doc.CreateElement("Type");
                        TempName.InnerXml          = ItemsToSave[i].ItemName;
                        TempAmount.InnerXml        = ItemsToSave[i].Amount.ToString();
                        TempValue.InnerXml         = ItemsToSave[i].ItemValue.ToString();
                        TempDescription.InnerXml   = ItemsToSave[i].ItemDescription;
                        TempType.InnerXml          = ItemsToSave[i].Type.ToString();
                        newNode.AppendChild(TempName);
                        newNode.AppendChild(TempAmount);
                        newNode.AppendChild(TempValue);
                        newNode.AppendChild(TempDescription);
                        newNode.AppendChild(TempType);
                        rootNode.AppendChild(newNode);
                    }
                    foreach (XmlElement r in inventorylist)
                    {
                        XmlNodeList NameList = r.SelectNodes("/ArrayOfItem/Item/ItemName");
                        foreach (XmlNode Names in NameList)
                        {
                            if (ItemsToSave[i].ItemName == Names.InnerText)
                            {
                                foreach (XmlElement Child in r.ChildNodes)
                                {
                                    switch (Child.Name)
                                    {
                                        case "ItemName":
                                            Child.InnerText = ItemsToSave[i].ItemName;
                                            break;
                                        case "Amount":
                                            Child.InnerText = ItemsToSave[i].Amount.ToString();
                                            break;
                                        case "ItemValue":
                                            Child.InnerText = ItemsToSave[i].ItemValue.ToString();
                                            break;
                                        case "ItemDescription":
                                            Child.InnerText = ItemsToSave[i].ItemDescription;
                                            break;
                                        case "Type":
                                            Child.InnerText = ItemsToSave[i].Type.ToString();
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                doc.Save(PlayerInvFile);
            }
        }

        public static List<Item> LoadItems()
        {
            List<Item> ItemsToLoad = new List<Item>();
            string PlayerInvFile = GetDataFilePath() + "PlayerInventory.xml";
            if (File.Exists(PlayerInvFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(PlayerInvFile);
                XmlNode rootNode = doc.SelectSingleNode("ArrayOfItem");
                XmlNodeList inventorylist = rootNode.SelectNodes("/ArrayOfItem/Item");

                foreach (XmlElement r in inventorylist)
                {
                    Item NewItem = new Item();
                    foreach (XmlElement Child in r.ChildNodes)
                    {
                        switch (Child.Name)
                        {
                            case "ItemName":
                                NewItem.ItemName            = Child.InnerText;
                                break;
                            case "Amount":
                                if (double.TryParse(Child.InnerText, out double dAmount))
                                    NewItem.Amount              = dAmount;
                                break;
                            case "ItemValue":
                                if (double.TryParse(Child.InnerText, out dAmount))
                                    NewItem.ItemValue           = dAmount;
                                break;
                            case "ItemDescription":
                                NewItem.ItemDescription     = Child.InnerText;
                                break;
                            case "Type":
                                if(Enum.TryParse(Child.InnerText, out ItemType itemtype))
                                    NewItem.Type            = itemtype;
                                break;
                        }
                    }
                    ItemsToLoad.Add(NewItem);
                }
            }
            return ItemsToLoad;
        }*/
    }
}