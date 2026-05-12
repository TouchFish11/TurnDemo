using System;
using System.Collections.Generic;
using Core.Mono;
using Core.Utility;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.Systems.Memorys
{
    /// <summary>
    /// 内存监视器
    /// </summary>
    public class MemoryMonitor : IMemoryMonitor
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
        
        // 当前内存相关
        private long currentMemory;
        private long currentSystemMemory;
        private float currentRatio;

        private MemoryMonitor(IMonoAdapter monoAdapter)
        {
            monoAdapter.AddUpdateListener(OnUpdate);
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
        /// 检查内存
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
            if (this.currentOccupationLevel == currentOccupationLevel) return;
            
            Logger.Log($"当前内存占用级别：{currentOccupationLevel}。" +
                       $"当前内存占用：{TextUtility.ToByteUnit((ulong)currentMemory)}，" +
                       $"系统内存：{TextUtility.ToByteUnit((ulong)currentSystemMemory)}，" +
                       $"比值：{TextUtility.FloatToStr(currentRatio * 100, 2)}%");
            
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
            if (TimeUtil.RealtimeSinceStartup - nowTime >= checkIntervalSeconds)
            {
                CheckMemory();
                nowTime = TimeUtil.RealtimeSinceStartup;
            }
        }
    }
}
