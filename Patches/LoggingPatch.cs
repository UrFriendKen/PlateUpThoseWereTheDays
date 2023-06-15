using HarmonyLib;
using Kitchen.Layouts;

namespace Steamtown.Patches
{
    [HarmonyPatch]
    static class LoggingPatch
    {
        [HarmonyPatch(typeof(LayoutGraph), "Build")]
        [HarmonyPrefix]
        static void MethodPrefix(ref LayoutBlueprint __result)
        {
            //Main.LogWarning(__result == null ? "null" : __result.ToString());
        }
    }
}
