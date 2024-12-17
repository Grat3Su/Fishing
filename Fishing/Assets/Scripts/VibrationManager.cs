using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    // 진동 패턴과 세기 설정
    public long[] pattern = { 0, 500, 200, 500 }; // [대기시간, 진동시간, 대기시간, 진동시간]
    public int[] amplitudes = { 0, 100, 0, 200 }; // 각 구간 진동 세기 (최대 255)
    //VibrationManager.VibratePattern(pattern, amplitudes);


    public static void Vibrate(long milliseconds, int amplitude = 255)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (vibrator != null)
        {
            // API 26 (Android 8.0) 이상에서만 사용 가능
            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                "createOneShot", milliseconds, amplitude
            );
            vibrator.Call("vibrate", vibrationEffect);
        }
#endif
    }

    public static void VibratePattern(long[] pattern, int[] amplitudes)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (vibrator != null)
        {
            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                "createWaveform", pattern, amplitudes, -1 // -1: 반복 안 함
            );
            vibrator.Call("vibrate", vibrationEffect);
        }
#endif
    }
}
