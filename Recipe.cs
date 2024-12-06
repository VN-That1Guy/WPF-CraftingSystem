using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static WPF_CraftingSystem.Library;

namespace WPF_CraftingSystem
{
    public class Recipe
    {
        public string RecipeName {get; set;}
        public double Amount = 0;
        public double Price = 0;
        public List<Item> ItemRequirements = new List<Item>();
        public Item CraftedItem = new Item();

        public Recipe()
        {
            if (RecipeName == "" || RecipeName == null)
                RecipeName = "Mysterious Recipe";
        }
        public string ViewRecipe()
        {
            string output = "";
            output += $"{RecipeName} (Amount: {Amount}) (Value: {Price.ToString("c")})\n";
            foreach (Item itemreq in ItemRequirements)
            {
                output += $"    * {itemreq.ItemName} (x{itemreq.Amount})\n";
            }
            return output;
        }
    }
}