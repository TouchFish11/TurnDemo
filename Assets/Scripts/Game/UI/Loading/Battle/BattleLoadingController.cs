using Framework;
using Game.Battle;
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

public class BattleLoadingController : UIController<BattleLoadingView, BattleLoadingModel>
{
    public BattleLoadingController(BattleLoadingView view, BattleLoadingModel model) : base(view, model)
    {

    }

    public void LoadBattle()
    {
        //// 加载战斗场景（异步加载，避免卡顿）
        //SceneManager.Instance.LoadSceneAsync(ResConfigCollection.LevelScene, UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) => UpdateProgress(progress), async () =>
        //{
        //    bool isSuccess = await BattleManager.Instance.StartBattle();
        //    if (isSuccess)
        //    {
        //        // 隐藏加载界面
        //        UIManager.Instance.DestroyView();
        //    }
        //});
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="progress"></param>
    public void UpdateProgress(float progress)
    {
        _model.Progress = progress;
    }
}
