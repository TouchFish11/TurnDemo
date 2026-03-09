using System.Threading.Tasks;
using Core.Service;
using Core.UI;
using HotUpdate.Battle.Core;
using HotUpdate.Battle.Object.Role.Warrior;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
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
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IBattleManager>(BattleManager.Instance);
            // 初始化UIHelper
            ServiceLocator.Register<IBattleUiHelper>(new BattleUiHelper(ServiceLocator.Get<IUIManager>()));
            return Task.CompletedTask;
        }
        
        public IPlayerObject AddWarrior(GameObject warrior)
        {
            return warrior.AddComponent<Warrior>();
        }
    }
}
