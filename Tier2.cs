using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Unity;
using Il2CppSystem.Linq;
using PathsPlusPlus;

namespace TeslaCoil
{
	public class AmpedUpTacks : UpgradePlusPlus<TeslaCoilPath>
    {
        public override int Cost => 500;
        public override int Tier => 2;
        public override string Icon => VanillaSprites.SpyPlaneUpgradeIcon;
		public override string Portrait => "AmpedUpTacks";
		public override string Description => "Higher current tacks can pop lead and camo bloons.";

        public override void ApplyUpgrade(TowerModel towerModel, int tier)
        {
            Log("Tesla Coil upgraded to tier 2");

            towerModel.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            towerModel.GetDescendants<DamageModel>().ForEach(model => model.immuneBloonProperties = BloonProperties.None);
            towerModel.GetDescendants<DamageOverTimeModel>().ForEach(model => model.immuneBloonProperties = BloonProperties.Purple);

            // Buff laser shock ticks
            foreach(AddBehaviorToBloonModel addBehavior in towerModel.GetDescendants<AddBehaviorToBloonModel>().ToArray())
            {
                addBehavior.GetBehavior<DamageOverTimeModel>().Interval = 0.45f;
            }

            if(IsHighestUpgrade(towerModel))
            {
                towerModel.display = towerModel.GetBehavior<DisplayModel>().display =
                    Game.instance.model.GetTowerFromId(TowerType.TackShooter + "-200").display;
            }
        }

        public void Log(object o) => ModHelper.Msg<TeslaCoilMod>(o);
    }
}