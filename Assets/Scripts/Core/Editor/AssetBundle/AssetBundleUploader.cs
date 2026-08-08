using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Json;
using Core.Utility;

namespace Core.Editor.AssetBundle.Core
{
    /// <summary>
    /// 负责上传 AB 包到服务器，支持增量上传
    /// </summary>
    public class AssetBundleUploader
    {
        private readonly Action<string> logAction;
        private readonly JsonManager jsonManager;
        private int totalUploadCount;
        private int finishedUploadCount;

        public AssetBundleUploader(Action<string> logAction = null)
        {
            this.logAction = logAction;
            jsonManager = DIContainer.Create<JsonManager>();
        }

        private void Log(string msg) => logAction?.Invoke(msg);

        /// <summary>
        /// 增量上传（默认）：仅上传与服务器清单相比有变化的文件
        /// </summary>
        public async void UploadIncrementalAsync(string serverDataPath, string serverUrl,
                                                 bool useAuth, string user, string pwd,
                                                 bool autoChunkSize, uint customChunkSize,
                                                 string catalogName,
                                                 HashSet<string> forceUploadBundleNames = null)
        {
            Log("--- Starting Incremental Upload ---");

            var localCatalogPath = Path.Combine(serverDataPath, catalogName);
            if (!File.Exists(localCatalogPath))
            {
                Log($"本地没有 {catalogName}，无法上传。");
                return;
            }

            // 读取本地目录
            var localCatalog = jsonManager.FromJson<AssetCatalog>(await File.ReadAllTextAsync(localCatalogPath));

            // 尝试下载服务器清单
            AssetCatalog serverCatalog = null;
            var catalogUrl = $"{serverUrl.TrimEnd('/')}/{AssetBundleBuilder.AssetCatalogName}";
            try
            {
                serverCatalog = await DownloadServerManifestAsync(catalogUrl, useAuth, user, pwd);
                if (serverCatalog != null)
                    Log($"服务器目录下载成功，包含 {serverCatalog.ABPackageCollection.Count} 个包。");
                else
                    Log("未找到服务器目录，将全量上传。");
            }
            catch (Exception e)
            {
                Log($"下载服务器目录失败：{e.Message}，将全量上传。");
            }

            var filesToUpload = new List<string>(); // 存储完整路径

            // 处理 .assetBundle 文件
            foreach (var (bundleName, localAbInfo) in localCatalog.ABPackageCollection)
            {
                var fileName = bundleName.WithAbSuffix();
                var filePath = Path.Combine(serverDataPath, fileName);
                if (!File.Exists(filePath)) continue;

                // 强制上传判断
                var forceUpload = forceUploadBundleNames != null && forceUploadBundleNames.Contains(bundleName);

                if (forceUpload)
                {
                    filesToUpload.Add(filePath);
                    Log($"强制上传：{fileName} (依赖变化)");
                    continue;
                }

                if (serverCatalog == null)
                {
                    filesToUpload.Add(filePath);
                }
                else
                {
                    if (!serverCatalog.ABPackageCollection.TryGetValue(fileName, out var serverInfo) || serverInfo.Hash != localAbInfo.Hash)
                    {
                        filesToUpload.Add(filePath);
                        Log($"需上传：{fileName} (哈希不同或新增)");
                    }
                }
            }

            // 强制加入目录
            filesToUpload.Add(localCatalogPath);

            if (filesToUpload.Count == 0)
            {
                Log("没有文件需要上传。");
                return;
            }

            totalUploadCount = filesToUpload.Count;
            finishedUploadCount = 0;
            Log($"共 {totalUploadCount} 个文件待上传。");

            foreach (var file in filesToUpload)
            {
                string fileName = Path.GetFileName(file);
                UploadFileAsync(file, fileName, serverUrl, useAuth, user, pwd, autoChunkSize, customChunkSize);
            }
        }

        // 辅助方法：下载服务器 AssetCatalog.json
        private async Task<AssetCatalog> DownloadServerManifestAsync(string url, bool useAuth, string user, string pwd)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var req = WebRequest.Create(url) as HttpWebRequest;
                    req.Method = WebRequestMethods.Http.Get;
                    req.Timeout = 30000;
                    if (useAuth) req.Credentials = new NetworkCredential(user, pwd);

