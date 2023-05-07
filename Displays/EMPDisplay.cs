using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using UnityEngine;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using System.Collections.Generic;

namespace TeslaCoil.Displays
{
	public class EMPDisplay : ModDisplay
	{
		public override string BaseDisplay => Game.instance.model.GetTowerFromId(TowerType.SuperMonkey+"-050").GetDescendant<CreateEffectOnAbilityModel>().effectModel.assetId.GUID;
		public Dictionary<string, Color> psColor = new Dictionary<string, Color>()
		{
			{ "Glow", new Color(0.1f, 0.7f, 1f, 0.409f) },
			{ "Lightning", new Color(0.1f, 0.8f, 1f) },
			{ "Pulse", new Color(0.1f, 0.5f, 1f, 0.518f) },
			{ "PulseBig", new Color(0.1f, 0.5f, 1f, 0.518f) }
		};

		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
#if DEBUG
			node.PrintInfo();
#endif

			foreach(ParticleSystem ps in node.GetComponentsInChildren<ParticleSystem>())
			{
				ps.startSize *= 0.3f;
				if(psColor.ContainsKey(ps.gameObject.name)) ps.startColor = psColor[ps.gameObject.name];
			}
		}
	}

	// For Long Range Tacks
	public class EMPDisplay1 : EMPDisplay
	{
		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
			base.ModifyDisplayNode(node);

			foreach(ParticleSystem ps in node.GetComponentsInChildren<ParticleSystem>())
			{
				ps.startSize *= 7f / 6f;
			}
		}
	}

	// For Super Range Tacks
	public class EMPDisplay2 : EMPDisplay
	{
		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
			base.ModifyDisplayNode(node);

			foreach(ParticleSystem ps in node.GetComponentsInChildren<ParticleSystem>())
			{
				ps.startSize *= 4f / 3f;
			}
		}
	}

	// For Tier 5
	public class EMP2Display : EMPDisplay
	{
		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
			base.ModifyDisplayNode(node);

			foreach(ParticleSystem ps in node.GetComponentsInChildren<ParticleSystem>())
			{
				ps.startSize *= 1.4f;
			}
		}
	}

	// For Long Range Tacks
	public class EMP2Display1 : EMPDisplay1
	{
		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
			base.ModifyDisplayNode(node);

			foreach(ParticleSystem ps in node.GetComponentsInChildren<ParticleSystem>())
			{
				ps.startSize *= 1.4f;
			}
		}
	}

	// For Super Range Tacks
	public class EMP2Display2 : EMPDisplay2
	{
		public override void ModifyDisplayNode(UnityDisplayNode node)
		{
			base.ModifyDisplayNode(node);

			foreach(ParticleSystem ps in node.GetComponentsInChildren<ParticleSystem>())
			{
				ps.startSize *= 1.4f;
			}
		}
	}
}
