using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.UI;
using TMPro;
using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    /// <summary>
    /// 伤害文字UI组件
    /// 负责显示伤害数值、伤害类型，控制文字的上浮、缩放、销毁逻辑
    /// </summary>
    public class DamageTextUI : UIBehaviourBase
    {
        // 伤害类型文本（如"暴击"、"普通攻击"等）
        [InjectUI] private TextMeshProUGUI txtDamageTip;
        // 伤害数值文本（如"1000"、"500"等）
        [InjectUI] private TextMeshProUGUI txtDamageNum;

        // 伤害文字的移动根节点（用于控制位置和缩放）
        [InjectUI(1)] private RectTransform DamageTextMover { get; set; }

        // 文字向上移动的速度（单位：像素/秒）
        private const float upMoveSpeed = 2.5f;
        // 文字显示后自动销毁的时长（单位：秒）
        private const float destroyTime = 0.85f;
        // 文字初始缩放比例（显示时先放大）
        private readonly Vector3 StartScale = Vector3.one * 2f;
        // 文字最终缩放比例（放大后过渡到正常大小）
        private readonly Vector3 endScale = Vector3.one;
        // 缩放过渡的速度因子（值越大缩放越快）
        private const float scaleFactor = 9f;

        // 文字当前显示时长（用于计时销毁）
        private float currentTime;
        // 文字初始颜色（记录用于透明度过渡）
        private Color originColor;
        // 文字初始透明度（记录原始透明度值）
        private float originAlpha;
        
        /// <summary>
        /// 组件启用时初始化
        /// 注册更新监听、重置位置/缩放/计时/透明度
        /// </summary>
        protected override void OnEnable()
        {
            // 注册帧更新监听，每帧执行OnUpdate逻辑
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
            // 重置文字移动节点的锚点位置为初始值
            (DamageTextMover.transform as RectTransform).anchoredPosition = Vector3.zero;
            // 设置文字初始缩放（放大显示）
            DamageTextMover.localScale = StartScale;
            // 重置计时
            currentTime = 0;
            // 重置文本颜色（恢复初始颜色）
            txtDamageTip.color = originColor;
            txtDamageNum.color = originColor;
        }

        /// <summary>
        /// 初始化伤害文字的显示内容和样式
        /// </summary>
        /// <param name="textColor">文字颜色（如伤害类型对应的颜色）</param>
        /// <param name="damageTypeText">伤害类型文本（如"暴击"、"法术伤害"）</param>
        /// <param name="damage">伤害数值（需要显示的具体伤害值）</param>
        public void InitDamageText(Color textColor, string damageTypeText, int damage)
        {
            // 设置伤害类型和数值的文字颜色
            txtDamageTip.color = textColor;
            txtDamageNum.color = textColor;

            // 设置伤害类型文本内容
            txtDamageTip.text = damageTypeText;
            // 设置伤害数值文本内容（转为字符串）
            txtDamageNum.text = damage.ToString();

            // 记录初始颜色（用于后续透明度过渡）
            originColor = txtDamageTip.color;
        }

        /// <summary>
        /// 帧更新逻辑
        /// 处理计时销毁、缩放过渡、向上移动逻辑
        /// </summary>
        private void OnUpdate()
        {
            // 累计当前显示时长
            currentTime += Time.deltaTime;
            // 达到销毁时长时，回收对象到对象池
            if (currentTime >= destroyTime)
            {
                // 重置计时（避免重复回收）
                currentTime = 0;
                // 将当前游戏对象推回对象池
                DIContainer.GetInstance<IPoolManager>().PushObj(gameObject);
            }

            // 缩放过渡：从初始缩放值平滑过渡到最终缩放值
            DamageTextMover.localScale = Vector3.Lerp(DamageTextMover.localScale, endScale, Time.deltaTime * scaleFactor);
            // 向上移动：每帧按移动速度向上偏移位置
            DamageTextMover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
        }

        /// <summary>
        /// 组件禁用时清理
        /// 移除更新监听，避免内存泄漏
        /// </summary>
        protected override void OnDisable()
        {
            // 移除帧更新监听，停止逻辑执行
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}