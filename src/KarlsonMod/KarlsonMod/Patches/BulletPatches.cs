using HarmonyLib;

namespace KarlsonMod.Patches
{
    [HarmonyPatch(typeof(Bullet))]
    internal class BulletPatches
    {
        public static bool explosiveBullets = false;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void Start_Prefix(Bullet __instance)
        {
            __instance.explosive = explosiveBullets;
        }
    }
}
