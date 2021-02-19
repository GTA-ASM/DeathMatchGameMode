using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SanAndreasUnity.Behaviours;
using SanAndreasUnity.Utilities;
using SanAndreasUnity.Net;
using SanAndreasUnity.Chat;

namespace DeathMatchGameMode
{

    public class DeathmatchGamemode
    {
        static DeathmatchGamemode()
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


            // don't add random weapon to ped


            // register help command which will display all possible commands


            // register command for selecting weapon (/w name)


            // when player is killed, we need to update score
            Ped.onDamaged += (ped, damageInfo, damageResult) =>
            {
                // ped is damaged, check if he died
                if (ped.Health <= 0)
                {
                    var deadPlayer = Player.GetOwningPlayer(ped);
                    if (deadPlayer != null)
                    {
                        // increase number of deaths for dying player
                        deadPlayer.SetExtraIntData("NumDeaths", deadPlayer.GetExtraIntData("NumDeaths") + 1);

                        // increase number of kills for killer
                        var killerPlayer = damageInfo.attackerPlayer as Player;
                        if (killerPlayer != null)
                            killerPlayer.SetExtraIntData("NumKills", killerPlayer.GetExtraIntData("NumKills") + 1);
                    }
                }
            };

            // display score in *Stats -> Players*


            // reset the score after each 20 min
            // also pause the game for 10 seconds


            // forbid all player requests



        }
    }

}
