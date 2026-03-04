using Core.GlobalEvent.Events;
using Core.Loader.Object;
using Core.Service;
using Core.UI.MVC;
using GameHotUpdate.Config;

namespace GameHotUpdate.Global.UI
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 全局消息界面
    /// </summary>
    public class GlobalMessageController : UIController<GlobalMessageView, GlobalMessageModel>
    {
        protected override Task OnShow()
        {
            // 注册全局消息事件
            eventCenter.SubscribeEvent<GlobalMessageEvent>(OnGlobalMessageEvent);
            return Task.CompletedTask;
        }

        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 全局消息事件回调
        /// </summary>
        /// <param name="globalMessageEvent"></param>
        private void OnGlobalMessageEvent(GlobalMessageEvent globalMessageEvent)
        {
            ShowMessage(globalMessageEvent.Message);
        }

        private async void ShowMessage(string msg)
        {
            var messageUIWrapper = await ServiceLocator.Get<IPrefabLoader>().GetObjectAsync<MessageUI>(AbKeyCollection.Ui, ResKeyCollection.MessageUI, view.MessageContainer);
            messageUIWrapper.InitMessage(msg);
        }
        
        protected override Task OnHide()
        {
            eventCenter.UnsubscribeEvent<GlobalMessageEvent>(OnGlobalMessageEvent);
            return Task.CompletedTask;
        }
    }
}