using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 公共Mono管理器
    /// </summary>
    public class MonoManager : SingletonAutoMono<MonoManager>
    {
        /// <summary>
        /// 物理帧更新事件
        /// </summary>
        private event UnityAction FixedUpdateEvent;
        /// <summary>
        /// 帧更新事件
        /// </summary>
        private event UnityAction UpdateEvent;
        /// <summary>
        /// 后期更新事件
        /// </summary>
        private event UnityAction LateUpdateEvent;

        /// <summary>
        /// 添加物理帧更新监听函数
        /// </summary>
        /// <param name="fixedUpdateFun">物理帧更新监听函数</param>
        public void AddFixedUpdateListener(UnityAction fixedUpdateFun)
        {
            this.FixedUpdateEvent += fixedUpdateFun;
        }

        /// <summary>
        /// 添加帧更新监听函数
        /// </summary>
        /// <param name="updateFun">帧更新监听函数</param>
        public void AddUpdateListener(UnityAction updateFun)
        {
            this.UpdateEvent += updateFun;
        }

        /// <summary>
        /// 添加后期帧更新监听函数
        /// </summary>
        /// <param name="lateUpdateFun">后期帧更新监听函数</param>
        public void AddLateUpdateListener(UnityAction lateUpdateFun)
        {
            this.LateUpdateEvent += lateUpdateFun;
        }

        /// <summary>
        /// 移除物理帧更新监听函数
        /// </summary>
        /// <param name="fixedUpdateFun">物理帧更新监听函数</param>
        public void RemoveFixedUpdateListener(UnityAction fixedUpdateFun)
        {
            this.FixedUpdateEvent -= fixedUpdateFun;
        }

        /// <summary>
        /// 移除帧更新监听函数
        /// </summary>
        /// <param name="updateFun">帧更新监听函数</param>
        public void RemoveUpdateListener(UnityAction updateFun)
        {
            this.UpdateEvent -= updateFun;
        }

        /// <summary>
        /// 移除后期帧更新监听函数
        /// </summary>
        /// <param name="lateUpdateFun">后期帧更新监听函数</param>
        public void RemoveLateUpdateListener(UnityAction lateUpdateFun)
        {
            this.LateUpdateEvent -= lateUpdateFun;
        }

        private void FixedUpdate()
        {
            FixedUpdateEvent?.Invoke();
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdateEvent?.Invoke();
        }

        protected override void OnDestroy()
        {
            FixedUpdateEvent = null;
            UpdateEvent = null;
            LateUpdateEvent = null;

            base.OnDestroy();
        }
    }
}
