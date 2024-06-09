using Bloody.Core.Models.v1;
using Bloody.Core;
using BloodyBoss;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using System;
using Unity.Transforms;
using VampireCommandFramework;
using Bloody.Core.GameData.v1;
using Bloody.Core.API.v1;
using Stunlock.Core;
using ProjectM;

namespace BloodyBoss.Command
{
    [CommandGroup("bb")]
    public static class BossCommand
    {

        [Command("list", usage: "", description: "List of Boss", adminOnly: true)]
        public static void ListBoss(ChatCommandContext ctx)
        {

            var Boss = Database.BOSSES;

            if (Boss.Count == 0)
            {
                throw ctx.Error(Database.LOCALIZATIONS["MSG_No_Boss_Created"]);
            }
            ctx.Reply($"Boss List");
            ctx.Reply($"----------------------------");
            ctx.Reply($"--");
            foreach (var boss in Boss)
            {
                ctx.Reply($"Boss {boss.name}");
                ctx.Reply($"--");
            }
            ctx.Reply($"----------------------------");
        }

        [Command("test", usage: "", description: "Test Boss", adminOnly: true)]
        public static void Test(ChatCommandContext ctx, string bossName)
        {

            SetLocation(ctx,bossName);
            DateTime currentTime = DateTime.Now;
            DateTime x1MinLater = currentTime.AddMinutes(1);
            SetHour(ctx,bossName, x1MinLater.ToString("HH:mm"));
        }

        [Command("reload", usage: "", description: "Reload Database Boss", adminOnly: true)]
        public static void ReloadDatabase(ChatCommandContext ctx)
        {
            try
            {
                Database.loadDatabase();
                ctx.Reply(Database.LOCALIZATIONS["MSG_Boss_Database_Reload"]);
            } catch(Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
            
        }

        // .bb Test -1391546313 200 2 60 
        // .bb Boss items add Test ItemName -257494203 20 1
        // 
        // .bb Boss set hour Test 13:20
        // .bb Boss start Test
        // 
        //
        // .bb create "Alpha Wolf" -1905691330 90 1 1800
        // .bb set location "Alpha Wolf"
        // .bb set hour "Alpha Wolf" 22:00
        // .bb items add "Alpha Wolf" "Blood Rose Potion" 429052660 25 1
        // .bb start "Alpha Wolf"
        //
        // 
        [Command("create", usage: "<NameOfBOSS> <PrefabGUIDOfBOSS> <Level> <Multiplier> <LifeTimeSeconds>", description: "Create a Boss", adminOnly: true)]
        public static void CreateBOSS(ChatCommandContext ctx, string bossName, int prefabGUID, int level, int multiplier, int lifeTime)
        {
            try
            {
                var entityUnit = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(prefabGUID)];

                //if(!entityUnit.Has<VBloodUnit>()) throw ctx.Error($"The PrefabGUID entered does not correspond to a VBlood Unit.");

                if (Database.AddBoss(bossName, prefabGUID, level, multiplier, lifeTime))
                {
                    var _message = Database.LOCALIZATIONS["MSG_Boss_Create_Succesfully"];
                    _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(bossName));
                    ctx.Reply(_message);
                }
            }
            catch (BossExistException)
            {
                var _message = Database.LOCALIZATIONS["MSG_Boss_Does_Not_Exist"];
                _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(bossName));
                throw ctx.Error(_message);
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("remove", usage: "", description: "Remove a Boss", adminOnly: true)]
        public static void RemoveBoss(ChatCommandContext ctx, string bossName)
        {

            try
            {
                if (Database.RemoveBoss(bossName))
                {
                    var _message = Database.LOCALIZATIONS["MSG_Boss_Remove_Succesfully"];
                    _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(bossName));
                    ctx.Reply(_message);
                }
            }
            catch (NPCDontExistException)
            {
                var _message = Database.LOCALIZATIONS["MSG_Boss_Does_Not_Exist"];
                _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(bossName));
                throw ctx.Error(_message);
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("set location", usage: "<NameOfBoss>", description: "Adds the current location of the player who sets it to the Boss.", adminOnly: true)]
        public static void SetLocation(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                var pos = Plugin.SystemsCore.EntityManager.GetComponentData<LocalToWorld>(user).Position;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetLocation(pos);
                    var _message = Database.LOCALIZATIONS["MSG_Boss_Set_Position_Successfully"];
                    _message = _message.Replace("#posx#", FontColorChatSystem.Green(Convert.ToString(pos.x)));
                    _message = _message.Replace("#posy#", FontColorChatSystem.Yellow(Convert.ToString(pos.y)));
                    _message = _message.Replace("#posz#", FontColorChatSystem.Yellow(Convert.ToString(pos.z)));
                    _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(BossName));
                    ctx.Reply(_message);
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                var _message = Database.LOCALIZATIONS["MSG_Boss_Does_Not_Exist"];
                _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(BossName));
                throw ctx.Error(_message);
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("set hour", usage: "<NameOfBoss> <Hour>", description: "Adds the hour and minutes in which the Boss spawn.", adminOnly: true)]
        public static void SetHour(ChatCommandContext ctx, string BossName, string hour)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetHour(hour);
                    var _message = Database.LOCALIZATIONS["MSG_Boss_Set_Hour_Succesfully"];
                    _message = _message.Replace("#hour#", FontColorChatSystem.Green(hour));
                    _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(BossName));
                    ctx.Reply(_message);
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                var _message = Database.LOCALIZATIONS["MSG_Boss_Does_Not_Exist"];
                _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(BossName));
                throw ctx.Error(_message);
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("start", usage: "<NameOfBoss>", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void start(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetHourDespawn();
                    Boss.Spawn(user);
                    
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                var _message = Database.LOCALIZATIONS["MSG_Boss_Does_Not_Exist"];
                _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(BossName));
                throw ctx.Error(_message);
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        // Create new command for launch all boss with command
        [Command("startset", description: "The confrontation with all Boss begins.", adminOnly: true)]
        public static void startSet(ChatCommandContext ctx)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                var _bossList = Database.BOSSES;
                int _counter = 0;
                foreach (BossEncounterModel Boss in _bossList)
                {
                        Boss.SetHourDespawn();
                        Boss.Spawn(user, true);
                        _counter++;
                }

                var _message = Database.LOCALIZATIONS["MSG_Boss_Counter"];
                _message = _message.Replace("#counter#", FontColorChatSystem.Yellow(Convert.ToString(_counter)));
                ctx.Reply(_message);
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
        }

        [Command("clearicon", usage: "<NameOfBoss>", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void ClearIcon(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    UserModel userModel = GameData.Users.GetUserByCharacterName(ctx.Event.User.CharacterName.Value);
                    Boss.RemoveIcon(userModel.Entity);
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                var _message = Database.LOCALIZATIONS["MSG_Boss_Does_Not_Exist"];
                _message = _message.Replace("#boss#", FontColorChatSystem.Yellow(BossName));
                throw ctx.Error(_message);
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }
    }
}
