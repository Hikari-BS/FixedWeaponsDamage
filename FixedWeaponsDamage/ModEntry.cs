using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;

namespace FixedWeaponsDamage
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            SlingshotPatches.Initialize(Monitor, Helper);

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(Slingshot), nameof(Slingshot.PerformFire)),
                prefix: new HarmonyMethod(typeof(SlingshotPatches), nameof(SlingshotPatches.PerformFire_Prefix))
                );

            helper.Events.Content.AssetRequested += Content_AssetRequested;
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // this doesn't work for now
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<int, string>().Data;

                    foreach ((int itemID, string itemData) in data)
                    {
                        string[] fields = itemData.Split('/');
                        fields[2] = fields[3];
                        data[itemID] = string.Join('/', fields);
                    }
                }, AssetEditPriority.Late);
            }
        }
    }
}
