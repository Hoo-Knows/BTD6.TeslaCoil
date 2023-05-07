using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using UnityEngine;
using Il2CppAssets.Scripts.Unity;

namespace TeslaCoil.Displays
{
	public class Tier4Display : ModDisplay
	{
		public override string BaseDisplay => Game.instance.model.GetTowerFromId(TowerType.TackShooter + "-400").display.GUID;

		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
#if DEBUG
			node.PrintInfo();
#endif

			SetMeshTexture(node, "Mesh300");
			node.transform.Find("Flame (1)").gameObject.SetActive(false);
		}
	}
}
