namespace Core.Utility
{
    /// <summary>
    /// Time工具类，对Unity引擎的Time类进行封装
    /// </summary>
    public static class TimeUtil
    {
        public static float Time => UnityEngine.Time.time;
        
        public static float DeltaTime => UnityEngine.Time.deltaTime;
        
        public static float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
        
        public static float RealtimeSinceStartup => UnityEngine.Time.realtimeSinceStartup;
        
        public static double RealtimeSinceStartupAsDouble => UnityEngine.Time.realtimeSinceStartupAsDouble;
        
        public static float Timescale { get => UnityEngine.Time.timeScale; set => UnityEngine.Time.timeScale = value; }

        public static float UnscaledTime => UnityEngine.Time.unscaledTime;
        
        public static float UnscaledDeltaTime => UnityEngine.Time.unscaledDeltaTime;

        public static double UnscaledTimeAsDouble => UnityEngine.Time.unscaledTimeAsDouble;

        public static float FixedUnscaledDeltaTime => UnityEngine.Time.fixedUnscaledDeltaTime;
        
        public static float FixedUnscaledTime => UnityEngine.Time.fixedUnscaledTime;
    }
}
