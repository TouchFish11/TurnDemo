using System.Threading.Tasks;
using Game.Module;
using HotUpdate.Base.Module;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗模块
    /// </summary>
    [ModuleExport(typeof(IBattleModule))]
    public class BattleModule : IBattleModule
    {
        public int Priority => 9;
        
        public void Register()
        {
            
        }

        public Task InitModuleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
