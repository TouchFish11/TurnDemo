using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utility
{
    /// <summary>
    /// Hash工具类
    /// </summary>
    public static class HashUtility
    {
        /// <summary>
        /// 计算文件内容的 SHA256 哈希值
        /// 阻塞主线程
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>SHA256 哈希值的十六进制字符串</returns>
        public static string GenerateFileSHA256Hash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var fileStream = File.OpenRead(filePath);
            var hashBytes = sha256.ComputeHash(fileStream);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// 异步计算文件内容的 SHA256 哈希值
        /// 通过多线程计算
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>SHA256 哈希值的十六进制字符串</returns>
        public static async Task<string> GenerateFileSHA256HashAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                using var sha256 = SHA256.Create();
                using var fileStream = File.OpenRead(filePath);
                var hashBytes = sha256.ComputeHash(fileStream);
            
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            });
        }
    }
}
