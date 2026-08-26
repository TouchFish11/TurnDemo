using System;
using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.UI;
using UnityEngine;

namespace HotUpdate.UI.Battle.FloatText
{
    public abstract class FloatTextLogic<TFloatUI, TFloatLogic> : IUILogic<TFloatUI, TFloatLogic> , IPoolData
        where TFloatUI : FloatTextUI<TFloatUI, TFloatLogic> where TFloatLogic : FloatTextLogic<TFloatUI, TFloatLogic> 
    {
        [Inject] protected IMonoAdapter _monoAdapter;
        [Inject] protected IPoolManager _poolManager;
        
        // 文字向上移动的速度（单位：像素/秒）
        protected const float upMoveSpeed = 2.5f;
        // 文字显示后自动销毁的时长（单位：秒）
        protected const float destroyTime = 0.85f;
        // 文字初始缩放比例（显示时先放大）
        protected readonly Vector3 StartScale = Vector3.one * 2f;
        // 文字最终缩放比例（放大后过渡到正常大小）
        protected readonly Vector3 endScale = Vector3.one;
        // 缩放过渡的速度因子（值越大缩放越快）
        protected const float scaleFactor = 9f;

        // 文字当前显示时长（用于计时销毁）
        protected float currentTime;
        // 文字初始颜色（记录用于透明度过渡）
        protected Color originColor;
        // 文字初始透明度（记录原始透明度值）
        protected float originAlpha;
        public event Action<TFloatUI> OnDurationOver;
        
        public TFloatUI View { get; protected set; }

        protected abstract RectTransform TextMover { get; }

        public virtual void SetTextMover()
        {
            // 重置文字移动节点的锚点位置为初始值
            TextMover.anchoredPosition = Vector3.zero;
            // 设置文字初始缩放（放大显示）
            TextMover.localScale = StartScale;
        }

        protected void StartUpdate()
        {
            _monoAdapter.AddUpdateListener(OnUpdate);
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
                // 重置计时
                currentTime = 0;
                OnDurationOver?.Invoke(View);
            }

            // 缩放过渡：从初始缩放值平滑过渡到最终缩放值
            TextMover.localScale = Vector3.Lerp(TextMover.localScale, endScale, Time.deltaTime * scaleFactor);
            // 向上移动：每帧按移动速度向上偏移位置
            TextMover.Translate(Time.deltaTime * upMoveSpeed * Vector3.up);
        }
        
        void IPoolData.ResetData()
        {
            // 移除帧更新监听，停止逻辑执行
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
