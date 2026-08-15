using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.UI.ViewController;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config;
using HotUpdate.Game.Dialogue;
using HotUpdate.Game.Dialogue.Datas;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Dialogue
{
    /// <summary>
    /// 对话控制器核心类
    /// 处理对话界面的交互逻辑、对话内容展示、分支选项设置等核心功能
    /// </summary>
    public class DialogueController : UIController<DialogueView>, IBlockOperation
    {
        private static readonly WaitForSeconds s_waitForSeconds0_25 = new(0.25f);

        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IDialogueManager _dialogueManager;
        [Inject] private ObjectSpawner _objectSpawner;
        
        // 对话提示动画的协程引用，用于停止协程
        private Coroutine dialogueTipCor;
        // 缓存历史对话记录
        private readonly List<IReviewInfo> historicalDialogueInfos = new();
        
        /// <summary>
        /// 对话加载提示文本（渐变显示的省略号）
        /// </summary>
        private const string DialogueTip = "...";
        
        /// <summary>
        /// 对话默认提示文本（继续按钮提示）
        /// </summary>
        private const string DefaultTip = "V";
        
        /// <summary>
        /// 对话是否正在播放中
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// 对话是否开启自动播放模式
        /// </summary>
        public bool IsAutoPlay { get; set; }

        /// <summary>
        /// 对话弹窗是否处于激活状态
        /// </summary>
        public bool IsActiveBox { get; set; }

        public bool BlockOperation { get; } = true;
        
        protected override bool IsCursorVisible { get; set; } = true;
        
        /// <summary>
        /// 控制器初始化方法（异步）
        /// 注册对话管理器的事件监听、初始化故事回顾子视图
        /// </summary>
        /// <returns>异步任务</returns>
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            // 获取对话管理器实例，注册单条对话开始/结束事件
            _dialogueManager.OnSingleDialogueStart += OnSingleDialogueStart;
            _dialogueManager.OnSingleDialogueEnd += OnSingleDialogueEnd;
            _dialogueManager.OnSelectDialogueBranch += OnSelectDialogueBranch;
            return Task.CompletedTask;
        }
        
        protected override Task OnInactivate()
        {
            historicalDialogueInfos.Clear();
            _dialogueManager.OnSingleDialogueStart -= OnSingleDialogueStart;
            _dialogueManager.OnSingleDialogueEnd -= OnSingleDialogueEnd;
            _dialogueManager.OnSelectDialogueBranch -= OnSelectDialogueBranch;
            view.StoryReviewView.OnSubViewClosed -= OnSubViewClosed;
            dialogueTipCor = null;
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 按钮点击事件处理
        /// </summary>
        /// <param name="btnName">按钮名称（与UI配置的按钮名对应）</param>
        protected override async void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnContinue": // 继续按钮
                    // 对话框激活时，触发下一条对话
                    if (IsActiveBox)
                    {
                        _dialogueManager.NextDialogue();
                    }
                    else
                    {
                        // 切换对话框激活状态并更新显示
                        IsActiveBox = !IsActiveBox;
                        view.SetDialogueBoxActive(IsActiveBox);
                    }
                    break;
                case "btnHide": // 隐藏按钮
                    // 切换对话框激活状态并更新显示
                    IsActiveBox = !IsActiveBox;
                    view.SetDialogueBoxActive(IsActiveBox);
                    break;
                case "btnReview": // 回顾按钮
                    await CreateOrShowReviewView();
                    break;
            }
        }
        
        /// <summary>
        /// 创建或显示回顾界面
        /// </summary>
        private async Task CreateOrShowReviewView()
        {
            if (view.StoryReviewView)
            {
                // 显示故事回顾界面
                view.SetActiveReview(true);
                await view.StoryReviewView.Review(historicalDialogueInfos);
                return;
            }
            
            var storyReviewView = await _objectSpawner.SpawnAsync<StoryReviewView>(AssetKeys.StoryReviewSubView, view.ReviewRoot);
            // 注册故事回顾子视图关闭事件
            storyReviewView.OnSubViewClosed += OnSubViewClosed;
            view.SetStoryReviewView(storyReviewView);
            await storyReviewView.Review(historicalDialogueInfos);
        }
        
        /// <summary>
        /// 故事回顾子视图关闭时的回调
        /// </summary>
        private void OnSubViewClosed()
        {
            // 隐藏故事回顾界面
            view.SetActiveReview(false);
        }

        /// <summary>
        /// 开关组件值变更事件处理
        /// </summary>
        /// <param name="toggleName">开关名称（与UI配置的开关名对应）</param>
        /// <param name="isOn">开关是否开启</param>
        protected override void OnToggleValueChanged(string toggleName, bool isOn)
        {
            switch (toggleName)
            {
                case "togAuto": // 自动播放开关
                    // 预留自动播放逻辑扩展点
                    break;
            }
        }

        /// <summary>
        /// 单条对话开始时的回调
        /// </summary>
        /// <param name="dialogueInfo">当前对话信息实体</param>
        private void OnSingleDialogueStart(DialogueInfo dialogueInfo)
        {
            historicalDialogueInfos.Add(dialogueInfo);
            // 启动对话提示动画协程
            dialogueTipCor = _monoAdapter.StartCoroutine(DialogueTip_Cor());
        }
        
        private void OnSelectDialogueBranch(BranchInfo branchInfo)
        {
            historicalDialogueInfos.Add(branchInfo);
        }
        
        /// <summary>
        /// 单条对话结束时的回调
        /// </summary>
        private void OnSingleDialogueEnd()
        {
            // 停止对话提示动画协程
            _monoAdapter.StopCoroutine(dialogueTipCor);
            // 恢复默认提示文本
            view.SetTip(DefaultTip);
        }

        /// <summary>
        /// 对话提示动画协程
        /// 循环渐变显示省略号提示文本
        /// </summary>
        /// <returns>协程迭代器</returns>
        public IEnumerator DialogueTip_Cor()
        {
            var length = DialogueTip.Length;
            // 直到对话结束停止协程
            while (true) 
            {
                // 逐字符显示省略号，每次等待0.25秒
                for (var i = 0; i < length; i++)
                {
                    view.SetTip(DialogueTip.Substring(0, i + 1));
                    yield return s_waitForSeconds0_25;
                }
            }
        }

        /// <summary>
        /// 显示对话文本内容
        /// </summary>
        /// <param name="speakerName">说话人名称</param>
        /// <param name="dialogueText">对话文本内容</param>
        public void ShowDialogueText(string speakerName ,string dialogueText)
        {
            // 清空上一次的分支选项
            _objectSpawner.Release(view.DialogueOptUIs);
            // 更新视图显示说话人名称和对话文本
            view.UpdateNameAndText(speakerName, dialogueText);
        }

        /// <summary>
        /// 设置对话分支选项
        /// 动态创建分支选项UI并绑定选择事件
        /// </summary>
        /// <param name="branchDatas">分支信息数组</param>
        public async void SetBranchOpt(BranchData[] branchDatas)
        {
            try
            {
                // 清空已有分支选项;
                _objectSpawner.Release(view.DialogueOptUIs);
                // 遍历分支信息，逐个创建选项UI
                foreach (var branchData in branchDatas)
                {
                    // 从资源包异步加载分支选项UI预制体，并挂载到对话框节点下
                    var optUI = await _objectSpawner.SpawnAsync<DialogueOptUI>(AssetKeys.DialogueOptUI, view.DialogueOptBox);
                    // 初始化分支选项UI
                    optUI.Init(branchData);
                    // 绑定选项选择事件到对话管理器的处理方法
                    optUI.OnSelectOpt += _dialogueManager.OnSelectOpt;
                    // 将选项UI缓存到模型中（便于后续管理）
                    view.DialogueOptUIs.Add(optUI);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Dialogue, e);
            }
        }

        protected override Task OnDispose()
        {
            _objectSpawner.Release(view.StoryReviewView);
            _objectSpawner.Dispose();
            _objectSpawner = null;
            return Task.CompletedTask;
        }
    }
}