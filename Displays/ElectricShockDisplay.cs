using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Bloons;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Utils;
using Il2CppNinjaKiwi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeslaCoil.Displays
{
    // Most of this is taken from https://github.com/doombubbles/ability-choice/blob/main/Displays/ElectricShock.cs
    public class ElectricShockDisplay : ModDisplay
    {
        private const string BaseOverlayType = "LaserShock";
        public static readonly string CustomOverlayType = "ElectricShock";
        private static SerializableDictionary<string, BloonOverlayScriptable> OverlayTypes => GameData.Instance.bloonOverlays.overlayTypes;
        public override string Name => base.Name + "-" + overlayClass;
        public override PrefabReference BaseDisplayReference => OverlayTypes[BaseOverlayType].assets[overlayClass];
        protected readonly BloonOverlayClass overlayClass;

        public ElectricShockDisplay() { }

        public ElectricShockDisplay(BloonOverlayClass overlayClass)
        {
            this.overlayClass = overlayClass;
        }

        public override IEnumerable<ModContent> Load() => Enum.GetValues(typeof(BloonOverlayClass))
            .Cast<BloonOverlayClass>()
            .Select(bc => new ElectricShockDisplay(bc));

        public override void Register()
        {
            base.Register();
#if DEBUG
            MelonLoader.MelonLogger.Msg("Registering ElectricShockDisplay with overlay class " + overlayClass);
#endif
            BloonOverlayScriptable electricShock;
            if(!OverlayTypes.ContainsKey(CustomOverlayType))
            {
                electricShock = OverlayTypes[CustomOverlayType] = ScriptableObject.CreateInstance<BloonOverlayScriptable>();
				electricShock.assets = new SerializableDictionary<BloonOverlayClass, PrefabReference>();
				electricShock.displayLayer = OverlayTypes[BaseOverlayType].displayLayer;
            }
            else
            {
                electricShock = OverlayTypes[CustomOverlayType];
            }
            electricShock.assets[overlayClass] = CreatePrefabReference(Id);
        }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
#if DEBUG
            node.PrintInfo();
#endif
            foreach(var renderer in node.genericRenderers)
            {
                if(renderer.Is<SpriteRenderer>(out var spriteRenderer))
                {
                    spriteRenderer.color = new Color(0.4f, 0.9f, 1f);
                }
                else if(renderer.Is<MeshRenderer>(out var meshRenderer))
                {
                    meshRenderer.SetMainTexture(GetTexture(CustomOverlayType));
                }
            }
        }
    }
}