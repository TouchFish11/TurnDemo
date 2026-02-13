using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using TMPro;
using UnityEngine;

namespace GameHotUpdate.Battle.UI.FloatText
{
    public class HealTextUI : UIBehaviourBase
    {
        // 治疗数值文本（如"1000"、"500"等）
        [Inject] private TextMeshProUGUI txtHealNum;

        // 治疗文字的移动根节点（用于控制位置和缩放）
        [Inject(1)] private RectTransform HealTextMover { get; set; }
        
        // 文字向上移动的速度（单位：像素/秒）
        private const float upMoveSpeed = 2.5f;
        // 文字显示后自动销毁的时长（单位：秒）
        private const float destroyTime = 0.85f;
        // 文字初始缩放比例（显示时先放大）
        private readonly Vector3 StartScale = Vector3.one * 1.8f;
        // 文字最终缩放比例（放大后过渡到正常大小）
        private readonly Vector3 endScale = Vector3.one;
        // 缩放过渡的速度因子（值越大缩放越快）
        private const float scaleFactor = 9f;
        
        // 文字当前显示时长（用于计时销毁）
        private float currentTime;
        
        /// <summary>
        /// 组件启用时初始化
        /// 注册更新监听、重置位置/缩放/计时/透明度
        /// </summary>
        protected override void OnEnable()
        {
            // 注册帧更新监听，每帧执行OnUpdate逻辑
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
            // 重置文字移动节点的锚点位置为初始值
            (HealTextMover.transform as RectTransform).anchoredPosition = Vector3.zero;
            // 设置文字初始缩放（放大显示）
            HealTextMover.localScale = StartScale;
        }

        /// <summary>
        /// 初始化治疗文字的显示内容和样式
        /// </summary>
        /// <param name="healText"></param>
        public void InitHealText(int healText)
        {
            // 设置治疗数值文本内容（转为字符串）
            txtHealNum.text = healText.ToString();
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
                ServiceLocator.Get<IPoolManager>().PushObj(gameObject);
            }

            // 缩放过渡：从初始缩放值平滑过渡到最终缩放值
            HealTextMover.localScale = Vector3.Lerp(HealTextMover.localScale, endScale, Time.deltaTime * scaleFactor);
            // 向上移动：每帧按移动速度向上偏移位置
            HealTextMover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
        }

        /// <summary>
        /// 组件禁用时清理
        /// 移除更新监听，避免内存泄漏
        /// </summary>
        protected override void OnDisable()
        {
            // 移除帧更新监听，停止逻辑执行
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
