using UnityEngine;

public class AndroidDeviceInfo
{
#if UNITY_ANDROID && !UNITY_EDITOR
   public static string GetAndroidId()
{
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver"))
                {
                    using (AndroidJavaClass settingsSecure = new AndroidJavaClass("android.provider.Settings$Secure"))
                    {
                        return settingsSecure.CallStatic<string>("getString", contentResolver, "android_id");
                    }
                }
            }
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError("Error getting Android ID: " + ex.Message);
        return "error_device_id";
    }
#else
    return System.Guid.NewGuid().ToString();  // fallback in Editor
#endif
}


    private static AndroidJavaObject GetContext()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
#else
    public static string GetAndroidId()
    {
        return System.Guid.NewGuid().ToString();  // Fallback to GUID in Unity Editor
    }
#endif
}
