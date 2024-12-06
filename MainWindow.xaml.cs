using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_CraftingSystem;

namespace WPF_CraftingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Engine engine = new Engine();

        Person person;
        Person lastperson;

        Person Client;
        Vendor Seller;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            engine.Start();
            MainWindow1.Title = engine.GetName();
            Client = engine.GetPlayer();
            Seller = engine.GetVendor();
            bCheckPerson();
            UpdatePlayerCurrency();
            UpdateInventoryInfo();
            RecipeComboBox.ItemsSource = engine.GetRecipes();
            RecipeList.ItemsSource = engine.GetRecipes();
            RecipeList.DisplayMemberPath = "RecipeName";
            RecipeComboBox.DisplayMemberPath = "RecipeName";
            BuyList.ItemsSource = Seller.Inventory;
            SellList.ItemsSource = Client.Inventory;
            BuyList.DisplayMemberPath = "ItemName";
            SellList.DisplayMemberPath = "ItemName";
        }

        private void UpdatePlayerCurrency()
        {
            PlayerInfo.Text = $"Your currency: {Client.Currency.ToString("c")}";
        }

        private void ChangePerson(Person newperson)
        {
            if (newperson != lastperson)
            {
                Avatar.Source = new BitmapImage(new Uri(newperson.ImagePath, UriKind.Relative));
                person = newperson;
                if (person == Seller)
                {
                    FlavorText.Text = $"{person.PersonName}: Welcome!";
                }
            }
            lastperson = person;
        }
        private bool bCheckPerson()
        {
            if (person == null)
            {
                person = Client;
            }
            return person != null;
        }

        private void UpdateInventoryInfo()
        {
            if (bCheckPerson())
            {
                Info_Inventory.Text = person.ShowInventory();
                PersonInfo.Content = person.PersonName + " " + person.Currency.ToString("c");
                AvatarName.Content = person.PersonName;
            }
        }

        private void RecipeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recipe recipe = (Recipe)RecipeComboBox.SelectedItem;
            CraftButton.IsEnabled = engine.RequiredItemsInInventory(recipe);
            FlavorText.Text = recipe.ViewRecipe();
        }

        private void CraftButton_Click(object sender, RoutedEventArgs e)
        {
            if (CraftButton.IsEnabled)
            {
                Client.AddToCollectionByName(engine.MakeItem((Recipe)RecipeComboBox.SelectedItem));
                UpdateInventoryInfo();
                SellList.Items.Refresh();
                CraftButton.IsEnabled = engine.RequiredItemsInInventory((Recipe)RecipeComboBox.SelectedItem);
                FlavorText.Text = "Item crafted!";
            }
        }


        private void RecipeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Recipe selectedRecipe = (Recipe)RecipeList.SelectedItem;
            FlavorText.Text = selectedRecipe.ViewRecipe();
        }

        private void ChangeNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeNameTextBox.Text == "")
                Client.PersonName = "Anonymous";
            else
                Client.PersonName = ChangeNameTextBox.Text;
            FlavorText.Text = "Your Name has been changed!";
            UpdatePlayerCurrency();
            UpdateInventoryInfo();
        }

        private void ChangeNameTextBox_Initialized(object sender, EventArgs e)
        {
            ChangeNameTextBox.Text = "";
        }

        private void ActionTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NameChangeTab.IsSelected || RecipeTab.IsSelected || CraftingTab.IsSelected)
            {
                ChangePerson(Client);
            }
            else if (BuyTab.IsSelected || SellTab.IsSelected)
            {
                ChangePerson(Seller);
                BuyList.ItemsSource = Seller.Inventory;
                SellList.ItemsSource = Client.Inventory;
            }

                UpdateInventoryInfo();
        }

        private string ItemShopSelectedInfo(Item item)
        {
            BuyItemCounter.Text = "Quantity: " + item.Amount.ToString() + "\nCost: " + item.ItemValue.ToString("c");
            return BuyItemCounter.Text;
        }

        private void BuyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Item item = (Item)BuyList.SelectedItem;
            if (BuyList.SelectedItem == null)
                return;
            FlavorText.Text = item.GetDescription();
            ItemShopSelectedInfo(item);
            if (item.Amount > 0)
                BuyButton.IsEnabled = Client.Currency >= item.ItemValue;
            else 
                BuyButton.IsEnabled = false;
        }

        private void SellList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Item item = (Item)SellList.SelectedItem;
            if (SellList.SelectedItem == null)
                return;
            FlavorText.Text = item.GetDescription();
            SellItemCounter.Text = ItemShopSelectedInfo(item);
            SellButton.IsEnabled = Seller.Currency >= item.ItemValue;
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            if (SellButton.IsEnabled)
            {
                if (SellList.SelectedIndex > Client.Inventory.Count)
                {
                    SellButton.IsEnabled = false;
                    return;
                }
                else
                {
                    Item item = Client.Inventory[SellList.SelectedIndex];
                    Seller.BuyItem(item);
                    Client.SellItem(item);
                    BuyList.Items.Refresh();
                    SellList.Items.Refresh();
                    SellItemCounter.Text = ItemShopSelectedInfo(item);
                    UpdatePlayerCurrency();
                    UpdateInventoryInfo();
                    FlavorText.Text = "Item Sold!";
                    if (item.Amount <= 0 || Seller.Currency < item.ItemValue)
                        SellButton.IsEnabled = false;
                }
                
            }
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (BuyButton.IsEnabled)
            {
                if (BuyList.SelectedIndex > Seller.Inventory.Count)
                {
                    BuyButton.IsEnabled = false;
                    return;
                }
                else
                {
                    Item item = Seller.Inventory[BuyList.SelectedIndex];
                    Client.BuyItem(item);
                    Seller.SellItem(item);
                    BuyList.Items.Refresh();
                    SellList.Items.Refresh();
                    UpdatePlayerCurrency();
                    UpdateInventoryInfo();
                    ItemShopSelectedInfo(item);
                    FlavorText.Text = $"Item bought!\n{Seller.PersonName}: Heheheh! Thank you.";
                    if (item.Amount <= 0)
                        BuyButton.IsEnabled = false;
                }
            }
        }
    }
}