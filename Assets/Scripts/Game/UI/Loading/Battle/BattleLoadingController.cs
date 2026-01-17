using Framework;
using Game.Battle;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleLoadingControllerFactory : UIControllerFactory<BattleLoadingView, BattleLoadingModel, BattleLoadingController>
{
    public override BattleLoadingController CreateController(BattleLoadingView view, BattleLoadingModel model)
    {
        return new BattleLoadingController(view, model);
    }

    public override BattleLoadingModel CreateModel()
    {
        return new BattleLoadingModel();
    }
}

/// <summary>
/// 战斗加载界面控制器
/// </summary>
[UIControllerFactory(typeof(BattleLoadingControllerFactory))]
public class BattleLoadingController : UIController<BattleLoadingView, BattleLoadingModel>
{
    public BattleLoadingController(BattleLoadingView view, BattleLoadingModel model) : base(view, model)
    {

    }

    public void LoadBattle()
    {
        // 加载战斗场景（异步加载，避免卡顿）
        SceneManager.Instance.LoadSceneAsync(ResKeyCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) => UpdateProgress(progress), async () =>
        {
            // 显示战斗界面
            BattleController battleController = await UIManager.Instance.CreateViewAsync<BattleView, BattleModel, BattleController>(E_UILayer.Mid);
            await BattleManager.Instance.StartBattle();
            // 隐藏加载界面
            UIManager.Instance.DestroyView();
        });
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="progress"></param>
    public void UpdateProgress(float progress)
    {
        view.UpdateProgress(progress);
    }
}
