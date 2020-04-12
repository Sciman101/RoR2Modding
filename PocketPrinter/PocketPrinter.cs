using BepInEx;
using R2API.Utils;
using R2API;
using BepInEx.Configuration;

namespace PocketPrinter
{
    [BepInDependency("com.bepis.r2api")]
    [R2APISubmoduleDependency(nameof(ItemAPI),nameof(ItemDropAPI),nameof(ResourcesAPI))]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class PocketPrinter : BaseUnityPlugin
    {

        private const string ModVer = "1.1.0";
        private const string ModName = "PocketPrinter";
        public const string ModGuid = "info.sciman.PocketPrinter";


        // Config options
        public static ConfigEntry<bool> AllowCopyLunarItems { get; set; }
        public static ConfigEntry<bool> AllowCopyBossItems { get; set; }
        public static ConfigEntry<bool> AllowCopyRedItems { get; set; }
        public static ConfigEntry<bool> FilamentAsRedItem { get; set; }

        public void Awake()
        {
            SetupConfig();
            Assets.Init();
            Hooks.Init();
        }

        private void SetupConfig()
        {
            AllowCopyLunarItems = Config.Bind<bool>(
                    "AdaptiveFilament",
                    "AllowCopyLunarItems",
                    false,
                    "Allows adaptive filament to duplicate lunar items"
                    );
            AllowCopyBossItems = Config.Bind<bool>(
                    "AdaptiveFilament",
                    "AllowCopyBossItems",
                    false,
                    "Allows adaptive filament to duplicate boss items"
                    );
            AllowCopyRedItems = Config.Bind<bool>(
                    "AdaptiveFilament",
                    "AllowCopyRedItems",
                    true,
                    "Allows adaptive filament to duplicate red (Tier 3) items"
                    );
            FilamentAsRedItem = Config.Bind<bool>(
                    "AdaptiveFilament",
                    "FilamentAsRedItem",
                    false,
                    "If true, Adaptive Filament will drop as a red (Tier 3) item"
                    );
        }


    }
}
