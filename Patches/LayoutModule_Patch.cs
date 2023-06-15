using HarmonyLib;
using Kitchen.Layouts.Modules;

namespace ThoseWereTheDays.Patches
{
    [HarmonyPatch]
    static class LayoutModule_Patch
    {
        [HarmonyPatch(typeof(Module), nameof(Module.OnCreateConnection))]
        [HarmonyPrefix]
        static bool OnCreateConnection_Prefix()
        {
            return Main.BuiltOnce;
        }
    }
}
