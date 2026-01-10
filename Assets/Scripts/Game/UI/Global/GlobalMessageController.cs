using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GlobalMessageControllerFactory : UIControllerFactory<GlobalMessageView, GlobalMessageModel, GlobalMessageController>
{
    public override GlobalMessageController CreateController(GlobalMessageView view, GlobalMessageModel model)
    {
        return new GlobalMessageController(view, model);
    }

    public override GlobalMessageModel CreateModel()
    {
        return new GlobalMessageModel();
    }
}

/// <summary>
/// 全局消息界面控制器
/// </summary>
[UIControllerFactory(typeof(GlobalMessageControllerFactory))]
public class GlobalMessageController : UIController<GlobalMessageView, GlobalMessageModel>
{
    public GlobalMessageController(GlobalMessageView view, GlobalMessageModel model) : base(view, model)
    {
        
    }

    protected async override Task OnInit()
    {
        // 监听事件
        EventCenter.Instance.SubscribeEvent<string>(E_EventType.E_GlobalMsg, ShowMessage);
        await Task.CompletedTask;
    }

    private async void ShowMessage(string msg)
    {
        MessageUI messageUI = await ObjectBuilder.GetObject<MessageUI>(E_AssetBundleType.UI, ResKeyCollection.MessageUI, view.MessageContainer);
        messageUI.InitMessage(msg);
    }


    public override void Destroy()
    {
        base.Destroy();
        EventCenter.Instance.UnsubscribeEvent<string>(E_EventType.E_GlobalMsg, ShowMessage);
    }
}
