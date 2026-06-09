namespace HotUpdate.UI.Activity.Base
{
    public abstract class ActivityContentHandler<T> : IActivityContentHandler where T : ActivityUIBehaviourBase
    {
        protected T activity;
        
        public void Init(ActivityUIBehaviourBase activity)
        {
            this.activity = activity as T;
        }
    }
}
