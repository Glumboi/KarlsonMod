using System;
using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace KarlsonMod.Patches
{
    [HarmonyPatch(typeof(PlayerMovement))]
    internal class PlayerPatches : MonoBehaviour
    {
        public static float newJumpingForce = 550f;
        public static float newSlideSlowdown = 0.2f;
        public static float newMoveSpeed = 20f;
        public static bool noclip = false;
        public static bool godMode = false;
        private static Transform transformOfPlayer;
        private static PlayerMovement instance;
        private static int spawnedEnemiesCount = 0;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake_Postfix(PlayerMovement __instance)
        {
            instance = __instance;
            /* instance.rb.useGravity = false;
             instance.rb.isKinematic = true;*/
        }

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
            var readyToJump = (bool)Traverse.Create(__instance).Field("readyToJump").GetValue();
            var sprinting = (bool)Traverse.Create(__instance).Field("sprinting").GetValue();
            var jumping = (bool)Traverse.Create(__instance).Field("jumping").GetValue();
            var crouching = (bool)Traverse.Create(__instance).Field("crouching").GetValue();
            var wallRunning = (bool)Traverse.Create(__instance).Field("wallRunning").GetValue();
            var surfing = (bool)Traverse.Create(__instance).Field("surfing").GetValue();
            var runSpeed = (float)Traverse.Create(__instance).Field("runSpeed").GetValue();
            var walkSpeedField = Traverse.Create(__instance).Field("walkSpeed");
            var currentWalkSpeed = (float)walkSpeedField.GetValue();
            var thisY = (float)Traverse.Create(__instance).Field("x").GetValue();
            var thisX = (float)Traverse.Create(__instance).Field("y").GetValue();
            
            //Noclip things
            __instance.rb.useGravity = !noclip;
            __instance.rb.isKinematic = noclip;
            if (noclip)
            {
                // Calculate the movement direction based on input
                // For some reason using thisY in the x parameter and thisX in the Z parameter fixes some weird movement orientation glitching
                Vector3 moveDirection = new Vector3(thisY, 0f, thisX);
                moveDirection.Normalize(); // Ensure the direction is normalized

                // Convert the moveDirection to be relative to the player's orientation
                moveDirection = __instance.orientation.transform.TransformDirection(moveDirection);

                // Calculate the new position based on movement direction and speed
                Vector3 newPosition = __instance.transform.position + moveDirection * currentWalkSpeed * Time.deltaTime;

                // Handle height adjustment based on Space and Control keys
                if (Input.GetKey(KeyCode.Space))
                {
                    newPosition.y += currentWalkSpeed * Time.deltaTime; // Move up
                }
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    newPosition.y -= currentWalkSpeed * Time.deltaTime; // Move down
                }

                // Apply the new position to the Rigidbody
                __instance.rb.MovePosition(newPosition);
            }
            
            // Update walkSpeed if necessary
            if (currentWalkSpeed != newMoveSpeed)
            {
                walkSpeedField.SetValue(newMoveSpeed);
                currentWalkSpeed = newMoveSpeed;
            }

            if (__instance.IsDead())
                return;

            // Gravity
            __instance.rb.AddForce(Vector3.down * Time.deltaTime * 10f);

            Vector2 velRelativeToLook = __instance.FindVelRelativeToLook();
            float x = velRelativeToLook.x;
            float y = velRelativeToLook.y;

            // Call the methods using Reflection
            Traverse.Create(__instance).Method("FootSteps");
            Traverse.Create(__instance).Method("CounterMovement", thisX, thisY, velRelativeToLook);

            if (readyToJump && jumping)
            {
                // Jump using Reflection
                Traverse.Create(__instance).Method("Jump");
            }

            float currentNewMoveSpeed = currentWalkSpeed;

            if (sprinting)
            {
                currentNewMoveSpeed = runSpeed;
            }

            if (crouching && __instance.grounded && readyToJump)
            {
                __instance.rb.AddForce(Vector3.down * Time.deltaTime * 3000f);
            }
            else
            {
                // Adjustments for diagonal movement
                if (Mathf.Abs(thisX) > currentNewMoveSpeed)
                    thisX = Mathf.Sign(thisX) * currentNewMoveSpeed;
                if (Mathf.Abs(thisY) > currentNewMoveSpeed)
                    thisY = Mathf.Sign(thisY) * currentNewMoveSpeed;

                float num2 = 1f;
                float num3 = 1f;

                if (!__instance.grounded)
                {
                    num2 = 0.5f;
                    num3 = 0.5f;
                }

                if (__instance.grounded && crouching)
                {
                    num3 = 0.0f;
                }

                if (wallRunning)
                {
                    num3 = 0.3f;
                    num2 = 0.3f;
                }

                if (surfing)
                {
                    num2 = 0.7f;
                    num3 = 0.3f;
                }

                // Apply forces for movement
                __instance.rb.AddForce(__instance.orientation.transform.forward * thisY * currentNewMoveSpeed *
                                       Time.deltaTime * num2 * num3);
                __instance.rb.AddForce(__instance.orientation.transform.right * thisX * currentNewMoveSpeed *
                                       Time.deltaTime * num2);

                // Call SpeedLines method using Reflection
                Traverse.Create(__instance).Method("SpeedLines");
            }

            return;
        }

        [HarmonyPatch("KillPlayer")]
        [HarmonyPrefix]
        public static void KillPlayer_Prefix(PlayerMovement __instance)
        {
            if (godMode)
            {
                KarlsonModPlugin.Log.LogInfo("Avoiding death!");
                return;
            }
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

        public static void SpawnEnemy()
        {
            GameObject enemyObj = new GameObject($"Mod_spawned_Enemy ({spawnedEnemiesCount})");
            spawnedEnemiesCount++;
            Enemy enemy = enemyObj.AddComponent<Enemy>();
            enemy.SendMessage("GiveGun");
            enemyObj.transform.position = new Vector3(
                instance.playerCam.forward.x,
                instance.playerCam.forward.y,
                instance.playerCam.forward.z + 10);

            KarlsonModPlugin.Log.LogInfo($"Spawned a new enemy ({enemyObj.name}) with weapon: {enemy.currentGun.name}");
        }

        public static bool MoveGameObjectToLocation(GameObject obj,
            float destLocX,
            float destLocY,
            float destLocZ)
        {
            bool failed = true;
            if (obj)
            {
                obj.transform.position = new Vector3(destLocX, destLocY, destLocZ);
                failed = false;
            }

            return failed;
        }

        public static bool MoveGameObjectToPlayer(GameObject obj)
        {
            return MoveGameObjectToLocation(obj,
                transformOfPlayer.position.x,
                transformOfPlayer.position.y,
                transformOfPlayer.position.z);
        }

        public static GameObject GetGameObject(string name)
        {
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            KarlsonModPlugin.Log.LogInfo($"Found {allObjects.Length} items");
            for (var i = 0; i < allObjects.Length; i++)
            {
                var obj = allObjects[i];
                if (obj.name.ToLower().Equals(name.ToLower()))
                {
                    KarlsonModPlugin.Log.LogInfo($"Requested item {obj.name} was found!");
                    return obj;
                }
            }

            return null;
        }

        public static void DragItemToMe(string name)
        {
            if (!MoveGameObjectToPlayer(GetGameObject(name)))
            {
                KarlsonModPlugin.Log.LogWarning($"Item \"{name}\" not found");
            }
        }
    }
}