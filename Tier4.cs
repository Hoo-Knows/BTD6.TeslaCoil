using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Effects;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Utils;
using Il2CppSystem.Linq;
using PathsPlusPlus;
using System.Linq;
using TeslaCoil.Displays;

namespace TeslaCoil
{
    public class EMP : UpgradePlusPlus<TeslaCoilPath>
    {
        public override int Cost => 4800;
        public override int Tier => 4;
        public override string Icon => GetTextureGUID<TeslaCoilMod>("Tier4");
		public override string Portrait => "EMP";
		public override string Description => "Periodically releases an EMP that stuns nearby bloons and small MOABs. More and Even More Tacks give additional damage over time.";

        public override void ApplyUpgrade(TowerModel towerModel, int tier)
        {
            Log("Tesla Coil upgraded to tier 4");

            TowerModel super = Game.instance.model.GetTowerFromId(TowerType.SuperMonkey+"-050");
            TowerModel ninja = Game.instance.model.GetTowerFromId(TowerType.NinjaMonkey+"-003");

            // Create EMP attack and make it attack normally
            AttackModel empAttack = super.GetDescendants<AttackModel>().ToArray().First(a => a.name == "AttackModel_TechTerror_").Duplicate();
            empAttack.range = towerModel.range;
            empAttack.fireWithoutTarget = false;
            empAttack.AddBehavior(new RandomTargetModel("RandomTargetModel_", true, false));
            empAttack.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            empAttack.name = "AttackModel_EMP";

            // Make EMP attack faster
            WeaponModel empWeapon = empAttack.weapons[0];
            float rate = 1.5f;
            if(towerModel.appliedUpgrades.Contains(UpgradeType.FasterShooting)) rate *= 0.9f;
            if(towerModel.appliedUpgrades.Contains(UpgradeType.EvenFasterShooting)) rate *= 0.9f;
            empWeapon.Rate = rate;
            empWeapon.fireWithoutTarget = false;
            empWeapon.projectile.GetBehavior<AgeModel>().Lifespan = 0.1f;
            empWeapon.RemoveBehavior<CritMultiplierModel>();
            empWeapon.name = "WeaponModel_EMP";

            // Add visual effect to EMP
            WeaponModel empVisual = empWeapon.Duplicate();
            empVisual.projectile.RemoveBehavior<DamageModel>();
            empVisual.projectile.RemoveBehavior<ProjectileFilterModel>();
            empVisual.projectile.RemoveBehavior<DistributeToChildrenBloonModifierModel>();
            empVisual.projectile.GetBehavior<AgeModel>().Lifespan = 0.7f;
            if(towerModel.appliedUpgrades.Contains(UpgradeType.LongRangeTacks)) empVisual.projectile.ApplyDisplay<EMPDisplay1>();
            else if(towerModel.appliedUpgrades.Contains(UpgradeType.SuperRangeTacks)) empVisual.projectile.ApplyDisplay<EMPDisplay2>();
            else empVisual.projectile.ApplyDisplay<EMPDisplay>();
            empVisual.name = "WeaponModel_EMPVisual";
            empAttack.AddWeapon(empVisual);

            // Shrink EMP projectile and nerf pierce and damage
            ProjectileModel empProjectile = empWeapon.projectile;
            empProjectile.radius = towerModel.range + 1f;
            empProjectile.scale = towerModel.range + 1f;
            empProjectile.pierce = 100f;
            empProjectile.GetBehavior<DamageModel>().damage = 1f;

            // Add stun effect to EMP projectile - halved for MOABs and BFBs
            empProjectile.AddBehavior(new SlowForBloonModel("SlowForBloonModel_", 0f, 0.4f, "Stun:Weak", 9999, "Stun", 
                true, false, "Zomg,Ddt,Bad,Boss", "Zomg,Ddt,Bad,Boss", true, null, false, false, false, 0));
            empProjectile.GetBehavior<SlowForBloonModel>().bloonIds = new string[] { "Zomg", "Ddt", "Bad", "Boss" };
            empProjectile.GetBehavior<SlowForBloonModel>().bloonTags = new string[] { "Zomg", "Ddt", "Bad", "Boss" };
            empProjectile.GetBehavior<SlowForBloonModel>().mutator = ninja.GetDescendant<SlowModel>().Mutator;
            empProjectile.AddBehavior(new SlowModifierForTagModel("SlowModifierForTagModel_", "Moabs", "Stun:Weak", 1f, false, false, 0.2f, false));

            // Add laser shock to EMP projectile
            AddBehaviorToBloonModel laserShock = towerModel.GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            empProjectile.AddBehavior(laserShock);
            empProjectile.collisionPasses = new[] { -1, 0, 1 };

            towerModel.AddBehavior(empAttack);

            // Buff laser shock ticks
            foreach(AddBehaviorToBloonModel addBehavior in towerModel.GetDescendants<AddBehaviorToBloonModel>().ToArray())
            {
                addBehavior.lifespan = 2.55f;
                addBehavior.GetBehavior<DamageOverTimeModel>().Interval = 0.25f;
                float damageBuff = 0f;
                if(towerModel.appliedUpgrades.Contains(UpgradeType.MoreTacks)) damageBuff += 2f;
                if(towerModel.appliedUpgrades.Contains(UpgradeType.EvenMoreTacks)) damageBuff += 2f;
                addBehavior.GetBehavior<DamageOverTimeModel>().damage += damageBuff;
            }

            // Buff lightning
            foreach(WeaponModel weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                if(weaponModel.name.Contains("WeaponModel_TeslaCoilLightning"))
				{
                    weaponModel.projectile.pierce += 2f;
                    weaponModel.projectile.GetDamageModel().damage++;
                    weaponModel.Rate *= 0.75f;
				}
            }

            if(IsHighestUpgrade(towerModel))
            {
                towerModel.ApplyDisplay<Tier4Display>();
            }
        }

        public void Log(object o) => ModHelper.Msg<TeslaCoilMod>(o);
    }
}