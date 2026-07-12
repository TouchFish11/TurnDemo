using System.Runtime.InteropServices;
using UnityEngine;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 内存存储服务器
    /// </summary>
    public class StorageHelper
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
        
        public static long GetAvailableSpace()
        {
#if UNITY_STANDALONE_WIN
            if (!GetDiskFreeSpaceEx(Application.persistentDataPath, out var freeBytesAvailable, out _, out _))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }
            return (long)freeBytesAvailable;
#else
            throw new NotSupportedException("当前平台不支持");
#endif
        }
    }
}
