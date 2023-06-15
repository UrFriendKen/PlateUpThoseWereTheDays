using KitchenData;
using System.Collections.Generic;

namespace ThoseWereTheDays
{
    public static class SettingLayoutProfiles
    {
        private static Dictionary<int, List<int>> validLayoutsBySetting = new Dictionary<int, List<int>>();

        public static void Add(RestaurantSetting setting, LayoutProfile layoutProfile, bool noDuplicates = false)
        {
            if (!validLayoutsBySetting.ContainsKey(setting.ID))
                validLayoutsBySetting.Add(setting.ID, new List<int>());
            if (noDuplicates && validLayoutsBySetting[setting.ID].Contains(layoutProfile.ID))
                return;
            validLayoutsBySetting[setting.ID].Add(layoutProfile.ID);
        }
        public static void Add(RestaurantSetting setting, IEnumerable<LayoutProfile> layoutProfiles, bool noDuplicates = false)
        {
            if (!validLayoutsBySetting.ContainsKey(setting.ID))
            {
                validLayoutsBySetting.Add(setting.ID, new List<int>());
            }
            foreach (LayoutProfile layoutProfile in layoutProfiles)
            {
                Add(setting, layoutProfile, noDuplicates);
            }
        }

        public static void Remove(RestaurantSetting setting)
        {
            if (!validLayoutsBySetting.ContainsKey(setting.ID))
                return;
            validLayoutsBySetting.Remove(setting.ID);
        }

        internal static bool TryGetValidLayoutIDs(int settingID, out int[] validLayoutIDs)
        {
            validLayoutIDs = null;
            bool success = validLayoutsBySetting.TryGetValue(settingID, out List<int> layoutIDsList);
            if (success)
            {
                validLayoutIDs = layoutIDsList.ToArray();
            }
            return success;
        }
    }
}
