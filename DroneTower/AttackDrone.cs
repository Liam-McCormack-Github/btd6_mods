using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Utils;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using BTD_Mod_Helper.Api.Display;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppSystem.Collections.Generic;
using Assets.Scripts.Models.Bloons.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Assets.Scripts.Models.Towers.Filters;
using BTD_Mod_Helper.Api;
using HarmonyLib;
using Assets.Scripts.Unity.UI_New.InGame;

namespace AttackDrone
{
    public class AttackDrone: ModTower
    {
        public override string BaseTower => "HeliPilot-210";
        public override string Name => "AttackDrone";
        public override int Cost => 750;
        public override string DisplayName => "Attack Drone";
        public override string Description => "Fast and mobile Attack Drone, fires deadly darts. Now with in-built pursuit targeting option!";
        public override string Icon => "AttackDrone1";
        public override string Portrait => "AttackDrone1";
        public override string TowerSet => MILITARY;
        public override int TopPathUpgrades => 5;
        public override int MiddlePathUpgrades => 5;
        public override int BottomPathUpgrades => 5;

        // 2D shit
        public override bool Use2DModel => true;
        public override float PixelsPerUnit => 5f;
        public override string Get2DTexture(int[] tiers)
        {
            /*
            if (tiers[0] == 1)
            {
                return "AttackDrone-2D-100";
            }
            */
            return "AttackDrone-2D-000";
        }

        // BaseTowerModel
        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.GetBehavior<AirUnitModel>().display = "14ce26c16228430478f466edd4592da3";

            var heliMovementModel = towerModel.GetBehavior<AirUnitModel>().behaviors.First().Cast<HeliMovementModel>();
            heliMovementModel.maxSpeed = 90;
            heliMovementModel.rotationSpeed = 0.16f;
            heliMovementModel.movementForceStart = 600;
            heliMovementModel.movementForceEnd = 300;
            heliMovementModel.movementForceEndSquared = 90000;
            heliMovementModel.brakeForce = 400;
            heliMovementModel.otherHeliRepulsonForce = 15;

            towerModel.range = 22f;
            towerModel.radius = 6;
            towerModel.radiusSquared = 36.0f;

            towerModel.GetBehavior<RectangleFootprintModel>().xWidth = 20;
            towerModel.GetBehavior<RectangleFootprintModel>().yWidth = 20;

            var footPrint = towerModel.GetBehavior<RectangleFootprintModel>();
            towerModel.footprint = footPrint;

            var attackModel = towerModel.GetAttackModel();

            attackModel.RemoveWeapon(attackModel.weapons[1]); // remove quad darts
            attackModel.weapons[0].projectile.display = Game.instance.model.GetTowerFromId("Drone").GetAttackModel().weapons[0].projectile.display;
            attackModel.weapons[0].emission = Game.instance.model.GetTowerFromId("HeliPilot-400").GetAttackModel().weapons[2].emission;
            attackModel.weapons[0].ejectX = 0;
            attackModel.weapons[0].ejectY = 10;
            attackModel.weapons[0].ejectZ = 4;
            attackModel.weapons[0].projectile.pierce = 2;
            attackModel.weapons[0].projectile.GetDamageModel().damage = 1;
            attackModel.weapons[0].Rate = 0.7f;
            attackModel.weapons[0].rateFrames = 32;
            attackModel.range = 30;
        }

        // Upgrade TowerModel TOP
        public class IncreasePierce : ModUpgrade<AttackDrone>
        {
            public override string Name => "IncreasePierce";
            public override string DisplayName => "Increase Pierce";
            public override string Description => "Darts have increased pierce.";
            public override string Icon => "100";
            public override string Portrait => "AttackDrone1";
            public override int Cost => 600; // 1350 = 750 + this
            public override int Path => TOP;
            public override int Tier => 1;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "14ce26c16228430478f466edd4592da3";
                
                var attackModel = towerModel.GetAttackModel();

