using Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// AB包更新上下文
/// </summary>
public class ABUpdateContext
{
    /// <summary>
    /// 存储远端下载的AB包信息集合
    /// </summary>
    public ABPackageCollection RemotePackageCollection { get; private set; }

    /// <summary>
    /// 存储本地读取的AB包信息集合
    /// </summary>
    public ABPackageCollection LocalPackageCollection { get; private set; }

    /// <summary>
    /// 存储本地的缓存已下载AB包信息集合
    /// </summary>
    public ABPackageCacheCollection CachePackageCollection {  get; private set; }

    /// <summary>
    /// 存储待下载的AB包信息集合
    /// </summary>
    public ABPackageCacheCollection WaitDownloadCollection { get; private set; }

    /// <summary>
    /// 存储不完整的AB包列表
    /// </summary>
    private readonly List<string> _incompleteABList = new List<string>();

    /// <summary>
    /// 存储待下载请求者列表
    /// </summary>
    private readonly LinkedList<ABWebRequester> _requesterWaitList = new LinkedList<ABWebRequester>();

    /// <summary>
    /// 存储下载失败的请求者列表
    /// </summary>
    private readonly LinkedList<ABWebRequester> _requesterFailList = new LinkedList<ABWebRequester>();

    /// <summary>
    /// 存储正在下载的请求者列表
    /// </summary>
    private readonly LinkedList<ABWebRequester> _requesterLoadingList = new LinkedList<ABWebRequester>();

    /// <summary>
    /// 是否暂停下载
    /// </summary>
    public bool IsPauseDownload { get; set; }

    /// <summary>
    /// 等待链表中的数量
    /// </summary>
    public int WaitListCount => _requesterWaitList.Count;

    /// <summary>
    /// 下载链表中的数量
    /// </summary>
    public int LoadListCount => _requesterLoadingList.Count;

    /// <summary>
    /// 失败链表中的数量
    /// </summary>
    public int FailListCount => _requesterFailList.Count;

    /// <summary>
    /// 不完整链表中的数量
    /// </summary>
    public int IncompleteListCount => _requesterLoadingList.Count;

    /// <summary>
    /// 更新进度事件
    /// </summary>
    public event UnityAction<long, long> OnProgress;

    /// <summary>
    /// 检查资源完整性进度事件
    /// </summary>
    public event UnityAction<int, int> OnCheckProgress;

    /// <summary>
    /// 更新阶段事件
    /// </summary>
    public event UnityAction<E_UpdatePhase> OnUpdatePhase;

    /// <summary>
    /// 更新速度事件
    /// </summary>
    public event UnityAction<long> OnUpdateSpeed;

    /// <summary>
    /// 更新完成事件
    /// </summary>
    public event UnityAction OnUpdateFinish;

    //当前已下载字节数
    private long cuurentDownloadedBytes;

    //当前已下载总大小
    private long _currentDownloadTotalSizes;

    public ABUpdateContext()
    {
        RemotePackageCollection = new ABPackageCollection();
        LocalPackageCollection = new ABPackageCollection();
        WaitDownloadCollection = new ABPackageCacheCollection();
        CachePackageCollection = new ABPackageCacheCollection();
    }

    /// <summary>
    /// 添加请求者到等待链表
    /// </summary>
    /// <param name="requester"></param>
    public void AddRequesterToWait(ABWebRequester requester)
    {
        _requesterWaitList.AddLast(new LinkedListNode<ABWebRequester>(requester));
    }

    /// <summary>
    /// 添加请求者到下载链表
    /// </summary>
    /// <param name="requester"></param>
    public void AddRequesterToLoad(ABWebRequester requester)
    {
        _requesterLoadingList.AddLast(new LinkedListNode<ABWebRequester>(requester));
    }

    /// <summary>
    /// 添加请求者到失败链表
    /// </summary>
    /// <param name="requester"></param>
    public void AddRequesterToFail(ABWebRequester requester)
    {
        _requesterFailList.AddLast(new LinkedListNode<ABWebRequester>(requester));
    }

    /// <summary>
    /// 添加AB包名到不完整链表
    /// </summary>
    /// <param name="abName"></param>
    public void AddABNameToIncomplete(string abName)
    {
        _incompleteABList.Add(abName);
    }

    /// <summary>
    /// 获取第一个请求者
    /// </summary>
    /// <returns></returns>
    public ABWebRequester GetFirstRequester()
    {
        var requester = _requesterWaitList.First;
        _requesterWaitList.RemoveFirst();
        return requester.Value;
    }

    /// <summary>
    /// 从下载链表中移除请求者
    /// </summary>
    /// <param name="requester"></param>
    /// <returns></returns>
    public bool RemoveRequesterFromLoad(ABWebRequester requester)
    {
        return _requesterLoadingList.Remove(requester);
    }

    /// <summary>
    /// 处理下载失败的请求者
    /// </summary>
    public void HandleFailReqeuster()
    {
        if (FailListCount > 0)
        {
            // 获取第一个节点
            LinkedListNode<ABWebRequester> failedRequesterNode = _requesterFailList.First;
            // 取出节点的请求者
            ABWebRequester failedRequester = failedRequesterNode.Value;
            while (failedRequesterNode != null)
            {
                // 有剩余下载次数
                if (failedRequester.CurrentRetryCount != 0)
                {
                    _requesterFailList.RemoveFirst();
                    _requesterWaitList.AddLast(failedRequesterNode);
                    // 重试次数减少
                    failedRequester.SubRetryCount();
                }
                // 当前临时节点等于其下一个节点
                failedRequesterNode = failedRequesterNode.Next;
            }
        }
    }
    
