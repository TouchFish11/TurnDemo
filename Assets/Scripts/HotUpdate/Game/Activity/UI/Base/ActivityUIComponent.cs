using Core.Components;
using Core.UI;
using HotUpdate.Base.Activity;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Activity.Core;

namespace HotUpdate.Game.Activity.UI.Base
{
    /// <summary>
    /// 活动UI组件
    /// </summary>
    public abstract class ActivityUIComponent : UIBehaviourBase, IComponent
    {
        protected ActivityUIBehaviourBase ActivityUIBehaviourBase { get; private set; }
        protected ActivityInfo ActivityInfo { get; private set; }
        protected IActivityData ActivityData { get; private set; }
        
        /// <summary>
        /// UI界面没有继承IEntityObject接口，此属性返回null
        /// </summary>
        public IEntityObject EntityObject { get; private set; }

        /// <summary>
        /// 初始化UI组件
        /// </summary>
        /// <param name="activityUIBehaviourBase"></param>
        /// <param name="activityInfo"></param>
        /// <param name="activityData"></param>
        public void Init(ActivityUIBehaviourBase activityUIBehaviourBase, ActivityInfo activityInfo, IActivityData activityData)
        {
            ActivityUIBehaviourBase = activityUIBehaviourBase;
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
            ActivityUIBehaviourBase = null;
            ActivityInfo = null;
            ActivityData = null;
        }
    }
}
