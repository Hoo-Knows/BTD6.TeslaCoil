using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using UnityEngine;
using Il2CppAssets.Scripts.Unity;

namespace TeslaCoil.Displays
{
	public class Tier5Display : ModDisplay
	{
		public override string BaseDisplay => Game.instance.model.GetTowerFromId(TowerType.TackShooter + "-500").display.GUID;

		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
#if DEBUG
			node.PrintInfo();
#endif

			SetMeshTexture(node, "Mesh500");
			node.transform.Find("Flame").gameObject.SetActive(false);
		}
	}
}
