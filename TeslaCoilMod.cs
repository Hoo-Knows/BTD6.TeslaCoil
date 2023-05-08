using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using MelonLoader;
using PathsPlusPlus;
using TeslaCoil;
using UnityEngine;

[assembly: MelonInfo(typeof(TeslaCoilMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace TeslaCoil
{
	public class TeslaCoilMod : BloonsTD6Mod
    {
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            MelonLogger.Msg("TeslaCoil loaded!");
		}
	}

	[HarmonyPatch(typeof(Factory.__c__DisplayClass21_0), "_CreateAsync_b__0")]
	public class FactoryCreateAsync_Patch
	{
		// I referenced Onixiya's SC2Expansion mod for this code, used to change the texture of druid lightning
		[HarmonyPrefix]
		public static bool Prefix(ref Factory.__c__DisplayClass21_0 __instance, ref UnityDisplayNode prototype)
		{
			if(__instance.objectId.guidRef.Split('-')[0] == "TeslaCoilLightning")
			{
				GameObject go = new GameObject(__instance.objectId.guidRef);
				go.transform.position = new Vector3(-3000f, 0f, 0f);

				SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
				sr.sprite = ModContent.GetSprite<TeslaCoilMod>(__instance.objectId.guidRef.Split('-')[1]);
				// These are just values from the original lightning
				sr.sortingGroupID = 1048575;
				sr.sortingLayerID = -4022049;
				sr.renderingLayerMask = 4294967295;

				go.AddComponent<UnityDisplayNode>();
				prototype = go.GetComponent<UnityDisplayNode>();

				__instance.__4__this.active.Add(prototype);
				__instance.onComplete.Invoke(prototype);
				return false;
			}
			return true;
		}
	}

	public class TeslaCoilPath : PathPlusPlus
    {
        public override string Tower => TowerType.TackShooter;
        public override int UpgradeCount => 5;
    }
}