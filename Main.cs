using KitchenData;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenMods;
using System.Collections.Generic;
using System.Reflection;
using ThoseWereTheDays.Customs;
using UnityEngine;
using CustomSettingsAndLayouts;
using PreferenceSystem.Utils;
using Kitchen.Layouts;
using XNode;
using System.Linq;
using LogoMaker.Helpers;
using static Unity.Entities.FastEquality;

// Namespace should have "Kitchen" in the beginning
namespace ThoseWereTheDays
{
    public class Main : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.ThoseWereTheDays";
        public const string MOD_NAME = "Those Were The Days";
        public const string MOD_VERSION = "0.2.5";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.6";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        internal static Dictionary<int, int> CustomSettingLayouts => new Dictionary<int, int>()
        {
            { RestaurantSettingReferences.JanuarySetting, LayoutProfileReferences.JanuaryLayoutProfile },
            { RestaurantSettingReferences.FebruarySetting, LayoutProfileReferences.FebruaryLayout },//_februaryLayoutProfileCopy?.GameDataObject?.ID ?? 0 },
            { _turboRestaurantSettingCopy?.ID ?? 0, LayoutProfileReferences.TurboDinerLayout },
            { 1970109064, -1203731809 }, // Coffee Shop
            { RestaurantSettingReferences.SantaWorkshopSetting, LayoutProfileReferences.LayoutProfile },//_northPoleLayoutProfileCopy?.GameDataObject?.ID ?? 0 }//
        };

        internal static Dictionary<int, HashSet<int>> SettingSpecialDecoration => new Dictionary<int, HashSet<int>>()
        {
            { RestaurantSettingReferences.FebruarySetting, new HashSet<int>() { ApplianceReferences.FlowerArch, ApplianceReferences.HeartBalloons, ApplianceReferences.LoveSign } },
            { RestaurantSettingReferences.SantaWorkshopSetting, new HashSet<int>() { ApplianceReferences.ChristmasBanner, ApplianceReferences.ChristmasTree } },
            { RestaurantSettingReferences.Halloween, new HashSet<int>() { ApplianceReferences.Pumpkin, ApplianceReferences.Cobwebs, ApplianceReferences.GhostStatue } }
        };

        //internal static Dictionary<int, HashSet<int>> SettingDisabledAppliances => new Dictionary<int, HashSet<int>>()
        //{
        //    { RestaurantSettingReferences.FebruarySetting, new HashSet<int>() { ApplianceReferences.Hob, ApplianceReferences.PlateStack, ApplianceReferences.BlueprintUpgradeDesk, ApplianceReferences.TableLarge } }
        //};

        internal static HashSet<int> AdditionalSettingsNoDistinctLayout = new HashSet<int>()
        {
            RestaurantSettingReferences.Halloween
        };

        static HalloweenCardsModularUnlockPack _halloweenCardsModularUnlockPack;
        //static NorthPoleLayoutProfileCopy _northPoleLayoutProfileCopy;
        static TurboRestaurantSettingCopy _turboRestaurantSettingCopy;
        //static FebruaryLayoutProfileCopy _februaryLayoutProfileCopy;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            _halloweenCardsModularUnlockPack = AddGameDataObject<HalloweenCardsModularUnlockPack>();
            //_northPoleLayoutProfileCopy = AddGameDataObject<NorthPoleLayoutProfileCopy>();
            _turboRestaurantSettingCopy = AddGameDataObject<TurboRestaurantSettingCopy>();
            //_februaryLayoutProfileCopy = AddGameDataObject<FebruaryLayoutProfileCopy>();

            LogInfo("Done loading game data.");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            // LogInfo("Attempting to load asset bundle...");
            // Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            // LogInfo("Done loading asset bundle.");

