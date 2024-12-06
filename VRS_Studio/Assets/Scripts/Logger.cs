using UnityEngine;

public static class Log
{
    public static void d(string tag, string msg)
    {
        Debug.Log($"{tag}, {msg}");
    }

    public static void w(string tag, string msg)
    {
        Debug.LogWarning($"{tag}, {msg}");
    }

    public static void e(string tag, string msg)
    {
        Debug.LogError($"{tag}, {msg}");
    }
}
