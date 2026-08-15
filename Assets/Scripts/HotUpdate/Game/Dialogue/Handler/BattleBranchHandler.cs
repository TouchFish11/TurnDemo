using System.Collections.Generic;
using Core.DI;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Scene;
using HotUpdate.Base.UI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Turn;
using HotUpdate.Game.Dialogue.Datas;

namespace HotUpdate.Game.Dialogue.Handler
{
    /// <summary>
    /// 战斗分支处理器
    /// </summary>
    public class BattleBranchHandler : IBranchHandler
    {
        [Inject] private ISceneGenerator _sceneGenerator;
        [Inject] private IUIService _uiService;
        [Inject] private IPlayerManager _playerManager;
        [Inject] private IDialogueManager _dialogueManager;
        
        public EBranchType BranchType => EBranchType.Battle;
    
        public async void Execute(BranchData branchData)
        {
            var waveDatas = new List<WaveData>
            {
                // 测试数据
                new(waveId: 1, victoryConditionType: EWaveVictoryConditionType.EliminateAllEnemies, monsterIds: new List<int> {1,2,1}),
            };

            var battleStartupParams = new BattleStartupParams
            {
                WaveDatas = waveDatas,
                OnPreEnter = async () =>
                {
                    _sceneGenerator.ClearMainScene();
                    await _uiService.CloseAsync(_uiService.GetPanel(EUIPanelId.DialoguePanel).PanelId, true);
                },
                OnBattleOver = async _ =>
                {
                    await _sceneGenerator.InitMainScene(-1);
                    await _playerManager.CreatePlayer(1001);
                    _dialogueManager.StartDialogue(-1);
                }
            };

            await BattleEntry.StartBattle(battleStartupParams);
        }
    }
}
