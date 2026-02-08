using Core.Components;
using Core.UI;
using GameHotUpdate.Activity.Core;
using GameHotUpdate.Activity.Data;

namespace GameHotUpdate.Activity.UI.Base
{
    /// <summary>
    /// 活动UI组件
    /// </summary>
    public abstract class ActivityUIComponent : BaseUIBehaviour, IComponent
    {
        protected ActivityBase ActivityBase { get; private set; }
        protected ActivityInfo ActivityInfo { get; private set; }
        protected ActivityData ActivityData { get; private set; }
        
        /// <summary>
        /// UI界面没有继承IEntityObject接口，此属性返回null
        /// </summary>
        public IEntityObject EntityObject { get; private set; }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        /// <param name="activityBase"></param>
        /// <param name="activityInfo"></param>
        /// <param name="activityData"></param>
        public void Init(ActivityBase activityBase, ActivityInfo activityInfo, ActivityData activityData)
        {
            ActivityBase = activityBase;
            ActivityInfo = activityInfo;
            ActivityData = activityData;
            
            // 在初始化后执行
            OnInit();
        }
        
        /// <summary>
        /// 在初始化后执行
        /// </summary>
        protected abstract void OnInit();
        
        void IComponent.Init(IEntityObject entityObject)
        {
            EntityObject = entityObject;
        }

        public void Destroy()
        {
            throw new System.NotImplementedException();
        }
    }
}
