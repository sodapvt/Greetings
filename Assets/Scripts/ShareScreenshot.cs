using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShareScreenshot : MonoBehaviour
{
    public Image targetImage;
    [Range(1, 4)]
    public int qualityMultiplier = 2;
    
    private bool isProcessing = false;
    
    public void CaptureAndShare()
    {
        if (isProcessing)
        {
            Debug.Log("Already processing a screenshot...");
            return;
        }
        
        if (targetImage == null)
        {
            Debug.LogError("Target image is not assigned!");
            return;
        }
        
        StartCoroutine(CaptureAndShareCoroutine());
    }
    
    private IEnumerator CaptureAndShareCoroutine()
    {
        isProcessing = true;
        
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
        
        // Encode to PNG
        byte[] bytes = finalTexture.EncodeToPNG();
        
        // Generate unique filename with timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Greeting_{timestamp}.png";
        string filePath = Path.Combine(Application.temporaryCachePath, fileName);
        
        // Save to temporary cache path
        File.WriteAllBytes(filePath, bytes);
        
        Debug.Log($"Screenshot saved temporarily: {filePath}");
        
        // Clean up texture
        Destroy(finalTexture);
        
        // Wait a frame before sharing
        yield return new WaitForEndOfFrame();
        
        // Share the file
        ShareFile(filePath, "Check out my greeting!", "Share Greeting");
        
        isProcessing = false;
    }
    
    private void ShareFile(string filePath, string shareText, string shareSubject)
    {
#if UNITY_ANDROID
        ShareAndroid(filePath, shareText, shareSubject);
#elif UNITY_IOS
        ShareIOS(filePath, shareText, shareSubject);
#else
        Debug.Log($"Sharing not supported in editor. File saved at: {filePath}");
#endif
    }
    
#if UNITY_ANDROID
    private void ShareAndroid(string filePath, string shareText, string shareSubject)
    {
        if (!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject uriObject;
            
            // Use FileProvider for Android 7.0+
            AndroidJavaClass fileProviderClass = new AndroidJavaClass("androidx.core.content.FileProvider");
            AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File");
            AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", filePath);
            
            string packageName = currentActivity.Call<string>("getPackageName");
            uriObject = fileProviderClass.CallStatic<AndroidJavaObject>("getUriForFile", 
                currentActivity, 
                packageName + ".fileprovider", 
                fileObject);
            
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>("addFlags", 1); // FLAG_GRANT_READ_URI_PERMISSION
            
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share via");
            currentActivity.Call("startActivity", chooser);
            
            Debug.Log("Android share initiated");
        }
    }
#endif
    
#if UNITY_IOS
    private void ShareIOS(string filePath, string shareText, string shareSubject)
    {
        // For iOS, you would need to use a native plugin or implement native code
        // This is a placeholder - you'll need NativeShare plugin or similar
        Debug.Log($"iOS sharing requires NativeShare plugin. File at: {filePath}");
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
