using System.Collections.Generic;
using System.Linq;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.GameContent;
using static HandbookDistillation.Constants;

namespace HandbookDistillation;

public static class HandbookExtensions
{
  
  public static DistillationProps GetDistillationProps(this CollectibleObject collectible, ICoreAPI api)
  {
    var distillationProps = collectible.Attributes?["distillationProps"]?.AsObject<DistillationProps>(null, collectible.Code.Domain);
    return distillationProps != null && !distillationProps.DistilledStack.Resolve(api.World, "distillation") ? null : distillationProps;
  }
  
  public static bool AddCreatedByDistillationInfo(
      this List<RichTextComponentBase> components,
      ItemStack itemStack,
      ICoreClientAPI capi,
      ItemStack[] allStacks,
      ActionConsumable<string> openDetailPageFor,
      bool haveText)
    {
      //if (components.Any(c => c is RichTextComponent rtc && rtc.DisplayText.Contains(Lang.Get(LangKeys.Distillation))))
      //{
      //  return haveText;
      //}
      List<ItemStack> itemStackList1 = new List<ItemStack>();
      foreach (ItemStack allStack in allStacks)
      {
        ItemStack distilledStack = allStack.Collectible.GetDistillationProps(capi)?.DistilledStack.ResolvedItemstack;
        if (distilledStack != null && distilledStack.Collectible.Code.Equals(itemStack?.Collectible?.Code) && !itemStackList1.Any((System.Func<ItemStack, bool>) (s => s.Collectible.Code.Equals(allStack.Collectible.Code))))
          itemStackList1.Add(allStack.Clone());
      }
      if (itemStackList1.Count == 0) return haveText;
      ClearFloatTextComponent floatTextComponent1 = new ClearFloatTextComponent(capi, 7f);
      bool haveHeading =
        components.Any(c => c is RichTextComponent rtc && rtc.DisplayText.Contains(Lang.Get(LangKeys.CreatedBy) + "\n"));
      if (!haveHeading)
        AddHeadingComponent(components, capi, LangKeys.CreatedBy, ref haveText);
      components.Add(floatTextComponent1);
      AddSubHeadingComponent(components, capi, openDetailPageFor, LangKeys.Distillation, null);
      while (itemStackList1.Count > 0)
      {
        ItemStack itemstackgroup = itemStackList1[0];
        itemStackList1.RemoveAt(0);
        if (itemstackgroup != null)
        {
          int num2;
          SlideshowItemstackTextComponent itemstackTextComponent = new SlideshowItemstackTextComponent(capi, itemstackgroup, itemStackList1, 40.0, EnumFloat.Inline, cs => num2 = openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)) ? 1 : 0);
          itemstackTextComponent.ShowStackSize = false;
          itemstackTextComponent.PaddingLeft = 2;
          components.Add(itemstackTextComponent);
        }
      }
      components.Add(new RichTextComponent(capi, "\n", CairoFont.WhiteSmallText()));
      return true;
    }
    
    public static bool AddDistillsIntoInfo(
      this List<RichTextComponentBase> components,
      ItemStack itemStack,
      ICoreClientAPI capi,
      ItemStack[] allStacks,
      ActionConsumable<string> openDetailPageFor,
      bool haveText)
    {
      if (components.Any(c => c is RichTextComponent rtc && rtc.Font.FontWeight == FontWeight.Bold && rtc.DisplayText.Contains(Lang.Get(LangKeys.DistillsInto))))
      {
        return haveText;
      }

      var distillationProps = itemStack.Collectible.GetDistillationProps(capi);
      ItemStack distilledStack = distillationProps?.DistilledStack.ResolvedItemstack;
      if (distilledStack == null) return haveText;
      ClearFloatTextComponent floatTextComponent1 = new ClearFloatTextComponent(capi, 7f);
      if (distillationProps.Ratio != 0)
      {
        distilledStack.StackSize = (int)(100 * itemStack.StackSize * distillationProps.Ratio);
      }
      AddHeadingComponent(components, capi, LangKeys.DistillsInto, ref haveText);
      components.Add(floatTextComponent1);
      ItemstackTextComponent itemstackTextComponent = new ItemstackTextComponent(capi, distilledStack, 40, 10, EnumFloat.Inline, cs => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(cs)));
      itemstackTextComponent.PaddingLeft = 2;
      itemstackTextComponent.ShowStacksize = distillationProps.Ratio != 0;
      components.Add(itemstackTextComponent);
      components.Add(new RichTextComponent(capi, "\n", CairoFont.WhiteSmallText()));
      return true;
    }
    
    public static void AddHeadingComponent(
      List<RichTextComponentBase> components,
      ICoreClientAPI capi,
      string heading,
      ref bool haveText)
    {
      if (haveText)
        components.Add(new ClearFloatTextComponent(capi, 14f));
      haveText = true;
      RichTextComponent richTextComponent = new RichTextComponent(capi, Lang.Get(heading) + "\n", CairoFont.WhiteSmallText().WithWeight(FontWeight.Bold));
      components.Add(richTextComponent);
    }
    
    public static void AddSubHeadingComponent(
      List<RichTextComponentBase> components,
      ICoreClientAPI capi,
      ActionConsumable<string> openDetailPageFor,
      string subheading,
      string detailpage)
    {
      if (detailpage == null)
      {
        RichTextComponent richTextComponent = new RichTextComponent(capi, "• " + Lang.Get(subheading) + "\n", CairoFont.WhiteSmallText());
        richTextComponent.PaddingLeft = 2.0;
        components.Add(richTextComponent);
      }
      else
      {
        RichTextComponent richTextComponent = new RichTextComponent(capi, "• ", CairoFont.WhiteSmallText());
        richTextComponent.PaddingLeft = 2.0;
        components.Add(richTextComponent);
        int num;
        components.Add(new LinkTextComponent(capi, Lang.Get(subheading) + "\n", CairoFont.WhiteSmallText(), _ => num = openDetailPageFor(detailpage) ? 1 : 0));
      }
    }
}