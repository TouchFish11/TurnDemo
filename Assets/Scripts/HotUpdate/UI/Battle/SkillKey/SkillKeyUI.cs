using Core.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.SkillKey
{
    /// <summary>
    /// 技能按键UI组件
    /// 负责单个技能按键的显示、选中、触发等交互逻辑
    /// </summary>
    public class SkillKeyUI : UIBehaviourBase, ILogicView<SkillKeyUI, SkillKeyLogic>
    {
        // 技能提示文本（显示技能类型信息）
        [InjectUI] public TextMeshProUGUI txtSkillTip;
        // 技能按键的Toggle组件（用于选中状态切换）
        public Toggle togSkillKeyUI;
        private SkillKeyLogic _logic;
        
        protected override void Awake()
        {
            base.Awake();
            // 获取当前GameObject同名的Toggle组件（UI绑定约定）
            // 注：Toggle和图片在同一GameObject下，需通过名称匹配，无法直接绑定同名字段
            togSkillKeyUI = binder.GetControl<Toggle>(gameObject.name);
            // 注册点击事件监听
            UIUtility.AddCustomEventListener(this, EventTriggerType.PointerClick, OnClick);
        }
        
        public void Init(SkillKeyLogic logic)
        {
            _logic = logic;
        }
        
        /// <summary>
        /// Toggle选中状态变更回调（BaseUIBehaviour生命周期）
        /// </summary>
        /// <param name="togName">Toggle组件名称</param>
        /// <param name="isOn">是否选中</param>
        protected override void OnToggleValueChanged(string togName, bool isOn)
        {
            _logic?.OnSelected(isOn);
        }
        
        /// <summary>
        /// 技能按键点击事件处理
        /// </summary>
        /// <param name="baseEventData">事件数据（UI事件基础数据）</param>
        private void OnClick(BaseEventData baseEventData)
        {
            _logic?.OnClick();
        }
        
        /// <summary>
        /// 组件禁用时的清理逻辑（OnDisable生命周期）
        /// 防止组件复用导致的状态残留
        /// </summary>
        protected override void OnDisable()
        {
            _logic.Dispose();
        }
    }
}