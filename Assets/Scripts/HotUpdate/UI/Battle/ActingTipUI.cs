using Core.DI;
using Core.Mono;
using Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle
{
    /// <summary>
    /// 行动提示UI组件
    /// 负责显示我方/敌方行动中的提示文本，以及行动图标的摆动动画
    /// </summary>
    public class ActingTipUI : UIBehaviourBase
    {
        // 行动图标图片组件
        [Inject] private Image imgActingIcon;
        // 行动提示文本组件
        [Inject] private TextMeshProUGUI txtActingTip;

        // 我方行动提示文本常量
        private const string PlayerTipText = "我方行动中...";
        // 敌方行动提示文本常量
        private const string MonsterTipText = "敌方行动中...";

        // 图标移动的范围（距离）
        [SerializeField] private float moveRange = 6f;
        // 图标移动的速度（频率）
        [SerializeField] private float moveSpeed = 7f;

        // 图标初始位置
        private Vector3 originTrans;

        /// <summary>
        /// 启用方法，重置图标位置并添加帧更新监听
        /// </summary>
        protected override void OnEnable()
        {
            // 重置图标到初始位置
            imgActingIcon.transform.position = originTrans;
            // 注册帧更新回调，用于处理图标动画
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }
        
        /// <summary>
        /// 初始化方法，赋值UI组件并记录图标初始位置
        /// </summary>
        /// <param name="imgActingIcon">行动图标图片组件</param>
        /// <param name="txtActingTip">行动提示文本组件</param>
        public void Init(Image imgActingIcon, TextMeshProUGUI txtActingTip)
        {
            // 记录图标初始位置
            originTrans = imgActingIcon.transform.position;

            this.imgActingIcon = imgActingIcon;
            this.txtActingTip = txtActingTip;
        }

        /// <summary>
        /// 更新提示文本内容
        /// </summary>
        /// <param name="isMonster">是否为敌方行动</param>
        public void UpdateTipText(bool isMonster)
        {
            // 根据是否为敌方，切换提示文本
            txtActingTip.text = isMonster ? MonsterTipText : PlayerTipText;
        }

        /// <summary>
        /// 帧更新回调方法，处理行动图标的摆动动画
        /// </summary>
        private void OnUpdate()
        {
            // 图标摆动动画：基于正弦函数实现往复移动
            // Time.time * moveSpeed 控制动画频率，Mathf.Sin返回-1到1之间的数值，乘以移动范围控制位移距离
            // transform.right 表示沿图标自身右方向移动
            imgActingIcon.transform.localPosition = Mathf.Sin(Time.time * moveSpeed) * moveRange * imgActingIcon.transform.right;

            // 文本动画预留位置（暂未实现）
        }

        /// <summary>
        /// 禁用方法，移除帧更新监听，避免内存泄漏
        /// </summary>
        protected override void OnDisable()
        {
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}