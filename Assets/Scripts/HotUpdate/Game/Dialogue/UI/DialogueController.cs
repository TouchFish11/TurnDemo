using System;
using System.Collections;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.UI.ViewController;
using HotUpdate.Base.Manager;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Common.Generated;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Dialogue.UI
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 对话控制器核心类
    /// 处理对话界面的交互逻辑、对话内容展示、分支选项设置等核心功能
    /// </summary>
    public class DialogueController : UIController<DialogueView>, IBlockOperation
    {
        private static readonly WaitForSeconds s_waitForSeconds0_25 = new(0.25f);

        [Inject] private IDialogueManager _dialogueManager;
        [Inject] private ObjectSpawner _objectSpawner;
        
        /// <summary>
        /// 对话加载提示文本（渐变显示的省略号）
        /// </summary>
        private const string DialogueTip = "...";
        
        /// <summary>
        /// 对话默认提示文本（继续按钮提示）
        /// </summary>
        private const string DefaultTip = "V";

        // 对话提示动画的协程引用，用于停止协程
        private Coroutine dialogueTipCor;
        
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
        
        public bool BlockOperation { get; private set; }
        
        public void SetBlock(bool isBlock)
        {
            BlockOperation = isBlock;
        }
        
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
            // 注册故事回顾子视图关闭事件
            view.StoryReviewView.OnSubViewClosed += OnSubViewClosed;
            // 初始状态隐藏故事回顾界面
            view.SetActiveReview(false);
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            _dialogueManager.OnSingleDialogueStart -= OnSingleDialogueStart;
            _dialogueManager.OnSingleDialogueEnd -= OnSingleDialogueEnd;
            view.StoryReviewView.OnSubViewClosed -= OnSubViewClosed;
            dialogueTipCor = null;
            return Task.CompletedTask;
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
        /// 按钮点击事件处理
        /// </summary>
        /// <param name="btnName">按钮名称（与UI配置的按钮名对应）</param>
        protected override void OnButtonClick(string btnName)
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
                    // 显示故事回顾界面
                    view.SetActiveReview(true);
                    break;
            }
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
            // 将当前对话信息缓存到故事回顾视图
            view.StoryReviewView.CacheDialogueInfo(dialogueInfo);
            // 启动对话提示动画协程
            dialogueTipCor = DIContainer.GetInstance<IMonoAdapter>().StartCoroutine(DialogueTip_Cor());
        }
        
        /// <summary>
        /// 单条对话结束时的回调
        /// </summary>
        private void OnSingleDialogueEnd()
        {
            // 停止对话提示动画协程
            DIContainer.GetInstance<IMonoAdapter>().StopCoroutine(dialogueTipCor);
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
            view.ClearBranchOpt();
            // 更新视图显示说话人名称和对话文本
            view.UpdateNameAndText(speakerName, dialogueText);
        }

        /// <summary>
        /// 设置对话分支选项
        /// 动态创建分支选项UI并绑定选择事件
        /// </summary>
        /// <param name="branchInfos">分支信息数组</param>
        public async void SetBranchOpt(BranchInfo[] branchInfos)
        {
            try
            {
                // 清空已有分支选项
                view.ClearBranchOpt();
                // 遍历分支信息，逐个创建选项UI
                foreach (var branchInfo in branchInfos)
                {
                    // 从资源包异步加载分支选项UI预制体，并挂载到对话框节点下
                    var optUIPoolObject = await _objectSpawner.SpawnAsync<DialogueOptUI>(AssetKeys.DialogueOptUI, view.DialogueOptBox);
                    
                    // 初始化分支选项UI
                    optUIPoolObject.Obj.Init(branchInfo);
                    // 绑定选项选择事件到对话管理器的处理方法
                    optUIPoolObject.Obj.OnSelectOpt += _dialogueManager.OnSelectOpt;
                    // 将选项UI缓存到模型中（便于后续管理）
                    view.CacheBranchOpt(optUIPoolObject);
                }
            }
            catch (Exception e)
            {
                // 记录分支选项创建异常日志
                Logger.LogError($"{nameof(DialogueController)}.{nameof(SetBranchOpt)}: {e.Message}，{e.StackTrace}");
            }
        }
    }
}