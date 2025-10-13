using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Helpers
{
    public class EnchantmentsHelper
    {
        //includes pouch, backpack and equiped items
        public static List<EnchantmentRecipeItem> GetAvailableEnchantmentRecipeItemsInInventory(Item item, CharacterInventory inventory)
        {
            List<Item> inventoryItems = ItemsHelper.GetUniqueItemsInInventory(inventory);
            List<EnchantmentRecipeItem> enchantments = ItemsHelper.GetAllItemsOfType<EnchantmentRecipeItem>(inventoryItems);
            List<EnchantmentRecipeItem> haveEnchantments = new List<EnchantmentRecipeItem>();

            foreach (EnchantmentRecipeItem enchantmentRecipeItem in enchantments)
            {
                foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipeItem.Recipes)
                {
                    if (enchantmentRecipe.GetHasMatchingEquipment(item))
                    {
                        #if DEBUG
                        SL.Log($"{OutwardGameSettings.prefix} equiment {item.Name} can be enchanted with {enchantmentRecipe.name} and {enchantmentRecipeItem.Name}, {enchantmentRecipeItem.name}");
                        #endif
                        haveEnchantments.Add(enchantmentRecipeItem);
                    }
                }
            }

            return haveEnchantments;
        }

        public static List<EnchantmentRecipe> GetAvailableEnchantmentRecipies(Item item)
        {
            List<EnchantmentRecipe> enchantmentRecipes = RecipeManager.Instance.GetEnchantmentRecipes();
            List<EnchantmentRecipe> availableEnchantments = new List<EnchantmentRecipe>();

            foreach (EnchantmentRecipe enchantmentRecipe in enchantmentRecipes)
            {
                if (enchantmentRecipe.GetHasMatchingEquipment(item))
                {
                    #if DEBUG
                    SL.Log($"{OutwardGameSettings.prefix} ItemDescriptionsManager@SetEquipmentsEnchantmentsDescription equiment {item.Name} can be enchanted with {enchantmentRecipe.name}");
                    #endif
                    availableEnchantments.Add(enchantmentRecipe);
                }
            }

            return availableEnchantments;
        }

        public static List<EnchantmentRecipe> GetMissingEnchantments(List<EnchantmentRecipe> availableEnchantments, List<EnchantmentRecipe> haveEnchantments)
        {
            List<EnchantmentRecipe> missingEnchantments = new List<EnchantmentRecipe>();
            bool foundMissingRecipe = false;

            foreach (EnchantmentRecipe availableEnchantmentRecipe in availableEnchantments)
            {
                foreach (EnchantmentRecipe haveEnchantmentRecipe in haveEnchantments)
                {
                    if (availableEnchantmentRecipe.RecipeID == haveEnchantmentRecipe.RecipeID)
                    {
                        foundMissingRecipe = true;
                    }
                }

                if (foundMissingRecipe)
                    foundMissingRecipe = false;
                else
                {
                    missingEnchantments.Add(availableEnchantmentRecipe);
                }
            }

            return missingEnchantments;
        }

        public static bool IsEnchantmentInList(int enchantmentId, List<EnchantmentRecipeItem> enchantmentItems)
        {
            foreach(EnchantmentRecipeItem enchantmentItem in enchantmentItems)
            {
                foreach (EnchantmentRecipe enchantmentRecipe in enchantmentItem.Recipes)
                {
                    if (enchantmentRecipe.RecipeID == enchantmentId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static EnchantmentRecipeItem GetEnchantmentInTheList(int enchantmentId, List<EnchantmentRecipeItem> enchantmentItems)
        {
            foreach(EnchantmentRecipeItem enchantmentItem in enchantmentItems)
            {
                foreach (EnchantmentRecipe enchantmentRecipe in enchantmentItem.Recipes)
                {
                    if (enchantmentRecipe.RecipeID == enchantmentId)
                    {
                        return enchantmentItem;
                    }
                }
            }

            return null;
        }

        // Fixes Game Developers Bug 
        public static void FixFilterRecipe()
        {
            Item filter = ResourcesPrefabManager.Instance.GetItemPrefab("5800047");

            if (filter == null)
                return;

            #if DEBUG
            SL.Log($"{OutwardGameSettings.prefix} EnchantmentsHelper@FixFilterRecipe got item Filter!");
            #endif

            EnchantmentRecipeItem filterRecipeItem = filter as EnchantmentRecipeItem;
            if(!filterRecipeItem)
            {
                return;
            }

            EnchantmentRecipe[] filterRecipes = filterRecipeItem.Recipes;

            if (filterRecipes.Count() == 3)
                return;

            EnchantmentRecipe filterArmor = RecipeManager.Instance.GetEnchantmentRecipeForID(52);
            EnchantmentRecipe filterHelmet = RecipeManager.Instance.GetEnchantmentRecipeForID(53);
            EnchantmentRecipe filterBoots = RecipeManager.Instance.GetEnchantmentRecipeForID(54);

            if(filterArmor == null || filterHelmet == null || filterBoots == null)
            {
                return;
            }

            EnchantmentRecipe[] filterSet = new EnchantmentRecipe[] { filterArmor, filterHelmet, filterBoots };
            filterRecipeItem.Recipes = filterSet;

            #if DEBUG
            SL.Log($"{OutwardGameSettings.prefix} EnchantmentsHelper@FixFilterRecipe fixed item Filter!");
            #endif
        }
    }
}
