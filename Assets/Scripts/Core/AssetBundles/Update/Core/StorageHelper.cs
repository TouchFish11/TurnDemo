using System.Runtime.InteropServices;
using UnityEngine;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 内存存储服务器
    /// </summary>
    public class StorageHelper
    {
        public static long GetAvailableSpace(string path = null)
        {
            path ??= Application.persistentDataPath;
            
#if UNITY_STANDALONE_WIN
            if (!GetDiskFreeSpaceEx(path, out var freeBytesAvailable, out _, out _))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return (long)freeBytesAvailable;
#elif UNITY_ANDROID
        return GetFreeSpaceAndroid(path);
#elif UNITY_IOS
        return GetFreeSpaceIOS(path);
#else
            throw new NotSupportedException("当前平台不支持");
#endif
        }
        
        // Windows 平台
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
#endif 
        
        // Android 平台
#if UNITY_ANDROID
        private static long GetFreeSpaceAndroid(string path)
        {
            try
            {
                using var statFs = new AndroidJavaObject("android.os.StatFs", path);
                // 适配 API 18+
                var blockSize = statFs.Call<long>("getBlockSizeLong");
                var availableBlocks = statFs.Call<long>("getAvailableBlocksLong");
                return blockSize * availableBlocks;
            }
            catch (System.Exception e)
            {
                throw new System.Exception($"获取 Android 磁盘空间失败", e);
            }
        }
#endif
        
        // iOS 平台
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern long _GetFreeDiskSpace(string path);

        private static long GetFreeSpaceIOS(string path)
        {
            try
            {
                return _GetFreeDiskSpace(path);
            }
            catch (System.Exception e)
            {
                throw new System.Exception($"获取 IOS 磁盘空间失败", e);
            }
        }
#endif
    }
}
