using Audio;
using HarmonyLib;
using KarlsonMod.MonoBehaviours;
using UnityEngine;

namespace KarlsonMod.Patches
{
    [HarmonyPatch(typeof(Bullet))]
    internal class BulletPatches
    {
        public static bool explosiveBullets = false;
        private static Traverse mainTvs;
        private static HitMarkerDisplay hm;

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void Start_Prefix(Bullet __instance)
        {
            __instance.explosive = explosiveBullets;
            mainTvs = Traverse.Create(__instance);
            //Setup hitmarker
            GameObject hitMarkerDisplayObject = new GameObject();
            hitMarkerDisplayObject.AddComponent<HitMarkerDisplay>();
            hm = hitMarkerDisplayObject.GetComponent<HitMarkerDisplay>();
        }

        [HarmonyPatch("OnCollisionEnter", new[] { typeof(Collision) })]
        [HarmonyPrefix]
        public static void OnCollisionEnter_Prefix(Bullet __instance, Collision other)
        {
            var done = mainTvs.Field("done");
            var explosive = mainTvs.Field("explosive");
            var col = mainTvs.Field("col");

            if (done.GetValue<bool>())
            {
                return;
            }

            done.SetValue(true);

            if (explosive.GetValue<bool>())
            {
                UnityEngine.Object.Destroy(__instance.gameObject);
                ((Explosion)UnityEngine.Object
                    .Instantiate(PrefabManager.Instance.explosion, other.contacts[0].point, Quaternion.identity)
                    .GetComponentInChildren(typeof(Explosion))).player = __instance.player;
                return;
            }

            mainTvs.Method("BulletExplosion", new[] { other.contacts[0] });
            UnityEngine.Object.Instantiate(PrefabManager.Instance.bulletHitAudio, other.contacts[0].point,
                Quaternion.identity);
            int layer = other.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Player"))
            {
                mainTvs.Method("HitPlayer", new[] { other.gameObject });
                UnityEngine.Object.Destroy(__instance.gameObject);
                return;
            }

            if (layer == LayerMask.NameToLayer("Enemy"))
            {
                if (col.GetValue<Color>() == Color.blue)
                {
                    AudioManager.Instance.Play("Hitmarker");
                    MonoBehaviour.print("HITMARKER");
                    hm.GetComponent<HitMarkerDisplay>().ShowHitMarker();
                }

                UnityEngine.Object.Instantiate(PrefabManager.Instance.enemyHitAudio, other.contacts[0].point,
                    Quaternion.identity);
                ((RagdollController)other.transform.root.GetComponent(typeof(RagdollController))).MakeRagdoll(
                    -__instance.transform.right * 350f);
                if ((bool)other.gameObject.GetComponent<Rigidbody>())
                {
                    other.gameObject.GetComponent<Rigidbody>().AddForce(-__instance.transform.right * 1500f);
                }

                ((Enemy)other.transform.root.GetComponent(typeof(Enemy))).DropGun(Vector3.up);
                UnityEngine.Object.Destroy(__instance.gameObject);
                return;
            }

            if (layer == LayerMask.NameToLayer("Bullet"))
            {
                if (other.gameObject.name == __instance.gameObject.name)
                {
                    return;
                }

                UnityEngine.Object.Destroy(__instance.gameObject);
                UnityEngine.Object.Destroy(other.gameObject);
                mainTvs.Method("BulletExplosion", new[] { other.contacts[0] });
            }

            UnityEngine.Object.Destroy(__instance.gameObject);
            return;
        }
    }
}