using System;
using Core.UI;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.Game.Quests.UI
{
    /// <summary>
    /// 任务项UI组件
    /// 负责单个任务项的显示、选中状态切换、事件回调等核心逻辑
    /// </summary>
    public class TaskItem : UIBehaviourBase
    {
        // 任务名称文本组件
        [InjectUI] private TextMeshProUGUI txtTaskName;
        // 选中状态显示图片
        [InjectUI] private Image imgSel;
        // 任务项切换选择器（用于控制选中状态）
        [InjectUI] private Toggle toggle;

        /// <summary>
        /// 当前任务项对应的任务ID
        /// </summary>
        public int TaskId { get; private set; }
        
        /// <summary>
        /// 选中任务时触发的事件（携带选中任务的ID）
        /// </summary>
        public event Action<int> OnSelectedTask;

        /// <summary>
        /// 初始化时执行（重写父类Awake）
        /// 主要完成游戏物体赋值、选中状态默认隐藏、切换选择器初始化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            
            // 初始状态隐藏选中标识
            imgSel.gameObject.SetActive(false);
            // 从绑定器中获取当前游戏物体对应的Toggle组件
            toggle = binder.GetControl<Toggle>(gameObject.name);
        }

        /// <summary>
        /// 任务项数据初始化
        /// </summary>
        /// <param name="questId"></param>
        /// <param name="questName"></param>
        /// <param name="group">Toggle分组（用于保证同组内仅能选中一个任务项）</param>
        public void Init(int questId, string questName, ToggleGroup group)
        {
            // 赋值当前任务项的唯一标识
            TaskId = questId;
            // 设置任务名称显示文本
            txtTaskName.text = questName;
            // 为Toggle绑定分组，确保分组内互斥选择
            toggle.group = group;
        }

        /// <summary>
        /// Toggle选中状态变更时触发的回调（重写父类方法）
        /// </summary>
        /// <param name="togName">触发状态变更的Toggle名称</param>
        /// <param name="isOn">当前Toggle是否被选中（true=选中，false=未选中）</param>
        protected override void OnToggleValueChanged(string togName, bool isOn)
        {
            // 根据选中状态显示/隐藏选中标识图片
            imgSel.gameObject.SetActive(isOn);
            // 若当前为选中状态，触发选中任务事件并传递任务ID
            if (isOn)
            {
                OnSelectedTask?.Invoke(TaskId);
            }
        }

        /// <summary>
        /// 主动选中当前任务项的方法
        /// 外部调用此方法可强制将当前任务项设为选中状态
        /// </summary>
        public void Select()
        {
            // 设置Toggle为选中状态（会自动触发OnToggleValueChanged回调）
            toggle.isOn = true;
        }
    }
}