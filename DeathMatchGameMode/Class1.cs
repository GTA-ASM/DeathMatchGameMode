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
using SanAndreasUnity.Commands;
using SanAndreasUnity.Importing;
using SanAndreasUnity.Importing.Items.Definitions;
using SanAndreasUnity.Importing.Weapons;

namespace DeathMatchGameMode
{

    public class DeathmatchGamemode : PluginManager.PluginBase
    {
        Dictionary<Player, WeaponData> weaponsPerPlayer = new Dictionary<Player, WeaponData>();


        public DeathmatchGamemode()
        {
            // our plugin is loaded

            // send a message to the console to signal that everything is allright
            Debug.Log("Deathmatch gamemode loaded");

            // register our gamemode
            Register();

        }

        void Register()
        {
            // register gamemode at GameModeManager
            // if our gamemode is selected when starting the server, the callback function (Setup) will be called
            GameModeManager.Instance.RegisterGameMode(new GameModeManager.GameModeInfo("Deathmatch Area 69", "", Setup));
        }

        void Setup()
        {
            // send chat message to all new players saying that this gamemode is running
            Player.onStart += player =>
            {
                ChatManager.SendChatMessageToPlayer(player, "Welcome to the server ! This server is running deathmatch gamemode. Type '/w' to select a weapon.");
            };

            // assign spawn handler
            SpawnManager.Instance.SpawnHandler = new MySpawnHandler();

            // register command for selecting weapon (/w name)
            CommandManager.Singleton.RegisterCommand(new CommandManager.CommandInfo
            {
                command = "w",
                allowToRunWithoutServerPermissions = true,
                commandHandler = ProcessWeaponCommand,
            });

            // don't automatically add weapons to spawned players
            SpawnManager.Instance.addWeaponsToSpawnedPlayers = false;

            // when player's ped is spawned, give him a weapon he choosed
            Ped.onStart += ped =>
            {
                if (ped.PlayerOwner != null)
                {
                    GiveWeaponToPlayer(ped.PlayerOwner);
                }
            };

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

            // set initial score for new players
            Player.onStart += player =>
            {
                player.ExtraData.SetInt("NumKills", 0);
                player.ExtraData.SetInt("NumDeaths", 0);
            };

            // reset the score after each 20 min
            // also pause the game for 10 seconds


            // forbid all player requests



        }

        CommandManager.ProcessCommandResult ProcessWeaponCommand(CommandManager.ProcessCommandContext context)
        {
            var arguments = CommandManager.SplitCommandIntoArguments(context.command);

            if (arguments.Length != 2)
                return new CommandManager.ProcessCommandResult { response = "Invalid syntax. Example: /w ak47" };

            string weaponName = arguments[1];

            var weaponData = WeaponData.LoadedWeaponsData.LastOrDefault(wd => wd.weaponType.Equals(weaponName, StringComparison.InvariantCultureIgnoreCase));
            if (weaponData == null)
                return new CommandManager.ProcessCommandResult { response = "Weapon with that name does not exist" };

            int[] allowedSlots = new int[]
            {
                WeaponSlot.Pistol,
                WeaponSlot.Shotgun,
                WeaponSlot.Submachine,
                WeaponSlot.Machine,
                WeaponSlot.Rifle,
            };

            var allowedWeaponNames = WeaponData.LoadedWeaponsData
                .Where(wd => allowedSlots.Contains(wd.weaponslot))
                .Select(wd => wd.weaponType)
                .Distinct();

            if (!allowedSlots.Contains(weaponData.weaponslot))
                return new CommandManager.ProcessCommandResult { response = $"Only these weapons are allowed: {string.Join(", ", allowedWeaponNames)}" };

            if (context.player == null)
                return new CommandManager.ProcessCommandResult { response = "This command can only be ran for a player" };

            weaponsPerPlayer[context.player] = weaponData;

            GiveWeaponToPlayer(context.player);

            return new CommandManager.ProcessCommandResult { response = $"Changed weapon to {weaponName}" };
        }

        void GiveWeaponToPlayer(Player player)
        {
            var ped = player.OwnedPed;

            if (ped == null)
                return;

            // first remove all existing weapons
            ped.WeaponHolder.RemoveAllWeapons();

            if (!weaponsPerPlayer.TryGetValue(player, out WeaponData weaponData))
            {
                // player has not choosen a weapon yet, give him a pistol
                weaponData = WeaponData.LoadedWeaponsData.LastOrDefault(wd => wd.weaponType.Equals("pistol", StringComparison.InvariantCultureIgnoreCase));
            }

            var weapon = ped.WeaponHolder.SetWeaponAtSlot(weaponData.modelId1, weaponData.weaponslot);
            ped.WeaponHolder.SwitchWeapon(weaponData.weaponslot);
            weapon.AmmoInClip = weapon.AmmoClipSize;
            weapon.AmmoOutsideOfClip = 500;
        }
    }

}
