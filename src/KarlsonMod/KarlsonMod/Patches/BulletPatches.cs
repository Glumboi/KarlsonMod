using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

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
