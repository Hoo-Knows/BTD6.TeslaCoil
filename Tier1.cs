using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons.Behaviors;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Unity;
using Il2CppSystem.Linq;
using PathsPlusPlus;
using TeslaCoil.Displays;

namespace TeslaCoil
{
	public class ElectrifiedTacks : UpgradePlusPlus<TeslaCoilPath>
    {
        public override int Cost => 250;
        public override int Tier => 1;
        public override string Icon => VanillaSprites.LaserShockUpgradeIcon;
		public override string Portrait => "ElectrifiedTacks";
		public override string Description => "Electrified tacks inflict a shocking effect on bloons.";

        public override void ApplyUpgrade(TowerModel towerModel, int tier)
        {
            Log("Tesla Coil upgraded to tier 1");

            TowerModel dartling = Game.instance.model.GetTowerFromId(TowerType.DartlingGunner+"-200");

            AddBehaviorToBloonModel electricShock = dartling.GetDescendant<AddBehaviorToBloonModel>().Duplicate();
			electricShock.overlayType = ElectricShockDisplay.CustomOverlayType;
            electricShock.mutationId = ElectricShockDisplay.CustomOverlayType;
            electricShock.filters = null;
            electricShock.name = "TeslaCoil_ElectricShock";

            foreach(var weaponModel in towerModel.GetDescendants<WeaponModel>().ToArray())
            {
                weaponModel.projectile.AddBehavior(electricShock);
                weaponModel.projectile.collisionPasses = new[] { 0, 1 };
            }

            if(IsHighestUpgrade(towerModel))
            {
                towerModel.display = towerModel.GetBehavior<DisplayModel>().display =
                    Game.instance.model.GetTowerFromId(TowerType.TackShooter+"-100").display;
            }
        }

        public void Log(object o) => ModHelper.Msg<TeslaCoilMod>(o);
    }
}