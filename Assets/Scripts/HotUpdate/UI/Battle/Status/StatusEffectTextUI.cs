using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Status
{
    /// <summary>
    /// 状态效果文本UI组件
    /// 负责显示战斗中角色的状态效果（如buff/debuff）文本与图标，
    /// 并处理文本的向上移动、自动回收逻辑
    /// </summary>
    public class StatusEffectTextUI : UIBehaviourBase, ILogicView<StatusEffectTextUI, StatusEffectTextLogic>
    {
        // 状态效果图标图片组件
        [InjectUI] public Image imgIcon;
        // 状态效果名称文本组件
        [InjectUI] public TextMeshProUGUI txtBuffName;

        // 控制文本移动的矩形变换组件（通过InjectUI指定索引1注入）
        [InjectUI(1)] public RectTransform Mover { get; set; }

        // 文本向上移动的速度（可在Inspector面板配置）
        [SerializeField] public float upMoveSpeed = 1f;
        // 文本显示后自动销毁/回收的时间（可在Inspector面板配置）
        [SerializeField] public float destroyTime = 0.85f;
        
        private StatusEffectTextLogic _logic;
        
        public void Init(StatusEffectTextLogic logic)
        {
            logic.SetTextMover();
            _logic = logic;
        }
        
        /// <summary>
        /// 组件禁用时执行的逻辑
        /// 移除帧更新监听，避免内存泄漏
        /// </summary>
        protected override void OnDisable()
        {
            _logic.Dispose();
        }
    }
}