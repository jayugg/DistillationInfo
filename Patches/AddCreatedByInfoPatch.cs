using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace HandbookDistillation.Patches;

public static class AddCreatedByInfoPatch
{
    public static bool isRunning = false;
    public static void Postfix(
        bool __result,
        ICoreClientAPI capi,
        ItemStack[] allStacks,
        ActionConsumable<string> openDetailPageFor,
        ItemStack stack,
        List<RichTextComponentBase> components,
        float marginTop,
        ref bool haveText)
    {
        // Add smoking information after the original method logic
        __result = components.AddCreatedByDistillationInfo(stack, capi, allStacks, openDetailPageFor, haveText);
    }
}