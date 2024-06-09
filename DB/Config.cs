using System.Collections.Generic;

namespace BloodyBoss.DB
{
    internal static class Config
    {
        public static Dictionary<string, string> Localization = new()
        {
            { "MSG_No_Boss_Created", "There are no boss created"},
            { "MSG_Boss_Database_Reload", "Boss database reload successfully"},
            { "MSG_Boss_Create_Succesfully", "Boss #boss# created successfully"},
            { "MSG_Boss_Does_Not_Exist", "Boss with name #boss# does not exist." },
            { "MSG_Boss_Remove_Succesfully", "Boss #boss# remove successfully" },
            { "MSG_Boss_Set_Position_Successfully", "Position #posx#,#posy#,#posz# successfully set to Boss '#boss#'"},
            { "MSG_Boss_Set_Hour_Succesfully", "Hour #hour# successfully set to Boss #boss#'"},
            { "MSG_Kill_Boss_Template", "The #vblood# boss has been defeated by the following brave warriors:"},
            { "MSG_Spawn_Boss_Template", "A Boss #worldbossname# has been summon you got #time# minutes to defeat it!"},
            { "MSG_Despawn_Boss_Template", "You failed to kill the Boss #worldbossname# in time."},
            { "MSG_Final_Concat_Character", "and" },
            { "MSG_Item_Already_Exist", "This item configuration already exists at Boss #boss#" },
            { "MSG_Item_Added_Successfully", "Item successfully added to Boss #boss#" },
            { "MSG_Item_Removed_Succesfully", "Boss #boss# item has been successfully removed" },
            { "MSG_Item_Does_Not_Exist", "This item does not exist at Boss '#boss#'" },
            { "MSG_Boss_Counter", "#counter# boss has spawned"}
        };
    }
}