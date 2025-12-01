using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotCapture : MonoBehaviour
{
    public Image targetImage;
    public string saveFileName = "greeting";
    [Range(1, 4)]
    public int qualityMultiplier = 2; // Higher values = better quality but larger file size
    
    public void CaptureImageArea()
    {AudioHandler.instance.PlaySFX("pop");
        if (targetImage == null)
        {
            Debug.LogError("Target image is not assigned!");
            return;
        }
          AndroidToaster.ShowToast("Saving Image...");
        StartCoroutine(CaptureArea());
    }
    
    private IEnumerator CaptureArea()
    {
      
        // Wait for end of frame to ensure everything is rendered
        yield return new WaitForEndOfFrame();
        
        // Get the RectTransform of the target image
        RectTransform rectTransform = targetImage.rectTransform;
        
        // Get the corners of the RectTransform in screen space
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        // Convert world corners to screen space
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);
        }
        
        // Calculate the bounds
        float minX = corners[0].x;
        float minY = corners[0].y;
        float maxX = corners[2].x;
        float maxY = corners[2].y;
        
        int width = Mathf.RoundToInt(maxX - minX);
        int height = Mathf.RoundToInt(maxY - minY);
        int x = Mathf.RoundToInt(minX);
        int y = Mathf.RoundToInt(minY);
        
        // Ensure values are within screen bounds
        x = Mathf.Clamp(x, 0, Screen.width);
        y = Mathf.Clamp(y, 0, Screen.height);
        width = Mathf.Clamp(width, 1, Screen.width - x);
        height = Mathf.Clamp(height, 1, Screen.height - y);
        
        // Apply quality multiplier for higher resolution
        int finalWidth = width * qualityMultiplier;
        int finalHeight = height * qualityMultiplier;
        
        // Read pixels from the specified area with RGBA32 for maximum quality
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(x, y, width, height), 0, 0);
        screenshot.Apply();
        
        // If quality multiplier > 1, scale up the texture
        Texture2D finalTexture = screenshot;
        if (qualityMultiplier > 1)
        {
            finalTexture = ScaleTexture(screenshot, finalWidth, finalHeight);
            Destroy(screenshot);
        }
        
        // Encode to PNG (PNG is lossless, so quality is preserved)
        byte[] bytes = finalTexture.EncodeToPNG();
        
        // Generate unique filename with timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string uniqueFileName = $"{saveFileName}_{timestamp}.png";
        
        // Save to gallery (Android) or persistent path (Editor)
        string filePath = SaveToGallery(bytes, uniqueFileName);
        
        if (!string.IsNullOrEmpty(filePath))
        {
            Debug.Log($"High quality screenshot saved to: {filePath} ({finalWidth}x{finalHeight})");
            AndroidToaster.ShowToast("Image saved to gallery!");
        }
        else
        {
            Debug.LogError("Failed to save screenshot");
            AndroidToaster.ShowToast("Failed to save image");
        }
        
        // Clean up
        Destroy(finalTexture);
    }
    
    private string SaveToGallery(byte[] imageBytes, string fileName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return SaveToGalleryAndroid(imageBytes, fileName);
#else
        // For editor/other platforms, save to persistent data path
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, imageBytes);
        return path;
#endif
    }
    
#if UNITY_ANDROID
    private string SaveToGalleryAndroid(byte[] imageBytes, string fileName)
    {
        try
        {
            // Save to Pictures directory
            using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
            {
                using (AndroidJavaObject picturesDir = environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", 
                    environment.GetStatic<string>("DIRECTORY_PICTURES")))
                {
                    string picturesPath = picturesDir.Call<string>("getAbsolutePath");
                    string greetingsFolder = Path.Combine(picturesPath, "Greetings");
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(greetingsFolder))
                    {
                        Directory.CreateDirectory(greetingsFolder);
                    }
                    
                    string filePath = Path.Combine(greetingsFolder, fileName);
                    File.WriteAllBytes(filePath, imageBytes);
                    
                    // Notify media scanner to make it visible in gallery
                    ScanFile(filePath);
                    
                    return filePath;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving to gallery: {e.Message}");
            return null;
        }
    }
    
    private void ScanFile(string path)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    using (AndroidJavaClass mediaScanner = new AndroidJavaClass("android.media.MediaScannerConnection"))
                    {
                        mediaScanner.CallStatic("scanFile", context, new string[] { path }, null, null);
                    }
                }
            }
        }
    }
#endif
    
    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGBA32, false);
        Color[] pixels = result.GetPixels(0);
        
        float incX = 1.0f / targetWidth;
        float incY = 1.0f / targetHeight;
        
        for (int px = 0; px < pixels.Length; px++)
        {
            pixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        
        result.SetPixels(pixels, 0);
        result.Apply();
        return result;
    }
}
