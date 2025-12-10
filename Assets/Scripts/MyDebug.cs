using System;

public static class MyDebug
{
    public static void Log(object obj)
    {
        UnityEngine.Debug.Log($"{DateTime.Now.Ticks} : {obj}");
    }

    public static void Error(object obj)
    {
        UnityEngine.Debug.LogError($"{DateTime.Now.Ticks} : {obj}");
    }
}