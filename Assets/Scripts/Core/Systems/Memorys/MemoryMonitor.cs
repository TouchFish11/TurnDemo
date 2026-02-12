using System;
using System.Collections.Generic;
using Core.Log;
using Core.Mono;
using Core.Service;
using Core.Singleton;
using Core.Utility;
using UnityEngine;

namespace Core.Systems.Memorys
{
    /// <summary>
    /// 内存监视器
    /// </summary>
    public class MemoryMonitor : SingletonAutoMono<MemoryMonitor>, IMemoryMonitor
    {
        // 监听者列表
        private readonly List<IMemoryListener> _listeners = new();
        // 当前内存占用级别
        private EMemoryOccupationLevel currentOccupationLevel = EMemoryOccupationLevel.Normal;
        // 占系统内存60%
        private const float warningThresholdRatio = 0.6f;
        // 占系统内存80%
        private const float criticalThresholdRatio = 0.8f;
        // 检查间隔
        private const float checkIntervalSeconds = 30f;
        // 当前时间
        private float nowTime;
        
        // 
        private long currentMemory;
        private long currentSystemMemory;
        private float currentRatio;
        
        private void OnEnable()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            Application.lowMemory += OnLowMemory;
        }

        public void Register(IMemoryListener listener)
        {
            _listeners.Add(listener);
        }

        private void OnLowMemory()
        {
            SetCurrentOccupationLevel(EMemoryOccupationLevel.Critical);
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void CheckMemory()
        {
            // 使用 GC 获取托管内存
            currentMemory = GC.GetTotalMemory(false);
            currentSystemMemory = SystemInfo.systemMemorySize * 1024L * 1024L;
            currentRatio = (float)currentMemory / currentSystemMemory;

            var newLevel = currentRatio switch
            {
                >= criticalThresholdRatio => EMemoryOccupationLevel.Critical,
                >= warningThresholdRatio => EMemoryOccupationLevel.Warning,
                _ => EMemoryOccupationLevel.Normal
            };

            SetCurrentOccupationLevel(newLevel);
        }

        public void Unregister(IMemoryListener listener)
        {
            _listeners.Remove(listener);
        }

        private void SetCurrentOccupationLevel(EMemoryOccupationLevel currentOccupationLevel)
        {
            LogManager.Log($"当前内存占用级别：{currentOccupationLevel}。" +
                           $"当前内存占用：{TextUtility.ToByteUnit((ulong)currentMemory)}，" +
                           $"系统内存：{TextUtility.ToByteUnit((ulong)currentSystemMemory)}，" +
                           $"比值：{TextUtility.FloatToStr(currentRatio * 100, 2)}%");
            
            if (this.currentOccupationLevel == currentOccupationLevel)
            {
                return;
            }
            
            this.currentOccupationLevel = currentOccupationLevel;
            // 通知所有监听者
            Report();
        }
        
        private void Report()
        {
            foreach (var listener in _listeners)
            {
                listener.OnReport();
            }
        }

        private void OnUpdate()
        {
            if (UnityEngine.Time.realtimeSinceStartup - nowTime >= checkIntervalSeconds)
            {
                CheckMemory();
                nowTime = UnityEngine.Time.realtimeSinceStartup;
            }
        }

        private void OnDisable()
        {
            Application.lowMemory -= OnLowMemory;
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
