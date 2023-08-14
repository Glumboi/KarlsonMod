using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KarlsonMod.Patches;
using UnityEngine;

namespace KarlsonMod
{
    internal class UI
    {
        public static void CustomMenu()
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(300));
            GUI.backgroundColor = Color.red;
            GUILayout.Label(KarlsonModPlugin.PluginName, new GUILayoutOption[0]);
            GUILayout.Space(10f);
            GUILayout.Label("Sliders", new GUILayoutOption[0]);

            GUILayout.Space(10f);
            GUILayout.Label("Jumping force", new GUILayoutOption[0]);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            KarlsonModPlugin.JumpingForceEntry.Value = GUILayout.HorizontalSlider(KarlsonModPlugin.JumpingForceEntry.Value, 550f, 5000f);
            if (GUILayout.Button("Reset", new GUILayoutOption[1] { GUILayout.Width(60) }))
            {
                KarlsonModPlugin.JumpingForceEntry.Value = 550f;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Min: {550f}", new GUILayoutOption[0]);
            GUILayout.Label($"Current: {KarlsonModPlugin.JumpingForceEntry.Value}", new GUILayoutOption[0]);
            GUILayout.Label($"Max: {5000f}", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.Label("Slide slowdown", new GUILayoutOption[0]);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            KarlsonModPlugin.SlideSlowDownEntry.Value = GUILayout.HorizontalSlider(KarlsonModPlugin.SlideSlowDownEntry.Value, 0.0f, 0.2f);
            if (GUILayout.Button("Reset", new GUILayoutOption[1] { GUILayout.Width(60) }))
            {
                KarlsonModPlugin.SlideSlowDownEntry.Value = 0.2f;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Min: {0.0f}", new GUILayoutOption[1] { GUILayout.Width(60) });
            GUILayout.Label($"Current: {KarlsonModPlugin.SlideSlowDownEntry.Value}", new GUILayoutOption[0]);
            GUILayout.Label($"Max: {0.2f}", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.Label("Move speed", new GUILayoutOption[0]);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            KarlsonModPlugin.MoveSpeedEntry.Value = GUILayout.HorizontalSlider(KarlsonModPlugin.MoveSpeedEntry.Value, 20f, 100f);
            if (GUILayout.Button("Reset", new GUILayoutOption[1] { GUILayout.Width(60) }))
            {
                KarlsonModPlugin.MoveSpeedEntry.Value = 20f;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Min: {20f}", new GUILayoutOption[0]);
            GUILayout.Label($"Current: {KarlsonModPlugin.MoveSpeedEntry.Value}", new GUILayoutOption[0]);
            GUILayout.Label($"Max: {100f}", new GUILayoutOption[0]);
            GUILayout.EndHorizontal();

            GUILayout.Space(30f);
            GUILayout.Label("Checkboxes", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            KarlsonModPlugin.GodmodeEntry.Value = GUILayout.Toggle(KarlsonModPlugin.GodmodeEntry.Value, "Godmode", new GUILayoutOption[0]);
            KarlsonModPlugin.ExplosiveBulletsEntry.Value = GUILayout.Toggle(KarlsonModPlugin.ExplosiveBulletsEntry.Value, "Explosive Bullets", new GUILayoutOption[0]);

            GUILayout.EndHorizontal();

            GUILayout.Space(30f);
            GUILayout.Label("Spawners", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);

            if (GUILayout.Button("Drag all weapons to me", new GUILayoutOption[0]))
            {
                PlayerPatches.DragAllWeaponsToMe();
            } 
            
            if (GUILayout.Button("Spawn enemy (Coming soon)", new GUILayoutOption[0]))
            {
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            string gameObjSearch = "";
            gameObjSearch = GUILayout.TextField(gameObjSearch);

            if (GUILayout.Button("Drag specified item to me (coming soon)", new GUILayoutOption[0]))
            {
                //PlayerPatches.DragItemToMe(gameObjSearch);
            }

            GUILayout.Space(30f);
            GUILayout.Label($"Remaining Enemies left: {PlayerPatches.EnemiesLeft()}");

            GUILayout.EndVertical();
        }
    }
}
