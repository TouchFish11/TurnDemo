using Core.DI;
using Core.UI;
using HotUpdate.Base.Data;


namespace HotUpdate.UI.Activity.Base
{
    /// <summary>
    /// 活动UI组件
    /// </summary>
    public abstract class ActivityUIComponent : UIBehaviourBase
    {
        [Inject] protected IActivityDataProvider ActivityDataProvider;
        
        protected ActivityUIBehaviourBase ActivityUIBehaviourBase { get; private set; }
        protected ActivityInfo ActivityInfo { get; private set; }
        
        /// <summary>
        /// 初始化UI组件
        /// </summary>
        /// <param name="activityUIBehaviourBase"></param>
        /// <param name="activityInfo"></param>
        public void Init(ActivityUIBehaviourBase activityUIBehaviourBase, ActivityInfo activityInfo)
        {
            ActivityUIBehaviourBase = activityUIBehaviourBase;
            ActivityInfo = activityInfo;
            // 在初始化后执行
            OnInit();
        }
        
        /// <summary>
        /// 在初始化后执行
        /// </summary>
        protected abstract void OnInit();

        protected override void OnDestroy()
        {
            ActivityUIBehaviourBase = null;
            ActivityInfo = null;
            ActivityDataProvider = null;
        }
    }
}