    /// <summary>
    /// 更新阶段
    /// </summary>
    /// <param name="updatePhase"></param>
    public void UpdatePhase(E_UpdatePhase updatePhase)
    {
        if (IsPauseDownload)
        {
            return;
        }

        OnUpdatePhase?.Invoke(updatePhase);
    }

    /// <summary>
    /// 更新下载进度
    /// </summary>
    /// <param name="bytesPerFrame"></param>
    /// <param name="downLoadTotalBytes"></param>
    public void UpdateProgress(long bytesPerFrame, long downLoadTotalBytes)
    {
        // 记录当前下载量，用于计算下载速度
        _currentDownloadTotalSizes += bytesPerFrame;
        // 累加当前进度
        cuurentDownloadedBytes += bytesPerFrame;
        //传递进度
        OnProgress?.Invoke(cuurentDownloadedBytes, downLoadTotalBytes);
    }

    /// <summary>
    /// 更新检查资源完整性进度
    /// </summary>
    /// <param name="current"></param>
    /// <param name="total"></param>
    public void UpdateCheckProgress(int current, int total)
    {
        OnCheckProgress?.Invoke(current, total);
    }

    /// <summary>
    /// 更新下载速度
    /// </summary>
    /// <param name="delta"></param>
    public void UpdateSpeed()
    {
        if (OnUpdateSpeed == null)
        {
            LogManager.Log($"{nameof(OnUpdateSpeed)}事件为空");
        }

        OnUpdateSpeed?.Invoke(_currentDownloadTotalSizes);
        _currentDownloadTotalSizes = 0;
    }

    /// <summary>
    /// 更新完成
    /// </summary>
    public void UpdateFinish()
    {
        OnUpdateFinish?.Invoke();
    }

    /// <summary>
    /// 从失败链表中获取构建的缓存信息
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ABPackageCacheInfo> GetCacheInfosFromFail()
    {
        // 遍历下载失败的请求者列表
        LinkedListNode<ABWebRequester> node = _requesterFailList.First;
        while (node != null)
        {
            // 获取文件信息
            FileInfo fileInfo = new FileInfo(PathUtility.GetAbLoadPath(node.Value.FileName));
            // 构造记录信息对象
            ABPackageCacheInfo cacheInfo = new ABPackageCacheInfo(node.Value.AbName, node.Value.MD5, fileInfo.Length);
            node = node.Next;
            yield return cacheInfo;
        }
    }

    /// <summary>
    /// 取消下载
    /// </summary>
    public async Task CancelDownload()
    {
        // 暂停下载
        IsPauseDownload = true;
        // 若在下载中强制退出，则中断所有正在下载的请求
        LinkedListNode<ABWebRequester> node = _requesterLoadingList.First;
        while (node != null)
        {
            node.Value.Abort();
            node.Value.Dispose();
            node = node.Next;
        }

        // 用临时列表记录所有请求者
        List<ABWebRequester> tempList = new List<ABWebRequester>();
        tempList.AddRange(_requesterFailList);
        tempList.AddRange(_requesterLoadingList);
        tempList.AddRange(_requesterWaitList);

        //遍历临时列表
        for (int i = 0; i < tempList.Count; i++)
        {
            // 若该路径不存在，说明还没有开始下载，不用记录，只需记录正在下载的和下载失败和下载失败后等待下载的
            if (!File.Exists(PathUtility.GetAbLoadPath(tempList[i].AbName)))
            {
                continue;
            }

            // 获取文件信息
            FileInfo fileInfo = new FileInfo(PathUtility.GetAbLoadPath(tempList[i].AbName));
            // 构建缓存文件
            ABPackageCacheInfo cacheInfo = new ABPackageCacheInfo(tempList[i].AbName, tempList[i].MD5, fileInfo.Length);
            // 更新缓存文件信息
            UpdateCacheFile(cacheInfo);
        }

        await WriteCacheFile();
    }

    /// <summary>
    /// 更新缓存文件
    /// </summary>
    /// <param name="cacheInfo">下载结束的构建信息对象</param>
    public void UpdateCacheFile(ABPackageCacheInfo cacheInfo)
    {
        // 记录到AB包到缓存记录文件的集合中，更新集合中的MD5码, 已下载的字节数和是否下载完成标识或添加新的AB包信息
        if (CachePackageCollection.TryGetValue(cacheInfo.AbName, out var aBPackageCacheInfo))
        {
            aBPackageCacheInfo.Md5 = cacheInfo.Md5;
            aBPackageCacheInfo.DownloadedBytes = cacheInfo.DownloadedBytes;
            aBPackageCacheInfo.IsSuccess = cacheInfo.DownloadedBytes == RemotePackageCollection[cacheInfo.AbName].Size;
        }
        else
        {
            cacheInfo.IsSuccess = cacheInfo.DownloadedBytes == RemotePackageCollection[cacheInfo.AbName].Size;
            CachePackageCollection.TryAdd(cacheInfo.AbName, cacheInfo);
        }
    }

    /// <summary>
    /// 写入缓存文件
    /// </summary>
    public async Task WriteCacheFile()
    {
        await JsonManager.Instance.SaveToJsonAsync(CachePackageCollection, PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName));
    }

    /// <summary>
    /// 重置数据
    /// </summary>
    public void ResetData()
    {
        // 清除上次下载残留数据
        RemotePackageCollection.Clear();
        LocalPackageCollection.Clear();
        WaitDownloadCollection.Clear();
        CachePackageCollection.Clear();

        _incompleteABList.Clear();
        _requesterWaitList.Clear();
        _requesterLoadingList.Clear();
        _requesterFailList.Clear();

        OnProgress = null;
        OnCheckProgress = null;
        OnUpdatePhase = null;
        OnUpdateSpeed = null;

        cuurentDownloadedBytes = 0;
        IsPauseDownload = false;
    }
}
