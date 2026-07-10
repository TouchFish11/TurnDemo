using System;
using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.UI.ViewController;
using HotUpdate.Base.Data;

namespace HotUpdate.UI.Tip
{
    public class TipController : UIController<TipView>
    {
        [Inject] private ConfirmContentFactory _confirmContentFactory;
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override Task OnActive()
        {
            return Task.CompletedTask;
        }

        protected override Task OnInactivate()
        {
            _confirmContentFactory.Dispose();
            _confirmContentFactory = null;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置提示
        /// </summary>
        /// <param name="confirmData">确认数据</param>
        public async void SetTip(ConfirmData confirmData)
        {
            try
            {
                view.txtTitle.text = confirmData.ConfirmTitle;
                var contentUI = await _confirmContentFactory.CreateContent(confirmData.ConfirmContent, view.ContentRoot);
                contentUI.DrawContent(confirmData);
            }
            catch (Exception e)
            {
                Logger.LogError(TODO, $"[{nameof(TipController)}] create tip content prefab error, {e.Message}");
            }
        }
    }
}
