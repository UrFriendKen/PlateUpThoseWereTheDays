using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThoseWereTheDays.Customs
{
    public class TurboRestaurantSettingCopy : CustomRestaurantSetting
    {
        public override string UniqueNameID => "turboRestaurantSettingCopy";

        public override WeatherMode WeatherMode => WeatherMode.None;

        public override List<IDecorationConfiguration> Decorators => new List<IDecorationConfiguration>()
        {
            new StreetDecorator.DecorationsConfiguration()
            {
                StreetPiece = (Appliance)GDOUtils.GetExistingGDO(ApplianceReferences.TurboStreetPiece),
                WallPiece = (Appliance)GDOUtils.GetExistingGDO(ApplianceReferences.TurboWallPiece),
                InternalWallPiece = (Appliance)GDOUtils.GetExistingGDO(ApplianceReferences.InternalWallPiece)
            },
            new TrainStationDecorator.DecorationsConfiguration()
            {
                FrontBorder = (Appliance)GDOUtils.GetExistingGDO(ApplianceReferences.TurboWallPieceHalf),
                BorderSpacing = 1
            }
        };

        public override UnlockPack UnlockPack => (CompositeUnlockPack)GDOUtils.GetExistingGDO(CompositeUnlockPackReferences.MarchPack);

        public override Unlock StartingUnlock => (UnlockCard)GDOUtils.GetExistingGDO(UnlockCardReferences.TurboMode);

        public override GameObject Prefab => ((RestaurantSetting)GDOUtils.GetExistingGDO(RestaurantSettingReferences.MarchSettingTurbo))?.Prefab;

        public override bool AlwaysLight => true;

        public override List<(Locale, BasicInfo)> InfoList => CopyInfo(RestaurantSettingReferences.MarchSettingTurbo);

        private static List<(Locale, BasicInfo)> CopyInfo(int restaurantSettingID)
        {
            GameDataObject gdo = GDOUtils.GetExistingGDO(restaurantSettingID);
            if (gdo == null || !(gdo is RestaurantSetting restaurantSetting))
                return new List<(Locale, BasicInfo)>();
            return restaurantSetting.Info.GetLocales().Select(locale => (locale, GetConvertedInfo(locale))).ToList();

            BasicInfo GetConvertedInfo(Locale locale)
            {
                BasicInfo infoToCopy = restaurantSetting.Info.Get(locale);
                BasicInfo info = new BasicInfo()
                {
                    Name = $"{infoToCopy.Name ?? "Turbo"} (Legacy)",
                    Description = infoToCopy.Description
                };
                return info;
            }
        }
    }
}
