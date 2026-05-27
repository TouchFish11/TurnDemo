using System;
using Core.AssetBundles.Management;
using Core.DI;
using Core.GlobalEvent.Events;
using Core.Log;
using Core.Time;
using Core.UI.ViewController;
using HotUpdate.Base.UI;
using HotUpdate.Common;


namespace HotUpdate.UI.Global.UI
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 全局消息界面
    /// </summary>
    public class GlobalMessageController : UIController<GlobalMessageView>, IBlockOperation
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private ITimerManager _timerManager;
        
        // 显示时间
        private const float Duration = 2.5f;

        public bool BlockOperation { get; private set; }
        
        public void SetBlock(bool isBlock)
        {
            BlockOperation = isBlock;
        }
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            // 注册全局消息事件
            eventCenter.SubscribeEvent<GlobalMessageEvent>(OnGlobalMessageEvent);
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            eventCenter.UnsubscribeEvent<GlobalMessageEvent>(OnGlobalMessageEvent);
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
                var poolObject = await _objectSpawner.SpawnAsync<MessageUI>(AssetKeys.MessageUI, view.MessageContainer);
                poolObject.Obj.InitMessage(msg);
                _timerManager.CreateTimer(false, (int)(Duration * 1000), () => poolObject.Collect());
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(GlobalMessageController)}.{nameof(ShowMessage)}：{e.Message}");
            }
        }
    }
}