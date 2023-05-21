using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppSystem.Linq;
using PathsPlusPlus;
using System.Linq;
using TeslaCoil.Displays;

namespace TeslaCoil
{
	public class TeslaCoil : UpgradePlusPlus<TeslaCoilPath>
    {
        public override int Cost => 42500;
        public override int Tier => 5;
        public override string Icon => GetTextureGUID<TeslaCoilMod>("Tier5");
		public override string Portrait => "TeslaCoil";
		public override string Description => "High frequency current discharges enough electricity to stop any bloon in its tracks.";

        public override void ApplyUpgrade(TowerModel towerModel, int tier)
        {
            Log("Tesla Coil upgraded to tier 5");

            // Buff tower range
            towerModel.range *= 1.4f;

            // Buff EMP and make it affect MOABs
            AttackModel empAttack = towerModel.GetAttackModel("AttackModel_EMP");
            empAttack.range = towerModel.range;

            // Faster firing
            WeaponModel empWeapon = empAttack.weapons.First(w => w.name == "WeaponModel_EMP");
            empWeapon.Rate /= 1.25f;

            // Scale up EMP projectile
            ProjectileModel empProjectile = empWeapon.projectile;
            empProjectile.radius *= 1.4f;
            empProjectile.scale *= 1.4f;
            empProjectile.pierce = 1000f;
            empProjectile.GetBehavior<DamageModel>().damage *= 2f;

            // Modify slow to be longer and affect all MOABs
            SlowForBloonModel slowForBloonModel = empProjectile.GetBehavior<SlowForBloonModel>();
            slowForBloonModel.bloonId = "";
            slowForBloonModel.bloonTag = "";
            slowForBloonModel.bloonIds = new string[] { };
            slowForBloonModel.bloonTags = new string[] { };
            slowForBloonModel.Lifespan *= 2f;
            empProjectile.GetBehavior<SlowModifierForTagModel>().lifespanOverride *= 2f;

            // Scale up EMP visual
            WeaponModel empVisual = empAttack.weapons.First(w => w.name == "WeaponModel_EMPVisual");
            empVisual.Rate /= 1.25f;
            empVisual.projectile.GetBehavior<AgeModel>().Lifespan = 0.7f;
            if(towerModel.appliedUpgrades.Contains(UpgradeType.LongRangeTacks)) empVisual.projectile.ApplyDisplay<EMP2Display1>();
            else if(towerModel.appliedUpgrades.Contains(UpgradeType.SuperRangeTacks)) empVisual.projectile.ApplyDisplay<EMP2Display2>();
            else empVisual.projectile.ApplyDisplay<EMP2Display>();

            // Buff laser shock ticks
            foreach(AddBehaviorToBloonModel behavior in towerModel.GetDescendants<AddBehaviorToBloonModel>().ToArray())
            {
                behavior.lifespan = 5.05f;
                float damageBuff = 0f;
                if(towerModel.appliedUpgrades.Contains(UpgradeType.MoreTacks)) damageBuff += 2.5f;
                if(towerModel.appliedUpgrades.Contains(UpgradeType.EvenMoreTacks)) damageBuff += 2.5f;
                behavior.GetBehavior<DamageOverTimeModel>().damage += damageBuff;
            }

            // Buff lightning
            foreach(WeaponModel weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                if(weaponModel.name.Contains("WeaponModel_TeslaCoilLightning"))
                {
                    weaponModel.projectile.pierce += 2f;
                    weaponModel.projectile.GetBehavior<DamageModel>().damage += 2f;
                    weaponModel.Rate *= 0.75f;
                }
            }

            if(IsHighestUpgrade(towerModel))
            {
                towerModel.ApplyDisplay<Tier5Display>();
            }
        }

        public void Log(object o) => ModHelper.Msg<TeslaCoilMod>(o);
    }
}