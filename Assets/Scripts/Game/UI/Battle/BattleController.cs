using Game.Battle;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 战斗界面控制器工厂
/// </summary>
public class BattleControllerFactory : UIControllerFactory<BattleView, BattleModel, BattleController>
{
    public override BattleController CreateController(BattleView view, BattleModel model)
    {
        return new BattleController(view, model);
    }

    public override BattleModel CreateModel()
    {
        return new BattleModel();
    }
}

/// <summary>
/// 战斗界面控制器
/// </summary>
[UIControllerFactory(typeof(BattleControllerFactory))]
public class BattleController : UIController<BattleView, BattleModel>
{
    // 依赖注入各子模块
    private BattleUIInitializer _uiInitializer;
    private BattleEventProcessor _eventProcessor;
    private BattleUIManager _uiManager;

    public BattleController(BattleView view, BattleModel model) : base(view, model)
    {

    }

    protected async override Task OnInit()
    {
        _uiInitializer = new BattleUIInitializer(view, model);
        _uiManager = new BattleUIManager(view, model);
        _eventProcessor = new BattleEventProcessor(this, _uiManager, _uiInitializer);
        await Task.CompletedTask;
    }

    /// <summary>
    /// 初始化战斗UI
    /// </summary>
    /// <param name="battleEntities"></param>
    public async Task InitBattleUI(IBattleContext battleContext)
    {
        await _uiInitializer.InitPlayerUI(battleContext.GetPlayerObjects());
        await _uiManager.UpdateBattlePointCount(battleContext.CurentBattlePointCount, battleContext.MaxBattlePointCount);
        _eventProcessor.RegisterBattleEvents(battleContext.GetEventBus());
    }

    public BattleUIManager GetBattleUI() => _uiManager;

    public BattleUIInitializer GetUIInitializer() => _uiInitializer;
}
