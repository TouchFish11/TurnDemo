using System;
using Core.Mono;
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
    public class StatusEffectTextUI : UIBehaviourBase
    {
        // 状态效果图标图片组件
        [InjectUI] private Image imgIcon;
        // 状态效果名称文本组件
        [InjectUI] private TextMeshProUGUI txtBuffName;

        // 控制文本移动的矩形变换组件（通过InjectUI指定索引1注入）
        [InjectUI(1)] private RectTransform Mover { get; set; }

        // 文本向上移动的速度（可在Inspector面板配置）
        [SerializeField] private float upMoveSpeed = 1f;
        // 文本显示后自动销毁/回收的时间（可在Inspector面板配置）
        [SerializeField] private float destroyTime = 0.85f;

        private IMonoAdapter _monoAdapter;
        // 移动组件的初始本地位置（用于每次激活时重置位置）
        private Vector3 originMoverPos;
        // 记录当前文本显示的累计时间
        private float currentTime;

        public event Action<StatusEffectTextUI> OnDurationOver;
        
        /// <summary>
        /// 唤醒方法，初始化游戏物体引用和移动组件初始位置
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            originMoverPos = Mover.localPosition;
        }

        /// <summary>
        /// 组件启用时执行的逻辑
        /// 重置移动组件位置，并注册帧更新监听
        /// </summary>
        protected override void OnEnable()
        {
            // 重置移动组件到初始位置
            Mover.localPosition = originMoverPos;
        }

        /// <summary>
        /// 初始化文本
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="buffName"></param>
        /// <param name="monoAdapter"></param>
        public void InitText(Sprite icon, string buffName, IMonoAdapter monoAdapter)
        {
            imgIcon.sprite = icon;
            txtBuffName.text = buffName;
            // 向Mono管理器注册帧更新回调
            monoAdapter.AddUpdateListener(OnUpadte);
            _monoAdapter = monoAdapter;
        }

        /// <summary>
        /// 帧更新回调方法
        /// 处理文本向上移动逻辑和超时回收逻辑
        /// </summary>
        private void OnUpadte()
        {
            // 累计当前显示时间
            currentTime += Time.deltaTime;
            // 检查是否达到回收时间
            if (currentTime >= destroyTime)
            {
                // 重置累计时间
                currentTime = 0;
                OnDurationOver?.Invoke(this);
            }
            // 让移动组件沿Y轴向上移动（基于帧率的平滑移动）
            Mover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
        }

        /// <summary>
        /// 组件禁用时执行的逻辑
        /// 移除帧更新监听，避免内存泄漏
        /// </summary>
        protected override void OnDisable()
        {
            // 从Mono管理器移除帧更新回调
            _monoAdapter.RemoveUpdateListener(OnUpadte);
        }
    }
}