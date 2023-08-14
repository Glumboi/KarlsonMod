using System.IO;
using System.Reflection;
using System;
using HarmonyLib;
using Audio;
using UnityEngine;
using EZCameraShake;
using System.Diagnostics;

namespace KarlsonMod.Patches
{
    // TODO Review this file and update to your own requirements, or remove it altogether if not required

    /// <summary>
    /// Sample Harmony Patch class. Suggestion is to use one file per patched class
    /// though you can include multiple patch classes in one file.
    /// Below is included as an example, and should be replaced by classes and methods
    /// for your mod.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovement))]
    internal class PlayerPatches : MonoBehaviour
    {
        public static float newJumpingForce = 550f;
        public static float newSlideSlowdown = 0.2f;
        public static float newMoveSpeed = 20f;
        public static bool godMode = false;

        private static Transform transformOfPlayer;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static void Update_Prefix(PlayerMovement __instance)
        {
            transformOfPlayer = __instance.transform;
        }

        [HarmonyPatch("Jump")]
        [HarmonyPrefix]
        public static void Jump_Prefix(PlayerMovement __instance)
        {
            Traverse.Create(__instance).Field("jumpForce").SetValue(newJumpingForce);
        }        
        
        [HarmonyPatch("CounterMovement")]
        [HarmonyPrefix]
        public static void CounterMovement_Prefix(PlayerMovement __instance)
        {
            Traverse.Create(__instance).Field("slideSlowdown").SetValue(newSlideSlowdown);
        }

        [HarmonyPatch("Movement")]
        [HarmonyPrefix]
        public static void Movement_Prefix(PlayerMovement __instance)
        {
            Traverse.Create(__instance).Field("walkSpeed").SetValue(newMoveSpeed);
        }
        
        [HarmonyPatch("IsDead")]
        [HarmonyPrefix]
        public static bool IsDead_Prefix(PlayerMovement __instance)
        {
            if(godMode) { return false; }

            return (bool)Traverse.Create(__instance).Field("dead").GetValue();
        }

        static GameObject[] GetObjectsWithTag(string tag)
        {
            return GameObject.FindGameObjectsWithTag("Gun");
        }

        public static void DragAllWeaponsToMe()
        {
            foreach (var obj in GetObjectsWithTag("Gun"))
            {
                obj.transform.position = transformOfPlayer.position;
            }
        }

        public static int EnemiesLeft()
        {
            return GetObjectsWithTag("Enemy").Length;
        }

        public static void DragItemToMe(string name)
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            for (var i = 0; i < allObjects.Length; i++)
            {
                var obj = allObjects[i];
                if (obj.name.Contains(name)) obj.transform.position = transformOfPlayer.position;
            }
        }
    }
}