                    using var res = req.GetResponse() as HttpWebResponse;
                    if (res.StatusCode != HttpStatusCode.OK) return null;

                    using var stream = res.GetResponseStream();
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    var json = reader.ReadToEnd();
                    return jsonManager.FromJson<AssetCatalog>(json);
                }
                catch(Exception e)
                {
                    Log($"下载服务器 AssetCatalog.json 异常：{e.Message}");
                    return null;
                }
            });
        }

        /// <summary>
        /// 全量上传（保留原有行为）
        /// </summary>
        public void UploadAll(string serverDataPath, string serverUrl, bool useAuth, string user, string pwd,
                              bool autoChunkSize, uint customChunkSize)
        {
            Log("--- Starting Full Upload ---");
            var dir = new DirectoryInfo(serverDataPath);
            var files = new List<FileInfo>();
            foreach (var file in dir.GetFiles())
            {
                if (file.Extension == FileUtility.AbSuffix || file.Extension == ".json")
                    files.Add(file);
            }

            totalUploadCount = files.Count;
            finishedUploadCount = 0;

            Log($"Total files to upload: {totalUploadCount}");
            foreach (var file in files)
            {
                UploadFileAsync(file.FullName, file.Name, serverUrl, useAuth, user, pwd, autoChunkSize, customChunkSize);
            }
        }

        private async void UploadFileAsync(string filePath, string fileName, string serverUrl,
                                           bool useAuth, string user, string pwd,
                                           bool autoChunkSize, uint customChunkSize)
        {
            // 与之前实现完全相同，仅将 Interlocked.Increment 改为 finishedUploadCount++
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var req = WebRequest.Create(new Uri(serverUrl)) as HttpWebRequest;
                        if (req == null) return;
                        req.Method = WebRequestMethods.Http.Post;
                        req.ContentType = "multipart/form-data;boundary=MrQiu";
                        req.Timeout = 500000;

                        if (useAuth)
                            req.Credentials = new NetworkCredential(user, pwd);
                        req.PreAuthenticate = true;

                        var head = "--MrQiu\r\n" +
                                   $"Content-Disposition:form-data;name=\"file\";filename=\"{fileName}\"\r\n" +
                                   "Content-Type:application/octet-stream\r\n\r\n";
                        var headBytes = Encoding.UTF8.GetBytes(head);
                        var endBytes = Encoding.UTF8.GetBytes("\r\n--MrQiu--\r\n");

                        using (var fileStream = File.OpenRead(filePath))
                        {
                            req.ContentLength = headBytes.Length + fileStream.Length + endBytes.Length;

                            long chunkSize;
                            if (autoChunkSize)
                            {
                                chunkSize = req.ContentLength switch
                                {
                                    >= 1024 * 1024 * 100 => 1024 * 1024,
                                    >= 1024 * 1024 * 50 => 65536,
                                    > 1024 * 1024 => 4096,
                                    _ => req.ContentLength
                                };
                            }
                            else
                            {
                                chunkSize = customChunkSize;
                            }

                            using var upStream = req.GetRequestStream();
                            upStream.Write(headBytes, 0, headBytes.Length);
                            var buffer = new byte[chunkSize];
                            int read;
                            while ((read = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                upStream.Write(buffer, 0, read);
                            upStream.Write(endBytes, 0, endBytes.Length);
                        }

                        var res = req.GetResponse() as HttpWebResponse;
                        if (res != null && res.StatusCode == HttpStatusCode.OK)
                        {
                            Log($"{fileName}：Upload Success，Progress：{Interlocked.Increment(ref finishedUploadCount)}/{totalUploadCount}");
                        }
                        else
                        {
                            Log($"{fileName}：Upload Fail，StatusCode：{res?.StatusCode}");
                        }
                        res?.Close();
                    }
                    catch (Exception e)
                    {
                        Interlocked.Increment(ref finishedUploadCount);
                        Log($"Upload error file：{fileName}，Exception：{e.Message}");
                    }
                });
            }
            catch (Exception e)
            {
                Log($"Upload Exception：{e.Message}");
            }

            if (finishedUploadCount == totalUploadCount && totalUploadCount > 0)
            {
                Log("--- End Upload Data Over ---");
                totalUploadCount = 0;
                finishedUploadCount = 0;
            }
        }
    }
}