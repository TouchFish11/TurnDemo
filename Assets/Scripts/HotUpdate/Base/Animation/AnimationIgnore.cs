using System;

namespace HotUpdate.Base.Animation
{
    /// <summary>
    /// 动画忽略
    /// </summary>
    [Serializable]
    public class AnimationIgnore
    {
        public EAnimationType ignoreType;
        public float ignoreTime;
        private float currentIgnoreTime;
        
        /// <summary>
        /// 忽略结束
        /// </summary>
        public bool IgnoreOver { get; private set; }

        public AnimationIgnore(EAnimationType ignoreType)
        {
            this.ignoreType = ignoreType;
            currentIgnoreTime = ignoreTime;
        }

        public void Update(float deltaTime)
        {
            if (IgnoreOver)
            {
                return;
            }
            
            currentIgnoreTime -= deltaTime;
            if (currentIgnoreTime <= 0)
            {
                IgnoreOver = true;
            }
        }

        public void Reset()
        {
            currentIgnoreTime = ignoreTime;
            IgnoreOver = false;
        }
    }
}
