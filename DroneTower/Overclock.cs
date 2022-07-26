using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Simulation.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Simulation.Towers.Projectiles;
using Assets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Assets.Scripts.Simulation.Towers.Weapons.Behaviors;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Assets.Scripts.Models.Towers.Behaviors;
using MelonLoader;
using Il2CppSystem.Text.RegularExpressions;
using Assets.Main.Scenes;
using static Assets.Scripts.Models.Towers.TowerType;
using Assets.Scripts.Utils;

namespace DroneTower
{
    public class Overclock
    {
        [HarmonyPatch(typeof(InGame), nameof(InGame.Update))]
        internal class InGame_Update
        {
            [HarmonyPostfix]
            internal static void Postfix(InGame __instance)
            {
                if (__instance.bridge == null) return;
                var inGame = __instance;
                // var overclock = Game.instance.model.GetTower(EngineerMonkey, 0, 4).GetAbility().GetBehavior<OverclockModel>();
                var transform = Game.instance.model.GetTower(Alchemist, 0, 4, 0).GetAbility().GetBehavior<IncreaseRangeModel>().Duplicate();

                transform.addative = 0f;
                transform.multiplier = 3f;

                foreach (var tts in inGame.bridge.GetAllTowers())
                {

                    if (tts.tower.GetMutatorById("OverRange") != null)
                    {
                        tts.tower.AddMutator(transform.Mutator, 60, true);
                    }
                    if (tts.tower.GetMutatorById("UltraRange") != null)
                    {
                        tts.tower.AddMutator(transform.Mutator, -1, false);
                    }
                }
            }
        }
    }
}