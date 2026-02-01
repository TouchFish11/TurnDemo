using UnityEngine;

namespace Core.Utility
{
    /// <summary>
    /// 加密工具类
    /// </summary>
    public static class EncryptionUtility
    {
        /// <summary>
        /// 获取随机密钥
        /// </summary>
        /// <returns>int类型密钥</returns>
        public static int GetRandomKey()
        {
            return Random.Range(-1_0000_0000, 10_0000_0001);
        }

        /// <summary>
        /// Int加密
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的int数值</returns>
        public static int Lock(int value, int key)
        {
            value ^= (key % 9);
            value ^= 0xADAD;
            value ^= (1 << 5);
            value += key;
            return value;
        }

        /// <summary>
        /// Long加密
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后的long数值</returns>
        public static long Lock(long value, int key)
        {
            value ^= (key % 9);
            value ^= 0xADAD;
            value ^= (1 << 5);
            value += key;
            return value;
        }

        /// <summary>
        /// Int解密
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="key">密钥</param>
        /// <returns>原int数值</returns>
        public static int UnLock(int value, int key)
        {
            value -= key;
            value ^= (key % 9);
            value ^= 0xADAD;
            value ^= (1 << 5);
            return value;
        }

        /// <summary>
        /// Long解密
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="key">密钥</param>
        /// <returns>原long数值</returns>
        public static long UnLock(long value, int key)
        {
            value -= key;
            value ^= (key % 9);
            value ^= 0xADAD;
            value ^= (1 << 5);
            return value;
        }
    }
}
