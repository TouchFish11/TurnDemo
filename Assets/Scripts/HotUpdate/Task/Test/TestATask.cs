using Core.GlobalEvent;
using Core.Service;
using HotUpdate.Core.Task;
using HotUpdate.Core.Task.Event;
using HotUpdate.Task.Core;

namespace HotUpdate.Task.Test
{
    public class TestATask
    {
        private readonly IEventCenter _eventCenter = ServiceLocator.Get<IEventCenter>();
        
        public TestATask(ITaskData taskData) : base()
        {
            
        }

        public void RegisterEvents()
        {
            _eventCenter.SubscribeEvent<BattleEvent>(BattleEvent);
        }

        public void UnregisterEvents()
        {
            _eventCenter.UnsubscribeEvent<BattleEvent>(BattleEvent);
        }

        private void BattleEvent(BattleEvent battleEvent)
        {

        }
    }
}
