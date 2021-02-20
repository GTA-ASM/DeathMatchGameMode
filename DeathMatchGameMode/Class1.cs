using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SanAndreasUnity.Behaviours;
using SanAndreasUnity.Utilities;
using SanAndreasUnity.Net;
using SanAndreasUnity.Chat;
using SanAndreasUnity.GameModes;
using SanAndreasUnity.Stats;

namespace DeathMatchGameMode
{

    public class DeathmatchGamemode : PluginManager.PluginBase
    {
        public DeathmatchGamemode()
        {
            // our plugin is loaded

            // send a message to the console to signal that everything is allright
            Debug.Log("Deathmatch gamemode loaded");

            // register our gamemode
            Register();

        }

        static void Register()
        {
            // register gamemode at GameModeManager
            // if our gamemode is selected when starting the server, the callback function (Setup) will be called
            GameModeManager.Instance.RegisterGameMode(new GameModeManager.GameModeInfo("Deathmatch Area 69", "", Setup));
        }

        static void Setup()
        {
            // send chat message to all new players saying that this gamemode is running
            Player.onStart += player =>
            {
                ChatManager.SendChatMessageToPlayer(player, "Welcome to the server ! This server is running deathmatch gamemode. Type '/help' for a list of commands.");
            };

            // assign spawn handler
            SpawnManager.Instance.SpawnHandler = new MySpawnHandler();

            // don't add random weapon to ped


            // register help command which will display all possible commands


            // register command for selecting weapon (/w name)


            // when player is killed, we need to update score
            Ped.onDamaged += (ped, damageInfo, damageResult) =>
            {
                // ped is damaged, check if he died
                if (ped.Health <= 0)
                {
                    var deadPlayer = ped.PlayerOwner;
                    if (deadPlayer != null)
                    {
                        // increase number of deaths for dying player
                        deadPlayer.ExtraData.SetInt("NumDeaths", deadPlayer.ExtraData.GetInt("NumDeaths") + 1);

                        // increase number of kills for killer
                        var killerPlayer = damageInfo.attackingPlayer as Player;
                        if (killerPlayer != null)
                            killerPlayer.ExtraData.SetInt("NumKills", killerPlayer.ExtraData.GetInt("NumKills") + 1);
                    }
                }
            };

            // display score in *Stats -> Players*
            SyncedServerData.Data.SetStringArray(PlayerStats.ColumnNamesKey, new string[] { "Kills", "Deaths" });
            SyncedServerData.Data.SetFloatArray(PlayerStats.ColumnWidthsKey, new float[] { 80, 80 });
            SyncedServerData.Data.SetStringArray(PlayerStats.ColumnDataKeysKey, new string[] { "NumKills", "NumDeaths" });

            // reset the score after each 20 min
            // also pause the game for 10 seconds


            // forbid all player requests



        }
    }

}