            // Register custom GDOs
            AddGameData();

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                foreach (KeyValuePair<int, int> settingLayout in CustomSettingLayouts)
                {
                    if (args.gamedata.TryGet(settingLayout.Value, out LayoutProfile layout, warn_if_fail: true))
                    {
                        if (args.gamedata.TryGet(settingLayout.Key, out RestaurantSetting setting, warn_if_fail: true))
                        {
                            Registry.GrantCustomSetting(setting);
                            Registry.AddSettingLayout(setting, layout, noDuplicates: true);
                        }
                    }
                }

                foreach (KeyValuePair<int, HashSet<int>> settingSpecialDecorations in SettingSpecialDecoration)
                {
                    if (args.gamedata.TryGet(settingSpecialDecorations.Key, out RestaurantSetting setting, warn_if_fail: true))
                    {
                        foreach (int applianceID in settingSpecialDecorations.Value)
                        {
                            if (args.gamedata.TryGet(applianceID, out Appliance appliance, warn_if_fail: true))
                            {
                                appliance.ShoppingTags |= ShoppingTags.SpecialEvent;
                                appliance.IsPurchasable = true;
                                Registry.AddSettingDecoration(setting, appliance);
                            }
                        }
                    }
                }

                //foreach (KeyValuePair<int, HashSet<int>> settingDisableAppliances in SettingDisabledAppliances)
                //{
                //    if (args.gamedata.TryGet(settingDisableAppliances.Key, out RestaurantSetting setting, warn_if_fail: true))
                //    {
                //        foreach (int applianceID in settingDisableAppliances.Value)
                //        {
                //            if (args.gamedata.TryGet(applianceID, out Appliance appliance, warn_if_fail: true))
                //            {
                //                Registry.AddSettingDisabledAppliance(setting, appliance);
                //            }
                //        }
                //    }
                //}

                int halloweenSettingID = 82131534; // To be replaced with KitchenLib reference once RestaurantSettingReferences is added.
                if (args.gamedata.TryGet(CompositeUnlockPackReferences.HalloweenPack, out CompositeUnlockPack halloweenCompositePack, warn_if_fail: true) &&
                    args.gamedata.TryGet(ModularUnlockPackReferences.FranchiseCardsPack, out ModularUnlockPack franchiseModularPack, warn_if_fail: true) &&
                    args.gamedata.TryGet(ModularUnlockPackReferences.NormalCardsPack, out ModularUnlockPack normalModularPack, warn_if_fail: true) &&
                    args.gamedata.TryGet(halloweenSettingID, out RestaurantSetting halloweenSetting, warn_if_fail: true) && _halloweenCardsModularUnlockPack?.GameDataObject != null &&
                    halloweenCompositePack.Packs.Count > 0 && halloweenCompositePack.Packs[0] == null)
                {
                    bool replacedNormalCards = false;
                    for (int i = 0; i < halloweenCompositePack.Packs.Count; i++)
                    {
                        if (halloweenCompositePack.Packs[i]?.name == normalModularPack.name)
                        {
                            replacedNormalCards = true;
                            halloweenCompositePack.Packs[i] = _halloweenCardsModularUnlockPack?.GameDataObject;
                        }
                    }
                    if (replacedNormalCards)
                    {
                        halloweenCompositePack.Packs[0] = franchiseModularPack;
                        halloweenSetting.UnlockPack = halloweenCompositePack;
                        Registry.GrantCustomSetting(halloweenSetting);
                    }
                    else
                    {
                        Main.LogError("Failed to replace normal cards");
                    }
                }

                foreach (int settingID in AdditionalSettingsNoDistinctLayout)
                {
                    if (args.gamedata.TryGet(settingID, out RestaurantSetting setting, warn_if_fail: true))
                    {
                        Registry.GrantCustomSetting(setting);
                    }
                }

