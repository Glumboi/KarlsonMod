using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;

namespace KarlsonMod.Patches;

[HarmonyPatch(typeof(Enemy))]
public class EnemyPatches
{
    private static Traverse mainTv;
    private static Traverse ragdollTv;
    private static Traverse animatorTv;
    private static Traverse agentTv;

    [HarmonyPatch("Aim")]
    [HarmonyPrefix]
    public static void Aim_Prefix(Enemy __instance)
    {
        if (!(__instance.currentGun == null) &&
            !ragdollTv.GetValue<RagdollController>().IsRagdoll() &&
            animatorTv.GetValue<Animator>().GetBool("Aiming")&&
            __instance.IsDead())
        {
            var target = mainTv.Field("target").GetValue<Transform>();
            var hipSpeed = mainTv.Field("hipSpeed").GetValue<float>();
            var headAndHandSpeed = mainTv.Field("headAndHandSpeed").GetValue<float>();
            var attackSpeed = mainTv.Field("attackSpeed").GetValue<float>();
            var gunScript = mainTv.Field("gunScript").GetValue<Weapon>();
            var readyToShoot = mainTv.Field("readyToShoot");

            Vector3 vector = target.position - __instance.transform.position;
            if (Vector3.Angle(__instance.transform.forward, vector) > 70f)
            {
                __instance.transform.rotation = Quaternion.Slerp(__instance.transform.rotation,
                    Quaternion.LookRotation(vector),
                    Time.deltaTime * hipSpeed);
            }

            __instance.head.transform.rotation = Quaternion.Slerp(__instance.head.transform.rotation,
                Quaternion.LookRotation(vector),
                Time.deltaTime * headAndHandSpeed);
            __instance.rightArm.transform.rotation = Quaternion.Slerp(__instance.head.transform.rotation,
                Quaternion.LookRotation(vector),
                Time.deltaTime * headAndHandSpeed);
            __instance.leftArm.transform.rotation = Quaternion.Slerp(__instance.head.transform.rotation,
                Quaternion.LookRotation(vector),
                Time.deltaTime * headAndHandSpeed);
            if (readyToShoot.GetValue<bool>())
            {
                gunScript.Use(target.position);
                readyToShoot.SetValue(false);
                __instance.Invoke("Cooldown",
                    attackSpeed + Random.Range(attackSpeed, attackSpeed * 5f));
            }
        }

        return;
    }

    [HarmonyPatch("Start")]
    [HarmonyPrefix]
    public static void Start_Prefix(Enemy __instance)
    {
        mainTv = Traverse.Create(__instance);
        ragdollTv = mainTv.Field("ragdoll");
        animatorTv = mainTv.Field("animator");
        agentTv = mainTv.Field("agent");

        ragdollTv.SetValue(__instance.GetComponent<RagdollController>());
        animatorTv.SetValue(__instance.GetComponentInChildren<Animator>());
        agentTv.SetValue(__instance.GetComponent<NavMeshAgent>());
        mainTv.Method("GiveGun");

        KarlsonModPlugin.Log.LogInfo("Overwritten enemy start!");
        return;
    }
}