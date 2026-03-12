using System.Threading.Tasks;
using Core.Log;
using Core.Pool;
using Core.Scene;
using Core.Service;
using Core.UI;
using HotUpdate.Battle.Core;
using HotUpdate.Battle.Object.Role.Warrior;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Input;
using HotUpdate.Core.Module;
using HotUpdate.Core.UI.Helper;
using UnityEngine;

namespace HotUpdate.Battle
{
    /// <summary>
    /// 热更战斗模块
    /// </summary>
    public class BattleModule : IBattleModule
    {
        public int Priority => 1;
        
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IBattleManager>(new BattleManager(
                ServiceLocator.Get<IUIManager>(),
                ServiceLocator.Get<ISceneManager>(),
                ServiceLocator.Get<IMouseManager>(),
                ServiceLocator.Get<IPoolManager>()));
            // 初始化UIHelper
            ServiceLocator.Register<IBattleUiHelper>(new BattleUiHelper(ServiceLocator.Get<IUIManager>()));
            LogManager.Log($"{nameof(BattleModule)}.{nameof(InitModuleAsync)}：初始化完成");
            return Task.CompletedTask;
        }
        
        public IPlayerObject AddWarrior(GameObject warrior)
        {
            return warrior.AddComponent<Warrior>();
        }
    }
}
