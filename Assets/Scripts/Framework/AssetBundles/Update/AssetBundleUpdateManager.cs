using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Framework
{
    /// <summary>
    /// AssetBundle更新管理器
    /// </summary>
    public class AssetBundleUpdateManager : SingletonAutoMono<AssetBundleUpdateManager>
    {
        /// <summary>
        /// 下载状态
        /// </summary>
        private enum E_DownloadState : byte
        {
            /// <summary>
            /// 切换下一个阶段
            /// </summary>
            NextPhase,
            /// <summary>
            /// 等待当前阶段
            /// </summary>
            Waiting,
            /// <summary>
            /// 下载结束
            /// </summary>
            Over,
        }

        /// <summary>
        /// 文件解析类型
        /// </summary>
        private enum E_FileAnalyzeType : byte
        {
            /// <summary>
            /// 本地
            /// </summary>
            Local,
            /// <summary>
            /// 远端
            /// </summary>
            Remote,
        }

        /// <summary>
        /// 存储远端下载的AB包信息字典
        /// Key：AB包名，Value：AB包对比信息
        /// </summary>
        private readonly Dictionary<string, AssetBundleCompareInfo> _remoteABCompareInfoDic = new Dictionary<string, AssetBundleCompareInfo>();

        /// <summary>
        /// 存储本地读取的AB包信息字典
        /// Key：AB包名，Value：AB包对比信息
        /// </summary>
        private readonly Dictionary<string, AssetBundleCompareInfo> _localABCompareInfoDic = new Dictionary<string, AssetBundleCompareInfo>();

        /// <summary>
        /// 存储本地的记录已下载AB包信息的记录文件的字典
        /// Key：AB包名，Value：记录信息
        /// </summary>
        private readonly Dictionary<string, AssetBundleRecordInfo> _localDownedABInfoRecordDic = new Dictionary<string, AssetBundleRecordInfo>();

        /// <summary>
        /// 存储待下载的AB包文件名的字典
        /// Key：AB包名，Value：记录信息
        /// </summary>
        private readonly Dictionary<string, AssetBundleRecordInfo> _waitDownLoadABDic = new Dictionary<string, AssetBundleRecordInfo>();

        /// <summary>
        /// 存储不完整的AB包列表
        /// </summary>
        private readonly List<string> _incompleteABList = new List<string>();

        /// <summary>
        /// 存储待下载请求者列表
        /// </summary>
        private readonly LinkedList<WebRequester> _requesterWaitList = new LinkedList<WebRequester>();

        /// <summary>
        /// 存储下载失败的请求者列表
        /// </summary>
        private readonly LinkedList<WebRequester> _requesterFailList = new LinkedList<WebRequester>();

        /// <summary>
        /// 存储正在下载的请求者列表
        /// </summary>
        private readonly LinkedList<WebRequester> _requesterLoadingList = new LinkedList<WebRequester>();

        //当前更新状态
        private IUpdateState currentUpdateState;
        //更新结束回调
        private UnityAction<E_UpdatePhase> overCallBack;
        //更新进度回调
        private UnityAction<long, long> proCallBack;
        //检查资源完整性进度回调
        private UnityAction<int, int> checkProgressCallBack;
        //更新阶段回调
        private UnityAction<E_UpdatePhase> phaseCallBack;
        //更新速度回调
        private UnityAction<long> speedCallBack;
        //当前进度
        private long cuurentProgress;
        //当前已下载总大小
        private long _currentDownloadTotalSizes;
        //更新状态字典
        private readonly Dictionary<E_UpdatePhase, IUpdateState> _updateStateDic = new Dictionary<E_UpdatePhase, IUpdateState>();
        //是否暂停下载
        private bool _isPauseDownload;

        /// <summary>
        /// 检查更新
        /// </summary>
        public void CheckUpdate(UnityAction<E_UpdatePhase> overCallBack, UnityAction<E_UpdatePhase> phaseCallBack, UnityAction<long, long> proCallBack, UnityAction<long> speedCallBack, UnityAction<int, int> checkProgressCallBack)
        {
            StartCoroutine(CheckUpate_Cor());

            IEnumerator CheckUpate_Cor()
            {
                ResetData();
                InitState();
                InitLocalPath();

                this.overCallBack = overCallBack;
                this.phaseCallBack = phaseCallBack;
                this.proCallBack = proCallBack;
                this.speedCallBack = speedCallBack;
                this.checkProgressCallBack = checkProgressCallBack;

                currentUpdateState = _updateStateDic[E_UpdatePhase.DownLoadRemoteCompareFile];
                currentUpdateState.Enter();
                while (!_isPauseDownload && currentUpdateState != null)
                {
                    yield return currentUpdateState.Execute();
                }
            }
        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        private void InitState()
        {
            _updateStateDic.Add(E_UpdatePhase.DownLoadRemoteCompareFile, new DownloadCompareFileState(this));
            _updateStateDic.Add(E_UpdatePhase.GetLocalCompareFile, new GetLocalCompareFileState(this));
            _updateStateDic.Add(E_UpdatePhase.CompareContrast, new CompareContrastState(this));
            _updateStateDic.Add(E_UpdatePhase.DownLoadAssets, new DownLoadAssetState(this));
            _updateStateDic.Add(E_UpdatePhase.CheckAssetsIntegrity, new CheckAssetIntegrityState(this));
            _updateStateDic.Add(E_UpdatePhase.Finished, new FinishState(this));
        }

        /// <summary>
        /// 初始化本地路径
        /// </summary>
        private void InitLocalPath()
        {
            //没有记录文件就创建记录文件
            if (!File.Exists(PathManager.GetAbLoadPath(FileUtility.RecordDefaultName)))
                File.Create(PathManager.GetAbLoadPath(FileUtility.RecordDefaultName)).Close();
        }

        /// <summary>
        /// 传递下载进度
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="total"></param>
        public void TransmitDownloadProgress(long delta, long total)
        {
            //累加当前下载量
            _currentDownloadTotalSizes += delta;
            //累加当前进度
            cuurentProgress += delta;
            //传递进度
            proCallBack?.Invoke(cuurentProgress, total);
        }

        /// <summary>
        /// 传递检查资源完整性进度
        /// </summary>
        /// <param name="current"></param>
        /// <param name="total"></param>
        public void TransmitCheckProgress(int current, int total)
        {
            checkProgressCallBack?.Invoke(current, total);
        }

        /// <summary>
        /// 传递下载速度
        /// </summary>
        /// <param name="delta"></param>
        public void TransmitDownloadSpeed()
        {
            speedCallBack?.Invoke(_currentDownloadTotalSizes);
            _currentDownloadTotalSizes = 0;
        }

        /// <summary>
        /// 完成更新
        /// </summary>
        /// <param name="isSuccess"></param>
        public void FinishUpdate(E_UpdatePhase updatePhase)
        {
            currentUpdateState.Exit();
            currentUpdateState = null;
            overCallBack?.Invoke(updatePhase);
        }

        /// <summary>
        /// 传递阶段
        /// </summary>
        /// <param name="updatePhase"></param>
        public void TransmitPhase(E_UpdatePhase updatePhase)
        {
            phaseCallBack?.Invoke(updatePhase);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="updatePhase"></param>
        public void ChangeState(E_UpdatePhase updatePhase)
        {
            if (_updateStateDic.TryGetValue(updatePhase, out IUpdateState state))
            {
                currentUpdateState?.Exit();
                currentUpdateState = state;
                currentUpdateState.Enter();
            }
            else
            {
                //目标状态未注册
                LogMgr.Log($"目标状态未注册；{updatePhase}");
            }
        }

        /// <summary>
        /// 获取总字节数
        /// </summary>
        /// <returns></returns>
        public long GetTotalDownLoadBytes()
        {
            //获取当前下载的总字节数
            return AssetBundleCompareInfo.GetTotalDownLoadBytes(_remoteABCompareInfoDic, _localDownedABInfoRecordDic);
        }

        /// <summary>
        /// 对比对比文件信息
        /// </summary>
        /// <returns>是否对比完成</returns>
        public IEnumerator CompareContrastFileInfo(UnityAction<bool> onCompared)
        {
            //遍历远端的AB包信息字典
            foreach (string abName in _remoteABCompareInfoDic.Keys)
            {
                //本地AB包信息字典存在相同AB包名就对比MD5码
                if (_localABCompareInfoDic.ContainsKey(abName))
                {
                    //不相等，说明远端是最新的，放入待下载字典
                    if (_localABCompareInfoDic[abName].Md5 != _remoteABCompareInfoDic[abName].Md5)
                    {
                        _waitDownLoadABDic.Add(abName, new AssetBundleRecordInfo(abName, _remoteABCompareInfoDic[abName].Md5, _remoteABCompareInfoDic[abName].Size));
                    }
                    //对比完同名AB包文件，就移除本地AB信息字典中对应内容
                    _localABCompareInfoDic.Remove(abName);
                }
                //没有就直接放入待下载字典中
                else
                    _waitDownLoadABDic.Add(abName, new AssetBundleRecordInfo(abName, _remoteABCompareInfoDic[abName].Md5, _remoteABCompareInfoDic[abName].Size));
            }

            //遍历本地AB信息字典中是否有剩余的内容，有就说明剩下的AB包是需要删除的资源，先删除后下载
            foreach (string abName in _localABCompareInfoDic.Keys)
            {
                //只能删除可读写文件夹的资源，流文件夹是默认资源不能删除，因为是只读的
                if (File.Exists(PathManager.GetAbLoadPath(abName)))
                    File.Delete(PathManager.GetAbLoadPath(abName));
            }

            //异步获取本地的AB包记录文件内容
            Task<string> task = File.ReadAllTextAsync(PathManager.GetAbLoadPath(FileUtility.RecordDefaultName));

            yield return task;

            string downedAllABinfo = "";
            if (task.IsCompletedSuccessfully)
            {
                downedAllABinfo = task.Result;
            }
            else
            {
                LogMgr.LogError("本地的AB包记录文件读取失败");
                onCompared?.Invoke(false);
                yield break;
            }

            //有内容说明之前更新中断过，需要进行对比，决定下载哪些资源和断点续传
            if (downedAllABinfo != null && downedAllABinfo.Length > 0)
            {
                try
                {
                    //自定义分割规则，与拼接规则一致即可
                    //切分内容，获取每条记录信息
                    string[] downedABInfos = downedAllABinfo.Split(";\n");
                    //遍历每条记录信息
                    for (int i = 0; i < downedABInfos.Length; i++)
                    {
                        //再次分割每条记录信息
                        string[] downedDetailABInfos = downedABInfos[i].Split(',');
                        //将临时记录文件的信息保存到容器中，用于后续对比
                        AssetBundleRecordInfo recordInfo = new AssetBundleRecordInfo(downedDetailABInfos[0], downedDetailABInfos[1], long.Parse(downedDetailABInfos[2]), bool.Parse(downedDetailABInfos[3]));
                        _localDownedABInfoRecordDic.Add(downedDetailABInfos[0], recordInfo);
                    }
                }
                catch
                {
                    LogMgr.LogError("拆分记录信息失败，拼接与拆分规则不一致");
                    onCompared?.Invoke(false);
                    yield break;
                }

                //待移除的AB包文件列表，存储不需要下载的AB包名
                List<string> waitRemoveABFileList = new List<string>();

                //待下载字典与临时记录文件字典进行对比
                //遍历待下载字典
                foreach (string abName in _waitDownLoadABDic.Keys)
                {
                    //如果记录的文件没有AB包名，说明是之前没有下载过的，也要下载。
                    if (!_localDownedABInfoRecordDic.ContainsKey(abName))
                        continue;

                    //如果临时记录文件有该AB包名，说明之前下载过该AB包，需判断是否是最新的
                    //若不等于说明待下载字典的资源是最新的，就要下载，覆盖上次旧的AB包资源
                    if (_localDownedABInfoRecordDic[abName].Md5 != _waitDownLoadABDic[abName].Md5)
                        continue;

                    //临时记录文件的AB包MD5码等于待下载字典的该AB包的MD5码，说明记录文件的AB包的资源是最新的
                    //判断如果下载完成就不用下载了
                    if (_localDownedABInfoRecordDic[abName].IsSuccess)
                    {
                        //记录进待移除的AB包文件列表
                        waitRemoveABFileList.Add(abName);
                    }
                    //若上次下载未完成, 也要继续接着下载
                    else
                    {
                        _waitDownLoadABDic[abName].DownloadedBytes = _localDownedABInfoRecordDic[abName].DownloadedBytes;
                    }
                }

                //移除待下载字典中不用下载的AB包名
                for (int i = 0; i < waitRemoveABFileList.Count; i++)
                {
                    _waitDownLoadABDic.Remove(waitRemoveABFileList[i]);
                }
            }

            onCompared?.Invoke(true);
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="overCallBack">下载结束回调</param>
        /// <param name="proCallBack">下载进度回调</param>
        public IEnumerator DownLoadAssets(UnityAction<bool> overCallBack, UnityAction<long> proCallBack)
        {
            //记录请求的服务器地址
            string serverIp = GlobalSettings.Instance.ResServerIp;
            //遍历待下载字典
            foreach (var info in _waitDownLoadABDic.Values)
            {
                //创建请求者
                WebRequester webRequester = new WebRequester(serverIp, info.AbName, true, info.AbName, info.Md5);
                //存储待下载的请求者
                _requesterWaitList.AddLast(new LinkedListNode<WebRequester>(webRequester));
            }

            //记录最大并发数
            int maxConcurrencyNum = GlobalSettings.Instance.MaxConcurrencyNum;

            /*
             * 暂停优先级最高，未暂停则继续下载，暂停则退出下载；
             * 待下载链表中有请求，或者有正在下载的资源,则继续下载；
             * 待下载链表、正在下载链表无请求，但下载失败链表有请求，则暂停下载；
            */
            while (!_isPauseDownload && (_requesterWaitList.Count > 0 || _requesterLoadingList.Count > 0 ||
                !(_requesterWaitList.Count == 0 && _requesterLoadingList.Count == 0 && _requesterFailList.Count >= 0)))
            {
                //正在下载的资源数小于最大并发数且有要下载的内容，才去下载资源
                while (_requesterLoadingList.Count < maxConcurrencyNum && _requesterWaitList.Count > 0)
                {
                    //取出第一个请求
                    LinkedListNode<WebRequester> webRequesterNode = _requesterWaitList.First;
                    //从待下载列表中移除
                    _requesterWaitList.RemoveFirst();
                    //放入正在下载列表
                    _requesterLoadingList.AddLast(webRequesterNode);
                    //获取节点的请求者
                    WebRequester webRequester = webRequesterNode.Value;
                    //调用请求者的下载方法
                    webRequester.DownLoad(PathManager.GetAbLoadPath(webRequester.FileName), (isOver) =>
                    {
                        //无论是否下载成功，都是下载结束，从正在下载的列表中移除
                        _requesterLoadingList.Remove(webRequesterNode);
                        //下载成功
                        if (isOver)
                        {
                            LogMgr.Log($"下载成功：{webRequester.FileName}");
                            //获取文件信息
                            FileInfo fileInfo = new FileInfo(PathManager.GetAbLoadPath(webRequester.FileName));
                            //构建记录信息对象
                            AssetBundleRecordInfo recordInfo = new AssetBundleRecordInfo(webRequester.FileName, _remoteABCompareInfoDic[webRequester.FileName].Md5, fileInfo.Length);
                            //更新记录文件信息
                            UpdateRecordFile(recordInfo);
                        }
                        //下载失败，添加到下载失败的列表中
                        else
                        {
                            _requesterFailList.AddLast(webRequesterNode);
                        }
                        //分发每帧下载字节数
                    }, proCallBack);

                    yield return null;
                }

                //有正在下载的资源
                if (_requesterLoadingList.Count > 0)
                {
                    //等待正在下载数小于最大并发数或者没有要下载的资源
                    yield return new WaitUntil(() => _requesterLoadingList.Count < maxConcurrencyNum || _requesterWaitList.Count == 0);
                }

                // 处理下载失败的任务
                if (_requesterFailList.Count > 0)
                {
                    //获取第一个节点
                    LinkedListNode<WebRequester> failedRequesterNode = _requesterFailList.First;
                    //取出节点的请求者
                    WebRequester failedRequester = failedRequesterNode.Value;

                    while (failedRequesterNode != null)
                    {
                        //有剩余下载次数
                        if (failedRequester.CurrentRetryCount != 0)
                        {
                            _requesterFailList.RemoveFirst();
                            _requesterWaitList.AddLast(failedRequesterNode);
                            //重试次数减少
                            failedRequester.SubRetryCount();
                        }

                        //当前临时节点等于其下一个节点
                        failedRequesterNode = failedRequesterNode.Next;
                    }
                }

                yield return null;
            }

            LogMgr.Log("下载结束");

            //判断全部是否下载成功
            bool isAllSuccess = true;
            foreach (var info in _localDownedABInfoRecordDic.Values)
            {
                if (!info.IsSuccess)
                {
                    isAllSuccess = false;
                    break;
                }
                yield return null;
            }

            //AB包下载结束回调
            overCallBack?.Invoke(isAllSuccess);
        }

        /// <summary>
        /// 检查资源完整性
        /// </summary>
        /// <param name="overCallBack">结束回调</param>
        public IEnumerator CheckAssetsIntegrity(UnityAction<bool> overCallBack, UnityAction<int, int> onCheckProgress)
        {
            //遍历下载失败的请求者列表
            LinkedListNode<WebRequester> node = _requesterFailList.First;
            while (node != null)
            {
                //获取文件信息
                FileInfo fileInfo = new FileInfo(PathManager.GetAbLoadPath(node.Value.FileName));
                //构造记录信息对象
                AssetBundleRecordInfo recordInfo = new AssetBundleRecordInfo(node.Value.AbName, node.Value.MD5, fileInfo.Length);
                //更新记录文件信息
                UpdateRecordFile(recordInfo);
                node = node.Next;
            }

            int currentProgress = 0;
            //获取当前更新的所有AB包
            foreach (string abName in _localDownedABInfoRecordDic.Keys)
            {
                ++currentProgress;
                onCheckProgress?.Invoke(currentProgress, _localDownedABInfoRecordDic.Count);
                yield return null;
                if (_remoteABCompareInfoDic[abName].Size == _localDownedABInfoRecordDic[abName].DownloadedBytes &&
                    _remoteABCompareInfoDic[abName].Md5 == _localDownedABInfoRecordDic[abName].Md5)
                {
                    continue;
                }
                _incompleteABList.Add(abName);
            }

            //根据_incompleteABList判断资源是否完整
            overCallBack?.Invoke(_incompleteABList.Count == 0);
        }

        /// <summary>
        /// 更新临时记录文件
        /// </summary>
        /// <param name="recordInfo">下载结束的构建信息对象</param>
        private void UpdateRecordFile(AssetBundleRecordInfo recordInfo)
        {
            //记录到AB包到临时记录文件的字典中，更新字典中的MD5码, 已下载的字节数和是否下载完成标识或添加新的AB包信息
            if (_localDownedABInfoRecordDic.TryGetValue(recordInfo.AbName, out var bundleRecordInfo))
            {
                bundleRecordInfo.Md5 = recordInfo.Md5;
                bundleRecordInfo.DownloadedBytes = recordInfo.DownloadedBytes;
                bundleRecordInfo.IsSuccess = recordInfo.DownloadedBytes == _remoteABCompareInfoDic[recordInfo.AbName].Size;
            }
            else
            {
                recordInfo.IsSuccess = recordInfo.DownloadedBytes == _remoteABCompareInfoDic[recordInfo.AbName].Size;
                _localDownedABInfoRecordDic.Add(recordInfo.AbName, recordInfo);
            }
        }

        /// <summary>
        /// 下载远端AB包对比文件
        /// </summary>
        /// <param name="overCallBack">下载结束回调</param>
        public IEnumerator DownloadCompareFile(UnityAction<bool> overCallBack)
        {
            //创建web请求对象
            WebRequester webRequester = new WebRequester(GlobalSettings.Instance.ResServerIp, FileUtility.CompareFileDefaultName, false, string.Empty, string.Empty);

            bool isFinish = false;
            bool isSuccess = false;

            for (int i = 0; i < GlobalSettings.Instance.ReDownloadCompareFileMaxNum; i++)
            {
                //是否完成
                isFinish = false;
                //下载
                webRequester.DownLoad(PathManager.GetAbLoadPath(FileUtility.TempCompareFileDefaultName), (isOver) =>
                {
                    //无论成功与否，都认为下载完成
                    isFinish = true;
                    //下载成功
                    if (isOver)
                    {
                        isSuccess = true;
                    }
                });

                //没有完成就等待完成
                yield return new WaitUntil(() => isFinish);

                //成功就退出协程
                if (isSuccess)
                {
                    overCallBack?.Invoke(isSuccess);
                    yield break;
                }
            }

            if (!isSuccess)
                //循环结束都没有成功，传递false
                overCallBack?.Invoke(isSuccess);
        }

        /// <summary>
        /// 读取本地AB包对比文件
        /// </summary>
        /// <param name="overCallBack">读取结束回调</param>
        public IEnumerator GetLocalCompareFileInfo(UnityAction<bool> overCallBack)
        {
            //可读写路径有本地对比文件，说明已经更新过了，通过UnityWebRequest获取本地的对比文件
            if (File.Exists(PathManager.GetAbLoadPath(FileUtility.CompareFileDefaultName)))
            {
                //通过UnityWebRequest获取本地可读写路径的对比文件需要添加文件协议
                yield return GetLocalCompareFileInfo_Cor("file:///" + PathManager.GetAbLoadPath(FileUtility.CompareFileDefaultName));
            }
            //流文件夹有对比文件，说明是有默认资源且是第一次更新，通过UnityWebRequest获取本地的对比文件
            else if (File.Exists(Application.streamingAssetsPath + "/" + FileUtility.CompareFileDefaultName))
            {
                //根据不同的平台判断是否需要添加文件协议
                string path =
#if UNITY_ANDROID
                    Application.streamingAssetsPath + "/";
#else
                    "file:///" + Application.streamingAssetsPath + "/";
#endif
                yield return GetLocalCompareFileInfo_Cor(path + FileUtility.CompareFileDefaultName);
            }
            else
            {
                //说明没有默认资源，且是第一次更新，不用获取
                overCallBack?.Invoke(true);
            }

            //本地协程函数
            IEnumerator GetLocalCompareFileInfo_Cor(string localFilePath)
            {
                //获取本地AB包对比文件
                UnityWebRequest req = UnityWebRequest.Get(localFilePath);
                yield return req.SendWebRequest();

                //获取成功才去解析
                if (req.result == UnityWebRequest.Result.Success)
                {
                    //解析本地AB包对比文件
                    AnalyzeCompareFileInfo(req.downloadHandler.text, E_FileAnalyzeType.Local);
                    overCallBack?.Invoke(true);
                }
                else
                {
                    //抛出错误
                    LogMgr.LogError($"本地AB包对比文件获取失败：{req.result}-{req.error}");
                    //获取失败，执行回调
                    overCallBack?.Invoke(false);
                }
            }
        }

        /// <summary>
        /// 解析远端AB包对比文件信息
        /// </summary>
        /// <returns>是否获取成功</returns>
        public bool AnalyzeRemoteCompareFileInfo()
        {
            //本地有该文件才去读取
            if (File.Exists(PathManager.GetAbLoadPath(FileUtility.TempCompareFileDefaultName)))
            {
                //读取已经下载的AB包临时对比文件
                string abInfo = File.ReadAllText(PathManager.GetAbLoadPath(FileUtility.TempCompareFileDefaultName));
                //解析文件信息到远端下载的AB包信息字典
                AnalyzeCompareFileInfo(abInfo, E_FileAnalyzeType.Remote);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 解析AB包对比文件信息
        /// </summary>
        /// <param name="abInfo">AB包信息</param>
        /// <param name="localOrRemoteDic">存储远端或本地AB包信息的资源</param>
        private void AnalyzeCompareFileInfo(string abInfo, E_FileAnalyzeType analyzeType)
        {
            //拆分为单个AB包信息
            string[] abInfos = abInfo.Split('\n');
            string[] abs = null;
            for (int i = 0; i < abInfos.Length; i++)
            {
                //将单个AB包信息再次拆分为关键信息(名称、大小、MD5)
                abs = abInfos[i].Split('=');
                //存储AB包拆分信息
                if(analyzeType == E_FileAnalyzeType.Local)
                    _localABCompareInfoDic.Add(abs[0], new AssetBundleCompareInfo(abs[0], abs[1], abs[2]));
                else
                    _remoteABCompareInfoDic.Add(abs[0], new AssetBundleCompareInfo(abs[0], abs[1], abs[2]));
            }
        }

        /// <summary>
        /// 写入记录文件
        /// </summary>
        public void WriteAllRecordFile()
        {
            StringBuilder sb = new StringBuilder();
            //通过字典存储的内容自定义拼接格式
            foreach (string downedABName in _localDownedABInfoRecordDic.Keys)
            {
                //自定义拼接规则，与读取规则一致即可
                sb.Append(downedABName + "," + _localDownedABInfoRecordDic[downedABName].Md5 + "," + _localDownedABInfoRecordDic[downedABName].DownloadedBytes + "," + _localDownedABInfoRecordDic[downedABName].IsSuccess);
                sb.Append(";\n");
            }

            if (sb.Length == 0)
            {
                return;
            }

            //改变文件内容
            File.WriteAllText(PathManager.GetAbLoadPath(FileUtility.RecordDefaultName), sb.ToString()[..(sb.Length - 2)]);
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public void PauseDownload()
        {
            //暂停下载
            _isPauseDownload = true;

            //若在下载中强制退出，则中断所有正在下载的请求
            LinkedListNode<WebRequester> node = _requesterLoadingList.First;
            while (node != null)
            {
                node.Value.Abort();
                node.Value.Dispose();
                node = node.Next;
            }

            //用临时列表记录所有请求者
            List<WebRequester> tempList = new List<WebRequester>();
            tempList.AddRange(_requesterFailList);
            tempList.AddRange(_requesterLoadingList);
            tempList.AddRange(_requesterWaitList);

            //遍历临时列表
            for (int i = 0; i < tempList.Count; i++)
            {
                //若该路径不存在，说明还没有开始下载，不用记录，只需记录正在下载的和下载失败和下载失败后等待下载的
                if (!File.Exists(PathManager.GetAbLoadPath(tempList[i].AbName)))
                    continue;
                //获取文件信息
                FileInfo fileInfo = new FileInfo(PathManager.GetAbLoadPath(tempList[i].AbName));
                //构建记录文件
                AssetBundleRecordInfo recordInfo = new AssetBundleRecordInfo(tempList[i].AbName, tempList[i].MD5, fileInfo.Length);
                //更新记录文件信息
                UpdateRecordFile(recordInfo);
            }

            WriteAllRecordFile();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        private void ResetData()
        {
            //清除上次下载残留数据
            _remoteABCompareInfoDic.Clear();
            _localABCompareInfoDic.Clear();
            _waitDownLoadABDic.Clear();
            _incompleteABList.Clear();
            _localDownedABInfoRecordDic.Clear();
            _requesterWaitList.Clear();
            _requesterLoadingList.Clear();
            _requesterFailList.Clear();
            _updateStateDic.Clear();

            overCallBack = null;
            phaseCallBack = null;
            proCallBack = null;
            cuurentProgress = 0;
            _isPauseDownload = false;
        }

        private void OnApplicationQuit()
        {
            PauseDownload();
        }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPauseDownload => _isPauseDownload;
    }
}
