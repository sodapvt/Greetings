using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndSharePhoto : MonoBehaviour
{
    public Image targetImage,offsetImage;
    [Range(1, 4)]
    public int qualityMultiplier = 2;
    
    private bool isProcessing = false;
    public PanelHandler panelHandler;
    // Call this to save to gallery and share
    public void SaveAndShare()
    {AudioHandler.instance.PlaySFX("pop");
        panelHandler.ClosePanel();
        if (isProcessing)
        {
            Debug.Log("Already processing...");
            return;
        }
        
        if (targetImage == null)
        {
            Debug.LogError("Target image is not assigned!");
            return;
        }
        
        StartCoroutine(SaveAndShareCoroutine());
    }
    
    private IEnumerator SaveAndShareCoroutine()
    {
        isProcessing = true;
        
        yield return new WaitForEndOfFrame();
        
        // Capture the image area
        byte[] imageBytes = CaptureImageArea();
        
        if (imageBytes == null)
        {
            Debug.LogError("Failed to capture image");
            isProcessing = false;
            yield break;
        }
        
        // Generate filename
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"Greeting_{timestamp}.png";
        
        // Save to gallery (Android/iOS)
        string savedPath = SaveToGallery(imageBytes, fileName);
        
        if (!string.IsNullOrEmpty(savedPath))
        {
            Debug.Log($"Image saved to gallery: {savedPath}");
            
            // Share the image
            ShareImage(savedPath);
        }
        else
        {
            Debug.LogError("Failed to save image to gallery");
        }
        
        isProcessing = false;
    }
    
    private byte[] CaptureImageArea()
    {
        RectTransform rectTransform = targetImage.rectTransform;
         RectTransform offsetRectTransform = offsetImage.rectTransform;
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
         Vector3[] offsetCorners = new Vector3[4];
        offsetRectTransform.GetWorldCorners(offsetCorners);
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[i]);
        }
        for (int i = 0; i < offsetCorners.Length; i++)
        {
            offsetCorners[i] = RectTransformUtility.WorldToScreenPoint(Camera.main, offsetCorners[i]);
        }

        float minX ;
        float minY ;
        float maxX ;
        float maxY;  
        if (FlowHandler.instance.doctorNameEntered)
        {
                    // Calculate combined bounds (targetImage + offsetImage)
 minX = Mathf.Min(corners[0].x, offsetCorners[0].x);
 minY = Mathf.Min(corners[0].y, offsetCorners[0].y);

 maxX = Mathf.Max(corners[2].x, offsetCorners[2].x);
 maxY = Mathf.Max(corners[2].y, offsetCorners[2].y);
        }
        else
        {
                  // Calculate the bounds
         minX = corners[0].x;
         minY = corners[0].y;
         maxX = corners[2].x;
         maxY = corners[2].y;  
        }
        
        int width = Mathf.RoundToInt(maxX - minX);
        int height = Mathf.RoundToInt(maxY - minY);
        int x = Mathf.RoundToInt(minX);
        int y = Mathf.RoundToInt(minY);
        
        x = Mathf.Clamp(x, 0, Screen.width);
        y = Mathf.Clamp(y, 0, Screen.height);
        width = Mathf.Clamp(width, 1, Screen.width - x);
        height = Mathf.Clamp(height, 1, Screen.height - y);
        
        int finalWidth = width * qualityMultiplier;
        int finalHeight = height * qualityMultiplier;
        
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(x, y, width, height), 0, 0);
        screenshot.Apply();
        
        Texture2D finalTexture = screenshot;
        if (qualityMultiplier > 1)
        {
            finalTexture = ScaleTexture(screenshot, finalWidth, finalHeight);
            Destroy(screenshot);
        }
        
        byte[] bytes = finalTexture.EncodeToPNG();
        Destroy(finalTexture);
        
        return bytes;
    }
    
    private string SaveToGallery(byte[] imageBytes, string fileName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return SaveToGalleryAndroid(imageBytes, fileName);
#else
        // For editor/other platforms, save to persistent data path
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllBytes(path, imageBytes);
        Debug.Log($"Saved to: {path}");
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
    
    private void ShareImage(string imagePath)
    {
        using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
        using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
        {
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            
            // Get content URI from MediaStore instead of file URI (Android 7.0+ compatible)
            AndroidJavaObject contentUri = GetContentUriForFile(imagePath);
            
            if (contentUri != null)
            {
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), contentUri);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "");
                intentObject.Call<AndroidJavaObject>("addFlags", 1); // FLAG_GRANT_READ_URI_PERMISSION
                
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Greeting");
                    currentActivity.Call("startActivity", chooser);
                }
                
                Debug.Log("Share dialog opened");
            }
            else
            {
                Debug.LogError("Failed to get content URI for sharing");
            }
        }
    }
    
    private AndroidJavaObject GetContentUriForFile(string filePath)
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
            {
                // Query MediaStore for the content URI
                using (AndroidJavaClass mediaStoreImages = new AndroidJavaClass("android.provider.MediaStore$Images$Media"))
                {
                    AndroidJavaObject externalContentUri = mediaStoreImages.GetStatic<AndroidJavaObject>("EXTERNAL_CONTENT_URI");
                    
                    // Get content resolver
                    using (AndroidJavaObject contentResolver = context.Call<AndroidJavaObject>("getContentResolver"))
                    {
                        // Query for the file
                        string[] projection = new string[] { "_id" };
                        string selection = "_data=?";
                        string[] selectionArgs = new string[] { filePath };
                        
                        using (AndroidJavaObject cursor = contentResolver.Call<AndroidJavaObject>("query", 
                            externalContentUri, projection, selection, selectionArgs, null))
                        {
                            if (cursor != null && cursor.Call<bool>("moveToFirst"))
                            {
                                int idColumn = cursor.Call<int>("getColumnIndexOrThrow", "_id");
                                long id = cursor.Call<long>("getLong", idColumn);
                                cursor.Call("close");
                                
                                // Build content URI
                                using (AndroidJavaClass contentUris = new AndroidJavaClass("android.content.ContentUris"))
                                {
                                    return contentUris.CallStatic<AndroidJavaObject>("withAppendedId", externalContentUri, id);
                                }
                            }
                            
                            if (cursor != null)
                            {
                                cursor.Call("close");
                            }
                        }
                    }
                }
            }
            
            // Fallback to file URI for older Android versions
            Debug.LogWarning("Could not find content URI, falling back to file URI");
            using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaClass fileClass = new AndroidJavaClass("java.io.File"))
            using (AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", filePath))
            {
                return uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting content URI: {e.Message}");
            return null;
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
