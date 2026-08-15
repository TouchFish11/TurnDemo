using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Binary;
using Core.UI;
using HotUpdate.Common.Config;
using UnityEngine.UI;

namespace HotUpdate.UI.Dialogue
{
    /// <summary>
    /// 剧情回顾视图类
    /// 负责展示历史对话内容、缓存对话信息、回收对话UI对象等核心逻辑
    /// 继承自基础UI行为类，并实现剧情回顾视图接口
    /// </summary>
    public class StoryReviewView : UIBehaviourBase
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IBinaryDataManager _binaryDataManager;
        
        /// <summary>
        /// 对话回顾列表的滚动容器（通过依赖注入赋值）
        /// </summary>
        [InjectUI] private ScrollRect svReview;
        
        /// <summary>
        /// 对话回顾UI对象的缓存
        /// </summary>
        private readonly List<UIBehaviourBase> _reviewUIs = new();

        /// <summary>
        /// 子视图关闭时的事件回调
        /// 外部可订阅该事件以处理视图关闭后的逻辑
        /// </summary>
        public event Action OnSubViewClosed;
        
        /// <summary>
        /// 按钮点击事件处理方法
        /// 重写父类方法，处理当前视图内的按钮交互
        /// </summary>
        /// <param name="btnName">点击的按钮名称（与UI配置的按钮标识一致）</param>
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    // 触发子视图关闭事件，通知外部处理
                    OnSubViewClosed?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// 缓存对话信息到历史集合
        /// 供外部调用，将新的对话数据加入待展示列表
        /// </summary>
        /// <param name="historicalInfos"></param>
        public async Task Review(List<IReviewInfo> historicalInfos)
        {
            // 遍历所有历史对话信息
            foreach (var reviewInfo in historicalInfos)
            {
                switch (reviewInfo.ReviewType)
                {
                    case IReviewInfo.EReviewType.Dialogue:
                        // 从资源包异步加载对话回顾UI预制体，并挂载到滚动容器的内容节点下
                        var dialogueReviewUI = await _objectSpawner.SpawnAsync<DialogueReviewUI>(AssetKeys.DialogueReviewUI, svReview.content);
                        // 从二进制数据管理器中获取NPC配置容器，根据说话者ID查询NPC信息
                        var npcInfo = _binaryDataManager.GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[((DialogueInfo)reviewInfo).f_speakerId];
                        // 初始化对话UI的显示内容（说话者名称 + 对话文本）
                        dialogueReviewUI.Init(npcInfo.f_speakerName, reviewInfo.GetViewText());
                        // 将实例化的UI对象加入缓存集合，便于后续回收
                        _reviewUIs.Add(dialogueReviewUI);
                        break;
                    case IReviewInfo.EReviewType.Branch:
                        // 从资源包异步加载对话回顾UI预制体，并挂载到滚动容器的内容节点下
                        var branchReviewUI = await _objectSpawner.SpawnAsync<BranchReviewUI>(AssetKeys.DialogueReviewUI, svReview.content);
                        // 初始化对话UI的显示内容（说话者名称 + 对话文本）
                        branchReviewUI.Init(reviewInfo.GetViewText());
                        _reviewUIs.Add(branchReviewUI);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// 清理所有对话回顾UI对象
        /// 将UI对象归还到对象池，避免重复创建销毁造成性能损耗
        /// </summary>
        private void ClearReviewUI()
        {
            // 遍历所有已实例化的对话UI，归还到对象池
            foreach (var reviewUI in _reviewUIs)
            {
                switch (reviewUI)
                {
                    case DialogueReviewUI dialogueReviewUI:
                        _objectSpawner.Release(dialogueReviewUI);
                        break;
                    case BranchReviewUI branchReviewUI:
                        _objectSpawner.Release(branchReviewUI);
                        break;
                }
            }
            // 清空UI缓存集合，避免内存泄漏
            _reviewUIs.Clear();
        }

        /// <summary>
        /// 视图禁用时的生命周期方法
        /// 重写父类方法，清理对话UI对象，释放资源
        /// </summary>
        protected override void OnDisable()
        {
            // 视图隐藏时清理所有对话UI
            ClearReviewUI();
        }

        protected override void OnDestroy()
        {
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}