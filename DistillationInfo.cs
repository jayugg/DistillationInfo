using System.Reflection;
using HandbookDistillation.Patches;
using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

[assembly: 
    ModInfo(
        name: "DistillationInfo",
        modID: "distillationinfo",
        Side = "Client",
        Version = "1.0.0", Authors = new string[] { "jayugg" }, 
        Description = "Show distillation properties in handbook"
        )
]

namespace HandbookDistillation
{
    public class HandbookDistillationModSystem : ModSystem
    {
        private Harmony HarmonyInstance;

        public override void StartClientSide(ICoreClientAPI api)
        {
            HarmonyInstance = new Harmony(Mod.Info.ModID);
        
            HarmonyInstance.Patch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo)
                    .GetMethod(nameof(CollectibleBehaviorHandbookTextAndExtraInfo.GetHandbookInfo)),
                postfix: typeof(GetHandbookInfoPatch).GetMethod(
                    nameof(GetHandbookInfoPatch.Postfix)));
        
            HarmonyInstance.Patch(typeof(CollectibleBehaviorHandbookTextAndExtraInfo)
                    .GetMethod("addCreatedByInfo", BindingFlags.NonPublic | BindingFlags.Instance),
                postfix: typeof(AddCreatedByInfoPatch).GetMethod(
                    nameof(AddCreatedByInfoPatch.Postfix)));
        }
        
        public override void AssetsFinalize(ICoreAPI api)
        {
            base.AssetsFinalize(api);
            if (api.Side == EnumAppSide.Server) return;
            
            foreach (var collectible in api.World.Collectibles)
            {
                var attributes = collectible?.Attributes?.Token;
                if (attributes == null || attributes["handbook"] == null) continue;
                if (attributes["handbook"]["exclude"]?.ToObject<bool>() != true || !collectible.Code.PathStartsWith("spiritportion")) continue;
                Mod.Logger.Warning("Found handbook excluded collectible: {0}", collectible.Code);
                attributes["handbook"]["exclude"] = false;
                attributes["handbook"]["ignoreCreativeInvStacks"] = true;
                collectible.Attributes = new JsonObject(attributes);
            }
        }
        
        public override void Dispose()
        {
            HarmonyInstance?.UnpatchAll(Mod.Info.ModID);
        }
    }
}