using System;
using System.Threading.Tasks;
using Core.DI;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.Base.UI;
using HotUpdate.Common;

using HotUpdate.Game.Dialogue.UI;
using HotUpdate.Game.Main.Back;
using HotUpdate.Game.Main.Loading.Battle;
using HotUpdate.Game.Main.UI;
using HotUpdate.UI.Activity.Base;
using HotUpdate.UI.Back;
using HotUpdate.UI.Battle.Base;
using HotUpdate.UI.Loading.Battle;
using HotUpdate.UI.Quests;
using UnityEngine;

namespace HotUpdate.UI
{
    /// <summary>
    /// UI服务
    /// </summary>
    public class UIService : IUIService
    {
        [Inject] private IUIManager _uiManager;
        
        public async Task<IuiController> OpenAsync(EUIPanelId panelId, E_UILayer layer, Vector2 pos = default, Quaternion quaternion = default)
        {
            IuiController controller = null;
            switch (panelId)
            {
                case EUIPanelId.MainPanel: 
                   controller = await _uiManager.CreateViewAsync<MainView, MainController>(AssetKeys.MainView, layer);
                   break;
                case EUIPanelId.BagPanel:
                    break;
                case EUIPanelId.BattlePanel:
                    controller = await _uiManager.CreateViewAsync<BattleView, BattleController>(AssetKeys.BattleView, layer);
                    break;
                case EUIPanelId.ActivityPanel:
                    controller = await _uiManager.CreateViewAsync<ActivityView, ActivityController>(AssetKeys.ActivityView, layer);
                    break;
                case EUIPanelId.QuestPanel:
                    controller = await _uiManager.CreateViewAsync<TaskView, TaskController>(AssetKeys.QuestView, layer);
                    break;
                case EUIPanelId.DialoguePanel:
                    controller = await _uiManager.CreateViewAsync<DialogueView, DialogueController>(AssetKeys.DialogueView, layer);
                    break;
                case EUIPanelId.BattleLoadingkPanel:
                    controller = await _uiManager.CreateViewAsync<BattleLoadingView, BattleLoadingController>(AssetKeys.BattleLoadingView, layer);
                    break;
                case EUIPanelId.BlackBackPanel:
                    controller = await _uiManager.CreateViewAsync<BackView, BackController>(AssetKeys.BackView, layer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(panelId), panelId, null);
            }

            return controller;
        }

        public Task CloseAsync(int panelId)
        {
           return _uiManager.DestroyView(panelId);
        }

        public IuiController GetPanel(EUIPanelId panelId)
        {
            switch (panelId)
            {
                case EUIPanelId.MainPanel: 
                    return _uiManager.GetController<MainController>();
                case EUIPanelId.BagPanel:
                    break;
                case EUIPanelId.BattlePanel:
                    return _uiManager.GetController<BattleController>();
                case EUIPanelId.ActivityPanel:
                    return _uiManager.GetController<ActivityController>();
                case EUIPanelId.QuestPanel:
                    return _uiManager.GetController<TaskController>();
                case EUIPanelId.DialoguePanel:
                    return _uiManager.GetController<DialogueController>();
                case EUIPanelId.BattleLoadingkPanel:
                    return _uiManager.GetController<BattleLoadingController>();
                case EUIPanelId.BlackBackPanel:
                    return _uiManager.GetController<BackController>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(panelId), panelId, null);
            }

            return null;
        }
    }
}
