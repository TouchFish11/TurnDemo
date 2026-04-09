using Core.Serialize.Binary;
using Core.Service;
using Core.UI;
using Core.UI.MVC;
using HotUpdate.Core.Task;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Main.UI
{
    /// <summary>
    /// 主界面视图类
    /// 负责主界面中交互列表、任务栏等UI元素的显示与更新逻辑
    /// 实现IMainView接口，提供主界面UI操作的统一入口
    /// </summary>
    public class MainView : UIView
    {
        #region 注入的UI组件
        /// <summary>
        /// 交互内容滚动视图组件
        /// 用于承载各类可交互的UI元素列表
        /// </summary>
        [Inject] private ScrollRect svInteract;
        
        /// <summary>
        /// 任务栏根节点
        /// 控制整个任务栏的显示/隐藏
        /// </summary>
        [Inject(1)] private RectTransform taskPart;
        
        /// <summary>
        /// 任务标题文本组件
        /// 显示当前任务的名称
        /// </summary>
        [Inject] private TextMeshProUGUI txtTaskTitle;
        
        /// <summary>
        /// 任务描述文本组件
        /// 显示当前任务的描述及进度信息
        /// </summary>
        [Inject] private TextMeshProUGUI txtTaskShortDescription;
        
        /// <summary>
        /// 任务描述文本组件
        /// 显示当前任务的描述及进度信息
        /// </summary>
        [Inject] private TextMeshProUGUI txtTaskProgress;
        #endregion

        #region 公共属性
        /// <summary>
        /// 交互列表的内容容器Transform
        /// 供外部添加/管理交互项UI
        /// </summary>
        public Transform InteractContent => svInteract.content;
        #endregion

        #region 公共方法
        /// <summary>
        /// 设置任务栏的激活状态（显示/隐藏）
        /// </summary>
        /// <param name="active">是否激活显示</param>
        public void SetTaskbarActive(bool active)
        {
            taskPart.gameObject.SetActive(active);
        }

        public void SetQuestbarTitle(string title)
        {
            // 设置任务标题
            txtTaskTitle.text = title;
        }

        public void SetQuestbarTip(string description)
        {
            txtTaskShortDescription.text = description;
        }

        public void SetQuestbarProgress(string progressStr)
        {
            txtTaskProgress.text = progressStr;
        }
        #endregion
    }
}