using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Loader.UI;
using Core.Service;
using Core.UI.MVC;
using GameHotUpdate.Config;

namespace GameHotUpdate.Global.UI
{
    /// <summary>
    ///
    /// </summary>
    public class GlobalMessageController : UIController<GlobalMessageView, GlobalMessageModel>
    {
        protected override async System.Threading.Tasks.Task OnInit()
        {
            // �����¼�
            ServiceLocator.Get<IEventCenter>().SubscribeEvent<GlobalMessageEvent>(OnGlobalMessageEvent);
            await System.Threading.Tasks.Task.CompletedTask;
        }

        private void OnGlobalMessageEvent(GlobalMessageEvent globalMessageEvent)
        {
            ShowMessage(globalMessageEvent.Message);
        }

        private async void ShowMessage(string msg)
        {
            var messageUIWrapper = await ServiceLocator.Get<IUiLoader>().GetUIObject<MessageUI>(AbKeyCollection.Ui, ResKeyCollection.MessageUI, view.MessageContainer);
            messageUIWrapper.InitMessage(msg);
        }
        
        public override void Destroy()
        {
            base.Destroy();
            ServiceLocator.Get<IEventCenter>().UnsubscribeEvent<GlobalMessageEvent>(OnGlobalMessageEvent);
        }
    }
}