                // Restore Turbo allow base mains
                if (args.gamedata.TryGet(ModularUnlockPackReferences.MarchCards, out ModularUnlockPack marchCards, warn_if_fail: true))
                {
                    bool changedAllowBaseDishes = false;
                    foreach (IUnlockFilter filter in marchCards.Filter)
                    {
                        if (filter is FilterBasic filterBasic)
                        {
                            filterBasic.AllowBaseDishes = true;
                            Main.LogWarning("Successfully modified March Cards to allow base dishes");
                            changedAllowBaseDishes = true;
                            break;
                        }
                    }
                    if (!changedAllowBaseDishes)
                    {
                        Main.LogWarning("Failed to modify March Cards to allow base dishes");
                    }
                }


                // Transfer BlockAllDishes and AllowedDishes
                if (args.gamedata.TryGet(16318784, out Dish coffeeBaseDessert, warn_if_fail: true) &&
                    args.gamedata.TryGet(746549422, out UnlockCard coffeeShopMode, warn_if_fail: true))
                {
                    coffeeBaseDessert.BlocksAllOtherFood = false;
                    coffeeShopMode.BlocksAllOtherFood = true;

                    if (!coffeeBaseDessert.AllowedFoods.IsNullOrEmpty())
                    {
                        foreach (Unlock allowedFood in coffeeBaseDessert.AllowedFoods)
                        {
                            if (!coffeeShopMode.AllowedFoods.Contains(allowedFood))
                                coffeeShopMode.AllowedFoods.Add(allowedFood);
                        }
                        coffeeBaseDessert.AllowedFoods.Clear();
                    }
                }


                HashSet<int> layoutIDs = new HashSet<int>()
                {
                    LayoutProfileReferences.LayoutProfile,
                    LayoutProfileReferences.FebruaryLayout,
                    LayoutProfileReferences.JanuaryLayoutProfile,
                    LayoutProfileReferences.TurboDinerLayout,
                    LayoutProfileReferences.BasicLayout,
                    LayoutProfileReferences.DinerLayout,
                    LayoutProfileReferences.ExtendedLayout,
                    LayoutProfileReferences.HugeLayout,
                    LayoutProfileReferences.MediumLayout
                };

                // Log Layout Profile Connections

                foreach (int layoutID in layoutIDs)
                {
                    LogLayoutGraphConnections(args.gamedata, layoutID);
                }
            };
        }

        public static void LogLayoutGraphConnections(GameData gameData, int layoutID)
        {
            FieldInfo f_ports = typeof(Node).GetField("ports", BindingFlags.NonPublic | BindingFlags.Instance);
            if (gameData.TryGet(layoutID, out LayoutProfile layout))
            {
                Main.LogInfo($"{(layout.Name.IsNullOrEmpty() ? layout.GetType().FullName : layout.name)} Graph");
                LayoutGraph graph = layout.Graph;
                List<Node> nodes = graph.nodes;
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i] == null)
                    {
                        Main.LogInfo("null");
                        continue;
                    }

                    object obj = f_ports.GetValue(nodes[i]);
                    if (obj == null || !(obj is Dictionary<string, NodePort> nodeDictionary))
                    {
                        Main.LogError($"Failed to get Node Dictionary from {nodes[i].GetType()}");
                        continue;
                    }
                    if (!nodeDictionary.TryGetValue("Output", out NodePort outputPort))
                    {
                        Main.LogError($"Failed to get Output Node from {nodes[i].GetType()}");
                        continue;
                    }
                    if (!nodeDictionary.TryGetValue("Input", out NodePort inputPort))
                    {
                        Main.LogError($"Failed to get Input Node from {nodes[i].GetType()}");
                        continue;
                    }

                    Main.LogInfo($"{(inputPort.GetConnections().Any() ? $"{$"{string.Join(", ", inputPort.GetConnections().Select(x => $"{x.node.GetType()}|{x.fieldName}"))}"}-" : "")}" +
                        $"{nodes[i].GetType()}" + $"{(outputPort.GetConnections().Any() ? $"-{string.Join(", ", outputPort.GetConnections().Select(x => $"{x.node.GetType()}|{x.fieldName}"))}" : "")}");
                }
            }
        }


        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
