using System;
using BepInEx;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using System.Reflection;
using BepInEx.Configuration;

namespace Sciman101
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("info.sciman.Overpolypd", "Overpolypd", "1.1.0")]
    public class Overpolypd : BaseUnityPlugin
    {

        public static ConfigEntry<bool> TurretsUseItems { get; set; }
        public static ConfigEntry<bool> TurretsDrainHealth { get; set; }
        public static ConfigEntry<bool> BlacklistEngineerItems { get; set; }
        //public static ConfigEntry<float> MaxTurretSpawnRadius { get; set; }

        public void Awake()
        {
            // Setup config entries
            TurretsUseItems = Config.Bind<bool>(
                    "SquidTurrets",
                    "TurretsUseItems",
                    true,
                    "If true, squid turrets will inherit their items from players"
                    );
            TurretsDrainHealth = Config.Bind<bool>(
                    "SquidTurrets",
                    "TurretsDrainHealth",
                    true,
                    "If true, squid turrets will passively lose health - this is their default behaviour in the base game"
                    );
            BlacklistEngineerItems = Config.Bind<bool>(
                    "SquidTurrets",
                    "BlacklistEngineerItems",
                    true,
                    "If true, squid turrets will have the same items blacklisted as Engineer turrets"
                    );

            /*MaxTurretSpawnRadius = Config.Bind<float>(
                    "SquidTurrets",
                    "MaxTurretSpawnRadius",
                    25,
                    "Maximum distance away turrets will spawn"
                    );*/


            // DEBUG CODE
            /*On.RoR2.GlobalEventManager.OnInteractionBegin += (orig, self, interactor, interactable, interactableObject) =>
             {

                 CharacterBody component = interactor.GetComponent<CharacterBody>();
                 if (component)
                 {
                     component.inventory.GiveItem(ItemIndex.Squid);
                     component.inventory.GiveItem(ItemIndex.Mushroom);
                 }

                 orig(self, interactor, interactable, interactableObject);
             };*/


            // Change the minimum and maximum radii at which turrets can spawn
            /*float maxRad = UnityEngine.Mathf.Max(5, MaxTurretSpawnRadius.Value);
            IL.RoR2.GlobalEventManager.OnInteractionBegin += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchDup(),
                    x => x.MatchLdcR4(5)
                    );
                c.Index+=4;
                c.Next.Operand = maxRad;
            };*/

           
            BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var ilHook = new ILHook(typeof(GlobalEventManager).GetNestedType("<>c__DisplayClass36_0", AllFlags).
            GetMethodCached("<OnInteractionBegin>b__2"), il =>
            {
                // Find section of IL we're interested in - we insert this before any of the usual item behaviour
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCallvirt<UnityEngine.GameObject>("GetComponent"),
                    x => x.MatchDup(),
                    x => x.MatchCallvirt<CharacterMaster>("get_inventory"),
                    x => x.MatchLdcI4((int)ItemIndex.HealthDecay),
                    x => x.MatchLdcI4(30)
                    );
                c.Index--;

                if (TurretsUseItems.Value)
                {
                    // Add behaviour to inherit player items
                    c.EmitDelegate<Action<SpawnCard.SpawnResult>>((sr) =>
                    {
                        CharacterMaster squid = sr.spawnedInstance.GetComponent<CharacterMaster>();
                        Inventory inv = sr.spawnRequest.summonerBodyObject.GetComponent<CharacterBody>().inventory;
                        UnityEngine.Debug.Log(inv.ToString());
                        if (squid)
                        {
                            squid.inventory.CopyItemsFrom(inv);

                            // Certain items are disallowed on turrets by the base game
                            if (BlacklistEngineerItems.Value)
                            {
                                squid.inventory.ResetItem(ItemIndex.WardOnLevel);
                                squid.inventory.ResetItem(ItemIndex.BeetleGland);
                                squid.inventory.ResetItem(ItemIndex.CrippleWardOnLevel);
                                squid.inventory.ResetItem(ItemIndex.TPHealingNova);
                            }
                        }
                        
                    });
                    c.Emit(OpCodes.Ldarg_1);
                }

                // Disable vanilla code to drain health over time
                if (!TurretsDrainHealth.Value)
                {
                    c.Index += 3;
                    c.RemoveRange(5);
                }
            });
        }
    }
}