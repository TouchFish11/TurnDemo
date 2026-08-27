using System;
using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.Time;
using Core.UI;
using UnityEngine;

namespace HotUpdate.UI.Battle.Status
{
    public class StatusEffectTextLogic : IUILogic<StatusEffectTextUI, StatusEffectTextLogic>, IPoolData
    {
        [Inject] private IPoolManager _poolManager;
        [Inject] private IMonoAdapter _monoAdapter;
        
        // 移动组件的初始本地位置（用于每次激活时重置位置）
        private Vector3 originMoverPos;
        // 记录当前文本显示的累计时间
        private float currentTime;
        
        public event Action<StatusEffectTextUI> OnDurationOver;
        
        public StatusEffectTextUI View { get; private set; }

        /// <summary>
        /// 初始化文本
        /// </summary>
        /// <param name="view"></param>
        /// <param name="icon"></param>
        /// <param name="buffName"></param>
        public void InitText(StatusEffectTextUI view, Sprite icon, string buffName)
        {
            View = view;
            View.imgIcon.sprite = icon;
            View.txtBuffName.text = buffName;
            // 向Mono管理器注册帧更新回调
            _monoAdapter.AddUpdateListener(OnUpadte);
        }
        
        public virtual void SetTextMover()
        {
            originMoverPos = View.Mover.localPosition;
        }
        
        /// <summary>
        /// 帧更新回调方法
        /// 处理文本向上移动逻辑和超时回收逻辑
        /// </summary>
        private void OnUpadte()
        {
            // 累计当前显示时间
            currentTime += TimeUtil.DeltaTime;
            // 检查是否达到回收时间
            if (currentTime >= View.destroyTime)
            {
                // 重置累计时间
                currentTime = 0;
                OnDurationOver?.Invoke(View);
            }
            // 让移动组件沿Y轴向上移动（基于帧率的平滑移动）
            View.Mover.Translate(Time.deltaTime * View.upMoveSpeed * Vector3.up);
        }
        
        void IPoolData.ResetData()
        {
            // 从Mono管理器移除帧更新回调
            _monoAdapter.RemoveUpdateListener(OnUpadte);
        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
