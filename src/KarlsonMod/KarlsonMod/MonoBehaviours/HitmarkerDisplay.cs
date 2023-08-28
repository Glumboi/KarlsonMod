using System;
using System.IO;
using BepInEx;
using UnityEngine;

namespace KarlsonMod.MonoBehaviours;

public class HitMarkerDisplay : MonoBehaviour
{
    public string hitMarkerImageRelativePath = $"plugins/{KarlsonModPlugin.PluginName}/hitmarker.png"; // Relative path to the hit marker image within the plugin folder
    public float hitMarkerSize = 50f; // Size of the hit marker in pixels
    public float displayDuration = 0.5f; // How long the hit marker should be visible

    private bool isShowingHitMarker = false;
    private float displayStartTime;
    private Texture2D hitMarkerTexture;

    private void Start()
    {
        string pluginPath = Path.GetDirectoryName(Paths.PluginPath);
        string hitMarkerImagePath = Path.Combine(pluginPath, hitMarkerImageRelativePath);

        hitMarkerTexture = LoadTextureFromFile(hitMarkerImagePath);

        if (hitMarkerTexture == null)
        {
            // Handle the failure to load the texture
            KarlsonModPlugin.Log.LogError("Hit marker texture could not be loaded.");
        }
    }

    private void Update()
    {
        if (isShowingHitMarker)
        {
            // Check if it's time to hide the hit marker
            if (Time.time - displayStartTime >= displayDuration)
            {
                isShowingHitMarker = false;
            }
        }
    }

    public void ShowHitMarker()
    {
        isShowingHitMarker = true;
        displayStartTime = Time.time;
    }

    private void OnGUI()
    {
        if (isShowingHitMarker && hitMarkerTexture != null)
        {
            float timeSinceDisplayStart = Time.time - displayStartTime;
        
            // Calculate the alpha value based on time to achieve fade-out effect
            float alpha = Mathf.Lerp(1f, 0f, timeSinceDisplayStart / displayDuration);
        
            // Apply the alpha value to the hit marker color
            Color originalColor = GUI.color;
            GUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            // Calculate the position to draw the hit marker at the center of the screen
            float centerX = Screen.width * 0.5f;
            float centerY = Screen.height * 0.5f;

            // Draw the hit marker using the loaded texture
            GUI.DrawTexture(
                new Rect(centerX - hitMarkerSize * 0.5f, centerY - hitMarkerSize * 0.5f, hitMarkerSize, hitMarkerSize),
                hitMarkerTexture);

            // Restore the original GUI color
            GUI.color = originalColor;

            // Check if it's time to hide the hit marker
            if (timeSinceDisplayStart >= displayDuration)
            {
                isShowingHitMarker = false;
            }
        }
    }

    
    // Load a texture from a file path
    private Texture2D LoadTextureFromFile(string filePath)
    {
        Texture2D texture = new Texture2D(2, 2);
        byte[] imageData = File.ReadAllBytes(filePath);

        try
        {
            if (ImageConversion.LoadImage(texture, imageData))
            {
                return texture;
            }
            else
            {
                KarlsonModPlugin.Log.LogError("Failed to load texture from file: " + filePath);
                Destroy(texture); // Clean up the texture if loading fails
            }
        }
        catch (Exception e)
        {
            KarlsonModPlugin.Log.LogError("Failed to load texture from file: " + filePath);
        }

        return null; // Return null to indicate failure
    }
}