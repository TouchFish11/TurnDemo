using System;
using Core.GlobalEvent.Events;
using Core.Log;
using Core.UI.MVC;
using HotUpdate.Common;

namespace HotUpdate.Main.Global.UI
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

        /// <summary>
        /// 显示消息
        /// </summary>
        /// <param name="msg"></param>
        private async void ShowMessage(string msg)
        {
            try
            {
                var messageUIWrapper = await prefabLoader.GetObjectAsync<MessageUI>(AbKeyCollection.Ui, ResKeyCollection.MessageUI, view.MessageContainer);
                messageUIWrapper.InitMessage(msg);
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(GlobalMessageController)}.{nameof(ShowMessage)}：{e.Message}，{e.StackTrace}");
            }
        }
        
        protected override Task OnHide()
        {
            eventCenter.UnsubscribeEvent<GlobalMessageEvent>(OnGlobalMessageEvent);
            return Task.CompletedTask;
        }
    }
}