using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Serialize.Binary;
using Core.UI;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;

using UnityEngine.UI;

namespace HotUpdate.Game.Dialogue.UI
{
    /// <summary>
    /// 剧情回顾视图类
    /// 负责展示历史对话内容、缓存对话信息、回收对话UI对象等核心逻辑
    /// 继承自基础UI行为类，并实现剧情回顾视图接口
    /// </summary>
    public class StoryReviewView : UIBehaviourBase
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        /// <summary>
        /// 对话回顾列表的滚动容器（通过依赖注入赋值）
        /// </summary>
        [InjectUI] private ScrollRect svReview;
        
        /// <summary>
        /// 存储历史对话信息的集合
        /// 用于缓存需要展示的所有对话数据
        /// </summary>
        private readonly List<DialogueInfo> historicalDialogueInfos = new();
        
        /// <summary>
        /// 对话回顾UI对象的缓存集合
        /// 用于管理已实例化的对话UI，方便后续回收
        /// </summary>
        private readonly List<PoolObject<DialogueReviewUI>> dialogueReviewUIs = new();

        /// <summary>
        /// 子视图关闭时的事件回调
        /// 外部可订阅该事件以处理视图关闭后的逻辑
        /// </summary>
        public event Action OnSubViewClosed;

        /// <summary>
        /// 视图启用时的生命周期方法
        /// 重写父类方法，异步加载并展示历史对话
        /// </summary>
        protected override async void OnEnable()
        {
            try
            {
                // 异步执行对话展示逻辑
                await Show();
            }
            catch (Exception e)
            {
                // 记录启用过程中的异常日志，便于问题定位
                Logger.LogError($"{GetType().Name}.{nameof(OnEnable)}： {e.Message}");
            }
        }

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
        /// 展示历史对话内容的核心方法
        /// 遍历缓存的对话信息，实例化对话UI并初始化显示内容
        /// </summary>
        /// <returns>异步任务对象</returns>
        private async System.Threading.Tasks.Task Show()
        {
            // 遍历所有缓存的历史对话信息
            foreach (var dialogueInfo in historicalDialogueInfos)
            {
                // 从资源包异步加载对话回顾UI预制体，并挂载到滚动容器的内容节点下
                var poolObject = await _objectSpawner.SpawnAsync<DialogueReviewUI>(AssetKeys.DialogueReviewUI, svReview.content);
                
                // 从二进制数据管理器中获取NPC配置容器，根据说话者ID查询NPC信息
                var npcInfo = DIContainer.GetInstance<IBinaryDataManager>()
                    .GetConfig<NpcInfoContainer>(EConfigLoadType.Excel)
                    .dataDic[dialogueInfo.f_speakerId];
                
                // 初始化对话UI的显示内容（说话者名称 + 对话文本）
                poolObject.Obj.Init(npcInfo.f_speakerName, dialogueInfo.f_dialgueText);
                
                // 将实例化的UI对象加入缓存集合，便于后续回收
                dialogueReviewUIs.Add(poolObject);
            }
        }

        /// <summary>
        /// 缓存对话信息到历史集合
        /// 供外部调用，将新的对话数据加入待展示列表
        /// </summary>
        /// <param name="dialogueInfo">待缓存的对话信息对象</param>
        public void CacheDialogueInfo(DialogueInfo dialogueInfo)
        {
            historicalDialogueInfos.Add(dialogueInfo);
        }

        /// <summary>
        /// 清理所有对话回顾UI对象
        /// 将UI对象归还到对象池，避免重复创建销毁造成性能损耗
        /// </summary>
        private void ClearReviewUI()
        {
            // 遍历所有已实例化的对话UI，归还到对象池
            foreach (var dialogueReviewUI in dialogueReviewUIs)
            {
                dialogueReviewUI.Collect();
            }
            // 清空UI缓存集合，避免内存泄漏
            dialogueReviewUIs.Clear();
            _objectSpawner.Dispose();
            _objectSpawner = null;
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
    }
}