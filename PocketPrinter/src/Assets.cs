using RoR2;
using R2API;
using System.Reflection;
using UnityEngine;

namespace PocketPrinter
{
    internal static class Assets
    {

        internal static GameObject PocketPrinterPrefab;
        internal static ItemIndex PocketPrinterItemIndex;

        private const string ModPrefix = "@PocketPrinter:";
        private const string PrefabPath = ModPrefix + "Assets/Import/printer/printer.prefab";
        private const string IconPath = ModPrefix + "Assets/Import/printer_icon/printer_icon.png";
        private const string AltIconPath = ModPrefix + "Assets/Import/printer_icon/printer_icon_alt.png";

        internal static void Init()
        {

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PocketPrinter.printer"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider(ModPrefix.TrimEnd(':'), bundle);
                ResourcesAPI.AddProvider(provider);

                PocketPrinterPrefab = bundle.LoadAsset<GameObject>(PrefabPath);
            }

            bool isRedItem = PocketPrinter.FilamentAsRedItem.Value;

            // Register pocket printer as item
            var pocketPrinterItemDef = new ItemDef
            {
                name = "PocketPrinter",
                tier = isRedItem ? ItemTier.Tier3 : ItemTier.Tier2,
                pickupModelPath = PrefabPath,
                pickupIconPath = isRedItem ? AltIconPath : IconPath,
                nameToken = "Adaptive Filament",
                pickupToken = "Duplicates the next item you pick up.\nOnly usable once",
                descriptionToken = "<style=cIsUtility>Turns into a copy</style> of the next item you pick up before being destroyed.",
                loreToken = "",
                tags = new[]
                {
                    ItemTag.Utility,
                    ItemTag.AIBlacklist
                }
            };

            // For now, we just won't render it - this might change in the future
            var itemDisplayRules = new ItemDisplayRule[1];
            var pocketPrinter = new R2API.CustomItem(pocketPrinterItemDef, itemDisplayRules);

            PocketPrinterItemIndex = ItemAPI.Add(pocketPrinter);
        }

    }
}
