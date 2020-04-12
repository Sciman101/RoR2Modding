using RoR2;

namespace PocketPrinter
{
    public class Hooks
    {
        internal static void Init()
        {
            On.RoR2.Inventory.GiveItem += (orig, self, index, count) =>
            {
                // Assuming this isn't another printer or special case item...
                if (index != Assets.PocketPrinterItemIndex)
                {
                    // Get item tier and check if it's allowed
                    ItemTier tier = ItemCatalog.GetItemDef(index).tier;
                    bool success = true;
                    if (!PocketPrinter.AllowCopyBossItems.Value && tier == ItemTier.Boss) success = false;
                    if (!PocketPrinter.AllowCopyLunarItems.Value && tier == ItemTier.Lunar) success = false;
                    if (!PocketPrinter.AllowCopyRedItems.Value && tier == ItemTier.Tier3) success = false;

                    if (success)
                    {
                        // Get how many printers we already have
                        int printerCount = self.GetItemCount(Assets.PocketPrinterItemIndex);
                        if (printerCount > 0)
                        {
                            // Remove all printers and increase count
                            count += printerCount;
                            self.RemoveItem(Assets.PocketPrinterItemIndex, printerCount);
                        }
                    }
                }
                // Call base function
                orig(self, index, count);
            };
        }
    }
}
