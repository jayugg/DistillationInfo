using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace HandbookDistillation.Patches;

public static class GetHandbookInfoPatch
{
    public static void Postfix(
        ref RichTextComponentBase[] __result,
        ItemSlot inSlot,
        ICoreClientAPI capi,
        ItemStack[] allStacks,
        ActionConsumable<string> openDetailPageFor)
    {
        List<RichTextComponentBase> list = ((IEnumerable<RichTextComponentBase>) __result).ToList<RichTextComponentBase>();
        list.AddDistillsIntoInfo(inSlot?.Itemstack, capi, allStacks, openDetailPageFor, true);
        __result = list.ToArray();
    }
}