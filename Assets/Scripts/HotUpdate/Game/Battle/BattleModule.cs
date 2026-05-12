using System.Threading.Tasks;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Game.Battle.Object.Role.Warrior;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle
{
    /// <summary>
    /// 热更战斗模块
    /// </summary>
    public class BattleModule
    {
        public int Priority => 2;
        
        public Task InitModuleAsync()
        {
            // DIContainer.GetInstance.Register<IBattleManager>(new BattleManager(
            //     DIContainer.GetInstance<IUIManager>(),
            //     DIContainer.GetInstance<ISceneManager>(),
            //     DIContainer.GetInstance<IMouseManager>(),
            //     DIContainer.GetInstance<IPoolManager>()));
            // // 初始化UIHelper
            // DIContainer.GetInstance.Register<IBattleUiHelper>(new BattleUiHelper(DIContainer.GetInstance<IUIManager>()));
            Logger.Log($"{nameof(BattleModule)}.{nameof(InitModuleAsync)}:Battle module initialization completed");
            return Task.CompletedTask;
        }
        
        public IPlayerObject AddWarrior(GameObject warrior)
        {
            return warrior.AddComponent<Warrior>();
        }
    }
}
