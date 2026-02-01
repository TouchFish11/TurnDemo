using System.Collections.Generic;
using Core.Pool;
using Core.Service;
using GameHotUpdate.UI.MVC;

namespace GameHotUpdate.UI.Dialogue
{
    /// <summary>
    /// 对话数据模型类
    /// 负责管理对话系统中的选择项UI、播放状态等核心数据
    /// 继承自UI模型基类，实现IDialogueModel接口
    /// </summary>
    public class DialogueModel : UIModel
    {
        // 对话选项UI的缓存列表，用于管理当前显示的所有对话分支选项
        private readonly List<DialogueOptUI> dialogueOptUIs = new();
        
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

        /// <summary>
        /// 清空所有对话分支选项
        /// 回收选项UI到对象池，并清空缓存列表
        /// </summary>
        public void ClearBranchOpt()
        {
            // 遍历所有缓存的对话选项UI，将其游戏对象回收至对象池
            foreach (var opt in dialogueOptUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(opt.gameObject);
            }
            // 清空对话选项UI缓存列表
            dialogueOptUIs.Clear();
        }
        
        /// <summary>
        /// 缓存对话分支选项UI
        /// 将创建的对话选项UI加入缓存列表，便于后续统一管理
        /// </summary>
        /// <param name="dialogueOpt">单个对话选项UI的接口实例</param>
        public void CacheBranchOpt(DialogueOptUI dialogueOpt)
        {
            dialogueOptUIs.Add(dialogueOpt);
        }

        /// <summary>
        /// 重写基类的清空数据方法
        /// 对话模型销毁/重置时，清空所有对话分支选项数据
        /// </summary>
        public override void ClearData()
        {
            ClearBranchOpt();
        }
    }
}