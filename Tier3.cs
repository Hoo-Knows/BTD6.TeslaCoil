using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Utils;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Linq;
using PathsPlusPlus;
using System.Linq;
using TeslaCoil.Displays;

namespace TeslaCoil
{
    public class HighVoltage : UpgradePlusPlus<TeslaCoilPath>
    {
        public override int Cost => 1100;
        public override int Tier => 3;
        public override string Icon => GetTextureGUID<TeslaCoilMod>("Tier3");
		public override string Portrait => "HighVoltage";
		public override string Description => "Discharges electricity in a small area. Super Range Tacks give more splitting.";

        public override void ApplyUpgrade(TowerModel towerModel, int tier)
        {
            Log("Tesla Coil upgraded to tier 3");

            TowerModel druid = Game.instance.model.GetTowerFromId(TowerType.Druid+"-200");

            // Create lightning weapon and increase attack speed
            WeaponModel lightningWeapon = druid.GetAttackModel().weapons.First(w => w.name == "WeaponModel_Lightning").Duplicate();
            lightningWeapon.Rate = towerModel.GetAttackModel().weapons.First(w => w.name == "WeaponModel_Weapon").Rate * 0.75f;
            lightningWeapon.animation = 1;
            lightningWeapon.name = "WeaponModel_TeslaCoilLightning";

            // Edit lightning projectile to have less splitting, pierce, and damage
            ProjectileModel lightningProjectile = lightningWeapon.projectile;
            lightningProjectile.pierce = towerModel.GetAttackModel().weapons.First(w => w.name == "WeaponModel_Weapon").projectile.pierce + 2f;
            lightningProjectile.GetBehavior<LightningModel>().splitRange = towerModel.range / 2.5f;
            int splits = 1;
            if(towerModel.appliedUpgrades.Contains(UpgradeType.SuperRangeTacks)) splits++;
            lightningProjectile.GetBehavior<LightningModel>().splits = splits;

            // Tag the lightning effect with a unique guid
            CreateLightningEffectModel lightningEffect = new CreateLightningEffectModel("CreateLightningEffect_", 0.3f,
				new PrefabReference[]
				{
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningSmall1" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningSmall2" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningSmall3" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningMedium1" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningMedium2" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningMedium3" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningLarge1" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningLarge2" },
					new PrefabReference() { guidRef = "TeslaCoilLightning-LightningLarge3" },
				},
				new float[]
				{
					17.962965f,
					17.962965f,
					17.962965f,
					50.0000076f,
					50.0000076f,
					50.0000076f,
					85.18519f,
					85.18519f,
					85.18519f
				});
			lightningProjectile.RemoveBehavior<CreateLightningEffectModel>();
			lightningProjectile.AddBehavior(lightningEffect);

			// Add laser shock to lightning
			AddBehaviorToBloonModel laserShock = towerModel.GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            lightningProjectile.AddBehavior(laserShock);
            lightningProjectile.collisionPasses = new[] { 0, 1 };

            // Add first lightning weapon
            towerModel.GetAttackModel().name = "AttackModel_TeslaCoilLightning";
            towerModel.GetAttackModel().SetWeapon(lightningWeapon, 0);
            towerModel.GetAttackModel().RemoveBehavior<TargetCloseModel>();
            towerModel.GetAttackModel().AddBehavior(new RandomTargetModel("RandomTargetModel", true, false));
            towerModel.GetAttackModel().GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);

            // Add remaining lightning weapons
            int count = 8;
            if(towerModel.appliedUpgrades.Contains(UpgradeType.MoreTacks)) count += 2;
            if(towerModel.appliedUpgrades.Contains(UpgradeType.EvenMoreTacks)) count += 2;

            for(int i = 1; i < count; i++)
            {
                towerModel.AddBehavior(towerModel.GetAttackModel("AttackModel_TeslaCoilLightning").Duplicate());
            }

            // Buff laser shock ticks
            foreach(AddBehaviorToBloonModel addBehavior in towerModel.GetDescendants<AddBehaviorToBloonModel>().ToArray())
            {
                addBehavior.lifespan = 2.05f;
                addBehavior.GetBehavior<DamageOverTimeModel>().Interval = 0.5f;
            }

            if(IsHighestUpgrade(towerModel))
            {
                towerModel.ApplyDisplay<Tier3Display>();
            }
        }

        public void Log(object o) => ModHelper.Msg<TeslaCoilMod>(o);
    }
}
