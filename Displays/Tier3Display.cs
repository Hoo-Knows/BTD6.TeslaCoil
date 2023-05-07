using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using UnityEngine;
using Il2CppAssets.Scripts.Unity;

namespace TeslaCoil.Displays
{
	public class Tier3Display : ModDisplay
	{
		public override string BaseDisplay => Game.instance.model.GetTowerFromId(TowerType.TackShooter+"-300").display.GUID;

		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
			SetMeshTexture(node, "Mesh300");
		}
	}
}