                attackModel.weapons[0].projectile.pierce += 1;
            }
        }
        public class HomingDarts : ModUpgrade<AttackDrone>
        {
            public override string Name => "HomingDarts";
            public override string DisplayName => "Homing Darts";
            public override string Description => "Sometimes the Navy and the Airforce share. Sometimes.";
            public override string Icon => "200";
            public override string Portrait => "AttackDrone2";
            public override int Cost => 400; // 1750 = 750 + 600 + this
            public override int Path => TOP;
            public override int Tier => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "14ce26c16228430478f466edd4592da3";
                
                var attackModel = towerModel.GetAttackModel();
                var behavior = Game.instance.model.GetTowerFromId("MonkeySub").GetDescendants<ProjectileModel>().ToList()[0].GetBehavior<TrackTargetModel>().Duplicate();
                
                attackModel.weapons[0].projectile.AddBehavior(behavior);

            }
        }
        public class LaserBlaster : ModUpgrade<AttackDrone>
        {
            public override string Name => "LaserBlaster";
            public override string DisplayName => "Laser Blaster";
            public override string Description => "Shoots powerful laser blasts instead of darts.";
            public override string Icon => "300";
            public override string Portrait => "AttackDrone3";
            public override int Cost => 1250; // 3000 = 750 + 600 + 400 + this
            public override int Path => TOP;
            public override int Tier => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "c625fdbd93b29ed4f82bd9e9162a6887";

                var attackModel = towerModel.GetAttackModel();
                var DartlingGunner300 = Game.instance.model.GetTowerFromId("DartlingGunner-300").GetAttackModel().weapons[0].projectile;

                attackModel.weapons[0].Rate /= 1.5f;
                attackModel.weapons[0].projectile.display = DartlingGunner300.display;
                attackModel.weapons[0].projectile.GetDamageModel().damage += 1;
                attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.Purple;
                attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespanFrames = DartlingGunner300.GetBehavior<TravelStraitModel>().lifespanFrames;
                attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().Speed = DartlingGunner300.GetBehavior<TravelStraitModel>().Speed;
                attackModel.weapons[0].projectile.ignoreBlockers = false;
            }
        }
        public class LaserDeFortify : ModUpgrade<AttackDrone>
        {
            public override string Name => "LaserDeFortify";
            public override string DisplayName => "Laser Targeting";
            public override string Description => "Advanced sensors allow for targeting of certain Bloons weak spots stripping Fortification off smaller Bloons.";
            public override string Icon => "400";
            public override string Portrait => "AttackDrone4";
            public override int Cost => 8600; // 6600 = 750 + 500 + 750 + 1000 + this
            public override int Path => TOP;
            public override int Tier => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "48778a9567419024685baff97efae912";

                var attackModel = towerModel.GetAttackModel();
                var ninjaAttackModel = Game.instance.model.GetTowerFromId("NinjaMonkey-020").GetAttackModel().weapons[0].Duplicate();
                var newBehaviorProjectile = ninjaAttackModel.projectile;
                var newBehaviorEmission = ninjaAttackModel.emission;
                var AlchemistRemoveBloonModifiersModel = Game.instance.model.GetTowerFromId("Alchemist-020").GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<RemoveBloonModifiersModel>().Duplicate();
                var ninjaRemoveBloonModifiersModel = ninjaAttackModel.projectile.GetBehavior<RemoveBloonModifiersModel>();
                var ninjaWindModel = ninjaAttackModel.projectile.GetBehavior<WindModel>();
                var behavior = ninjaRemoveBloonModifiersModel.Duplicate();

                newBehaviorProjectile.RemoveBehavior(ninjaRemoveBloonModifiersModel);
                newBehaviorProjectile.RemoveBehavior(ninjaWindModel);

                behavior.cleanseCamo = false;
                behavior.cleanseFortified = true;
                behavior.bloonTagExcludeList = AlchemistRemoveBloonModifiersModel.bloonTagExcludeList;

                newBehaviorProjectile.AddBehavior(behavior);
                newBehaviorProjectile.display = null;

                // newBehaviorProjectile.GetDamageModel().damage = 0;
                // newBehaviorProjectile.pierce = 1;

                var newBehavior = new CreateProjectileOnContactModel("CreateProjectileOnContactModel", newBehaviorProjectile, newBehaviorEmission, false, false, false);
                
                attackModel.weapons[0].projectile = Game.instance.model.GetTowerFromId("DartlingGunner-300").GetAttackModel().weapons[0].projectile;
                attackModel.weapons[0].projectile.pierce = 3;
                attackModel.weapons[0].projectile.ignoreBlockers = false;
                attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                attackModel.weapons[0].projectile.AddBehavior(newBehavior);

            }
        }
        public class LaserBeam : ModUpgrade<AttackDrone>
        {
            public override string Name => "LaserBeam";
            public override string DisplayName => "Dr Death's Death Ray";
            public override string Description => "New advancement in miniature reactors allow the Attack Drone to shoot a continuous beam of energy which annihilates Bloons.";
            public override string Icon => "500";
            public override string Portrait => "AttackDrone5";
            public override int Cost => 80000; // 91600 = 750 + 500 + 750 + 1000 + 8600 + this
            public override int Path => TOP;
            public override int Tier => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "ceb34e729d263d34ea24fc7811cf0ed8";

                var attackModel = towerModel.GetAttackModel();
                var AdoraAbility = Game.instance.model.GetTowerFromId("BallOfLightTower").GetAttackModel().Duplicate();
                var ballOfLight = AdoraAbility.weapons[0];
                var airBehavior = towerModel.GetAttackModels()[0].weapons[0].GetBehavior<FireFromAirUnitModel>();
                
                ballOfLight.AddBehavior(airBehavior);
                // ballOfLight.ejectX = ballOfLight.ejectY = ballOfLight.ejectZ = 0;
                ballOfLight.emission.Cast<LineProjectileEmissionModel>().dontUseTowerPosition = true;
               
                attackModel.weapons[0] = ballOfLight;
            }
        }

        // Upgrade TowerModel MIDDLE
        public class IncreaseRange : ModUpgrade<AttackDrone>
        {
            public override string Name => "IncreaseRange";
            public override string DisplayName => "Advanced Range Finder";
            public override string Description => "Attack Drone can identify Bloons from further away.";
            public override string Icon => "010";
            public override string Portrait => "AttackDrone1";
            public override int Cost => 400; // 1150 = 750 + this
            public override int Path => MIDDLE;
            public override int Tier => 1;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "e9bca490bee0dc146b024781836097ff";
                
                var attackModel = towerModel.GetAttackModel();

                attackModel.range *= 2.5f;
            }
        }
        public class CamoDetection : ModUpgrade<AttackDrone>
        {
            public override string Name => "CamoDetection";
            public override string DisplayName => "Drone Detection";
            public override string Description => "Allows Attack Drone to attack and shoot Camo Bloons.";
            public override string Icon => "020";
            public override string Portrait => "AttackDrone2";
            public override int Cost => 450; // 1600 = 750 + 400 + this
            public override int Path => MIDDLE;
            public override int Tier => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "e9bca490bee0dc146b024781836097ff";

                towerModel.AddBehavior(new OverrideCamoDetectionModel("OverrideCamoDetectionModel_", true));
            }
        }
        public class CryoCannon : ModUpgrade<AttackDrone>
        {
            public override string Name => "CryoCannon";
            public override string DisplayName => "Cryo Air Control System";
            public override string Description => "Fires a blast which freeze Bloons instead of darts.";
            public override string Icon => "030";
            public override string Portrait => "AttackDrone3";
            public override int Cost => 3250; // 4850 = 750 + 400 + 450 + this
            public override int Path => MIDDLE;
            public override int Tier => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "c625fdbd93b29ed4f82bd9e9162a6887";

                var attackModel = towerModel.GetAttackModel();

                attackModel.weapons[0].projectile = Game.instance.model.GetTowerFromId("IceMonkey-203").GetAttackModel().weapons[0].projectile;
                attackModel.weapons[0].projectile.display = Game.instance.model.GetTowerWithName("SentryCold").GetAttackModel().weapons[0].projectile.display;
                attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().Lifespan = 60f;
                attackModel.weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespanFrames = 1;

                if (towerModel.appliedUpgrades.Contains("AttackDroneHomingDarts"))
                {
                    var behavior = Game.instance.model.GetTowerFromId("MonkeySub").GetDescendants<ProjectileModel>().ToList()[0].GetBehavior<TrackTargetModel>().Duplicate();
                    
                    attackModel.weapons[0].projectile.AddBehavior(behavior);
                }
            }
        }
        public class OverRange : ModUpgrade<AttackDrone>
        {
            public override string Name => "OverRange";
            public override string DisplayName => "Over-Range";
            public override string Description => "Over-Range Ability: Target Monkey can see further for 60 seconds. AKA Monkey go to specsavers.";
            public override string Icon => "040";
            public override string Portrait => "AttackDrone4";
            public override int Cost => 6000; // 10850 = 750 + 400 + 3250 + this
            public override int Path => MIDDLE;
            public override int Tier => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "48778a9567419024685baff97efae912";
                
                var ability = Game.instance.model.GetTowerFromId("EngineerMonkey-040").GetAbility().Duplicate();

                ability.icon = GetSpriteReference(Icon);
                ability.displayName = "Over-Range";
                ability.description = "Over-Range Ability: Target Monkey can see further for 60 seconds. AKA Monkey go to specsavers.";
                ability.GetBehavior<OverclockModel>().rateModifier = 1.0f;
                ability.GetBehavior<OverclockModel>().Mutator.buffIndicator.buffName = "Over-Range";
                ability.GetBehavior<OverclockModel>().Mutator.buffIndicator.iconName = "040";
                ability.GetBehavior<OverclockModel>().Mutator.id = "OverRange"; 
                ability.GetBehavior<OverclockModel>().Mutator.saveId = "OverRange4";
                ability.GetBehavior<OverclockModel>().mutatorSaveId = "OverRange4"; 
                ability.GetBehavior<OverclockModel>().mutatorId = "OverRange";

                towerModel.AddBehavior(ability);
            }
        }
        public class OverRangeAndSeeking : ModUpgrade<AttackDrone>
        {
            public override string Name => "UltraRange";
            public override string DisplayName => "Ultra-Range";
            public override string Description => "Ultra-Range Ability: Target Monkey can see further forever. AKA Monkey has laser eye surgery.";
            public override string Icon => "050";
            public override string Portrait => "AttackDrone5";
            public override int Cost => 36000; // 46850 = 750 + 400 + 3250 + 6000 + this
            public override int Path => MIDDLE;
            public override int Tier => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "ceb34e729d263d34ea24fc7811cf0ed8";

                var ability = towerModel.GetAbility();

                ability.icon = GetSpriteReference(Icon);
                ability.displayName = "Ultra-Range";
                ability.description = "Ultra-Range Ability: Target Monkey can see further forever. AKA Monkey has laser eye surgery.";
                ability.GetBehavior<OverclockModel>().rateModifier = 1.0f;
                ability.GetBehavior<OverclockModel>().Mutator.buffIndicator.buffName = "Ultra-Range";
                ability.GetBehavior<OverclockModel>().Mutator.buffIndicator.iconName = "050";
                ability.GetBehavior<OverclockModel>().Mutator.id = "UltraRange";
                ability.GetBehavior<OverclockModel>().Mutator.saveId = "UltraRange4";
                ability.GetBehavior<OverclockModel>().mutatorSaveId = "UltraRange4";
                ability.GetBehavior<OverclockModel>().mutatorId = "UltraRange";
            }
        }

        // Upgrade TowerModel BOTTOM
        public class IncreaseMovementSpeed : ModUpgrade<AttackDrone>
        {
            public override string Name => "IncreaseMovementSpeed";
            public override string DisplayName => "Faster Fans";
            public override string Description => "Faster Fans allow for increase maneuverability.";
            public override string Icon => "001";
            public override string Portrait => "AttackDrone1";
            public override int Cost => 250; // 1000 = 750 + this
            public override int Path => BOTTOM;
            public override int Tier => 1;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "14ce26c16228430478f466edd4592da3";

                var heliMovementModel = towerModel.GetDescendant<HeliMovementModel>();

                heliMovementModel.maxSpeed *= 2;
                heliMovementModel.movementForceStart *= 3;
                heliMovementModel.movementForceEnd *= 3;
                heliMovementModel.movementForceEndSquared *= 3;
                heliMovementModel.brakeForce *= 3;
            }
        }
        public class IncreaseAttackSpeed : ModUpgrade<AttackDrone>
        {
            public override string Name => "IncreaseAttackSpeed";
            public override string DisplayName => "Accelerated Coils";
            public override string Description => "Accelerated Coils allow for faster loading of munitions.";
            public override string Icon => "002";
            public override string Portrait => "AttackDrone2";
            public override int Cost => 1750; // 2750 = 750 + 250 + this
            public override int Path => BOTTOM;
            public override int Tier => 2;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "e9bca490bee0dc146b024781836097ff";

                var attackModel = towerModel.GetAttackModel();

                attackModel.weapons[0].Rate /= 2f; // 0.70 / 2 = 0.35
                attackModel.weapons[0].rateFrames /= 2; // 32 / 2 = 16 
            }
        }
        public class AddRockets : ModUpgrade<AttackDrone>
        {
            public override string Name => "AddRockets";
            public override string DisplayName => "Rocket Pods";
            public override string Description => "\"Just add a few Rockets they said...\"";
            public override string Icon => "003";
            public override string Portrait => "AttackDrone3";
            public override int Cost => 2500; // 5250 = 750 + 250 + 1750 + this
            public override int Path => BOTTOM;
            public override int Tier => 3;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "c625fdbd93b29ed4f82bd9e9162a6887";

                var attackModel = towerModel.GetAttackModel();

                attackModel.AddWeapon(Game.instance.model.GetTowerFromId("UCAV").GetAttackModel().weapons[0].Duplicate());
                attackModel.weapons[1].emission.Cast<ArcEmissionModel>().count = 2;
                attackModel.weapons[1].projectile.pierce = 8f;
                attackModel.weapons[1].Rate = 3.0f;

                if (towerModel.appliedUpgrades.Contains("AttackDroneIncreasePierce"))
                {
                    attackModel.weapons[1].projectile.pierce *= 1.5f;
                }
            }
        }
        public class RapidIncreaseAttackSpeed : ModUpgrade<AttackDrone>
        {
            public override string Name => "RapidIncreaseAttackSpeed";
            public override string DisplayName => "Overclocked Coils";
            public override string Description => "Overclocked Coils allow for the fastest loading of munitions.";
            public override string Icon => "004";
            public override string Portrait => "AttackDrone4";
            public override int Cost => 7500; // 7750 = 750 + 250 + 1750 + 2500 + this
            public override int Path => BOTTOM;
            public override int Tier => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "48778a9567419024685baff97efae912";

                var attackModel = towerModel.GetAttackModel();

                attackModel.weapons[0].Rate /= 2f; // 0.35 / 2 = 0.175
                attackModel.weapons[0].rateFrames /= 2; // 16 / 2 = 8
                attackModel.weapons[1].emission.Cast<ArcEmissionModel>().count = 6;
                attackModel.weapons[1].projectile.pierce *= 2f;
                attackModel.weapons[1].Rate = 2.0f;

            }
        }
        public class IncreaseAttackSpeedReallyFast : ModUpgrade<AttackDrone>
        {
            public override string Name => "IncreaseAttackSpeedReallyFast";
            public override string DisplayName => "The Hyperthreader";
            public override string Description => "The greatest Monkey Engineers have a annual competition to create the best Attack Drone. Winner of competition 2022!";
            public override string Icon => "005";
            public override string Portrait => "AttackDrone5";
            public override int Cost => 30000; // 37750 = 750 + 250 + 1750 + 2500 + 7500 + this
            public override int Path => BOTTOM;
            public override int Tier => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<AirUnitModel>().display = "ceb34e729d263d34ea24fc7811cf0ed8";

                var heliMovementModel = towerModel.GetDescendant<HeliMovementModel>();
                var attackModel = towerModel.GetAttackModel();

                heliMovementModel.maxSpeed *= 1.5f;
                attackModel.weapons[0].Rate /= 3.5f; // 0.175 / 3.5 = 0.05
                attackModel.weapons[0].rateFrames /= 2; // 8 / 2 = 4
                attackModel.weapons[0].Rate /= 5.5f;
                attackModel.weapons[0].rateFrames /= 4;
                attackModel.weapons[1].emission.Cast<ArcEmissionModel>().count = 10;
                attackModel.weapons[1].projectile.pierce *= 2f;
                attackModel.weapons[1].Rate = 1.0f;
            }
        }
    }
}