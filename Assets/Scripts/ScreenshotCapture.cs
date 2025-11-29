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
    {
        if (targetImage == null)
        {
            Debug.LogError("Target image is not assigned!");
            return;
        }
        
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
        
        // Save to persistent data path
        string filePath = Path.Combine(Application.persistentDataPath, uniqueFileName);
        File.WriteAllBytes(filePath, bytes);
        
        Debug.Log($"High quality screenshot saved to: {filePath} ({finalWidth}x{finalHeight})");
        
        // Clean up
        Destroy(finalTexture);
    }
    
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
