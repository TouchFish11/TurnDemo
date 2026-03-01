using Core.UI;
using Core.UI.MVC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Task.UI
{
    /// <summary>
    /// 任务界面视图层
    /// 负责任务界面的UI显示、组件引用和交互状态更新
    /// 实现ITaskView接口，为控制层提供UI操作入口
    /// </summary>
    public class TaskView : UIView
    {
        #region 组件引用
        /// <summary>
        /// 任务列表滚动视图
        /// </summary>
        [Inject] private ScrollRect svTask;

        /// <summary>
        /// 接受/追踪任务按钮
        /// </summary>
        [Inject] private Button btnAcceptTask;

        /// <summary>
        /// 任务项单选组（保证同一时间仅选中一个任务）
        /// </summary>
        [Inject] private ToggleGroup taskContent;

        /// <summary>
        /// 任务名称显示文本
        /// </summary>
        [Inject] private TextMeshProUGUI txtTaskName;

        /// <summary>
        /// 任务描述显示文本
        /// </summary>
        [Inject] private TextMeshProUGUI txtTaskDescription;

        /// <summary>
        /// 任务接受/追踪状态提示文本
        /// </summary>
        [Inject] private TextMeshProUGUI txtAccceptInfo;

        /// <summary>
        /// 任务详情面板根节点
        /// </summary>
        [Inject(1)] private RectTransform detailView;

        /// <summary>
        /// 任务奖励展示容器
        /// </summary>
        [Inject(1)] private RectTransform rewardBox;

        /// <summary>
        /// 有任务时的显示面板
        /// </summary>
        [Inject(1)] private RectTransform hasTaskView;

        /// <summary>
        /// 无任务时的显示面板
        /// </summary>
        [Inject(1)] private RectTransform noTaskView;
        #endregion

        #region 公共属性
        /// <summary>
        /// 任务列表滚动视图的内容容器（用于挂载任务项预制体）
        /// </summary>
        public Transform TaskContent => svTask.content;

        /// <summary>
        /// 任务项单选组对外暴露属性
        /// </summary>
        public ToggleGroup TaskItemGroup => taskContent;
        
        /// <summary>
        /// 奖励框
        /// </summary>
        public RectTransform RewardBox => rewardBox;
        #endregion

        #region 公共方法

        /// <summary>
        /// 更新任务详情面板显示
        /// </summary>
        /// <param name="taskInfo">当前选中的任务信息数据</param>
        public void UpdateTaskDetail(TaskInfo taskInfo)
        {
            // 更新任务名称和描述
            txtTaskName.text = taskInfo.f_taskName;
            txtTaskDescription.text = taskInfo.f_taskDescription;
        }

        /// <summary>
        /// 切换有无任务的显示面板
        /// </summary>
        /// <param name="hasTasks">是否有可显示的任务</param>
        public void HasTasks(bool hasTasks)
        {
            hasTaskView.gameObject.SetActive(hasTasks);
            noTaskView.gameObject.SetActive(!hasTasks);
        }

        /// <summary>
        /// 更新任务追踪状态的文本提示
        /// </summary>
        /// <param name="isFollowingTask">是否正在追踪当前任务</param>
        public void UpdateFollowTask(bool isFollowingTask)
        {
            txtAccceptInfo.text = isFollowingTask ? "取消追踪" : "开始追踪";
        }
        #endregion
    }
}