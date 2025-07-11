using BloodyBoss.DB.Models;
using BloodyBoss.DB;
using ProjectM;
using System.Linq;
using VampireCommandFramework;
using BloodyBoss.Exceptions;
using System;

namespace BloodyBoss.Command
{
    [CommandGroup("bb item", "Boss item drop management commands")]
    public static class BossItemCommand
    {

        [Command("list", usage: "<NameOfBoss>", description: "List items of Boss drop", adminOnly: true)]
        public static void ListBossItems(ChatCommandContext ctx, string BossName)
        {

            try
            {
                if (Database.GetBoss(BossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"{boss.name} Items List");
                    ctx.Reply($"----------------------------");
                    ctx.Reply($"--");
                    foreach (var item in boss.GetItems())
                    {
                        ctx.Reply($"Item {item.ItemID}");
                        ctx.Reply($"Stack {item.Stack}");
                        ctx.Reply($"Chance {item.Chance}");
                        ctx.Reply($"--");
                    }
                    ctx.Reply($"----------------------------");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (ProductExistException)
            {
                throw ctx.Error($"This item configuration already exists at Boss '{BossName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
            
        }

        // .encounter product add Test1 736318803 20
        [Command("add", usage: "<NameOfBoss> <ItemName> <ItemPrefabID> <Stack> <Chance>", description: "Add a item to a Boss drop. Chance is number between 0 to 1, Example 0.5 for 50% of drop", adminOnly: true)]
        public static void CreateItem(ChatCommandContext ctx, string BossName, string ItemName, int ItemPrefabID, int Stack, float Chance)
        {
            try
            {
                if(Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.AddItem(ItemName,ItemPrefabID, Stack, Chance);
                    ctx.Reply($"Item successfully added to Boss '{BossName}'");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            } 
            catch (ProductExistException)
            {
                throw ctx.Error($"This item configuration already exists at Boss '{BossName}'");
            } 
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }

        // .encounter product remove Test1 736318803
        [Command("remove", usage: "<NameOfBoss> <ItemName>", description: "Remove a item to a Boss", adminOnly: true)]
        public static void RemoveProduct(ChatCommandContext ctx, string BossName, string ItemName)
        {
            try
            {
                if (Database.GetBoss(BossName, out BossEncounterModel npc))
                {
                    npc.RemoveItem(ItemName);
                    ctx.Reply($"Boss '{BossName}'\'s item has been successfully removed");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (ProductDontExistException)
            {
                throw ctx.Error($"This item does not exist at Boss '{BossName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

            
        }
    }
}
