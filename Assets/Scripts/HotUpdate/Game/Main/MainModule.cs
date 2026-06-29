using System.Threading.Tasks;
using Core.DI;
using Game.Module;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Module;
using HotUpdate.Base.Scene;
using HotUpdate.Base.Service;
using HotUpdate.Game.Activity.Core;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Operation;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Statuses;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.Toughness;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Inputs;
using HotUpdate.Game.Interact;
using HotUpdate.Game.Main.FloatingText;
using HotUpdate.Game.Main.Player;
using HotUpdate.Game.Quests;
using HotUpdate.Game.Scene;
using HotUpdate.Game.VFX;

namespace HotUpdate.Game.Main
{
    /// <summary>
    /// 游戏主场景模块
    /// </summary>
    [ModuleExport(typeof(IMainModule))]
    public class MainModule : IMainModule
    {
        public int Priority => 10;

        public void Register()
        {
            // 注册浮动文本管理器
            DIContainer.BindSingleton<IFloatingTextManager, FloatingTextManager>();
            // 注册玩家管理器
            DIContainer.BindSingleton<IPlayerManager, PlayerManager>();
            // 注册特效管理器
            DIContainer.BindSingleton<IVFXManager, VFXManager>();
            // 注册游戏管理器
            DIContainer.BindSingleton<IGameDataManager, GameDataManager>();
            // 注册场景生成器
            DIContainer.BindSingleton<ISceneGenerator, SceneGenerator>();
            // 注册鼠标管理器
            DIContainer.BindSingleton<IMouseManager, MouseManager>();
            // 注册鼠标管理器
            DIContainer.BindSingleton<IQuestManager, QuestManager>();
            // 注册图标服务
            DIContainer.BindType<IIconService, IconService>();
        }

        public Task InitModuleAsync()
        {
            BindFactorys();
            return Task.CompletedTask;
        }

        private static void BindFactorys()
        {
            DIContainer.BindSingleton<IActivityDataFactory, ActivityDataFactory>();
            DIContainer.BindSingleton<ICastSkillConditionFactory, CastSkillConditionFactory>();
            DIContainer.BindSingleton<ISkillCastPostHandlerFactory, SkillCastPostHandlerFactory>();
            DIContainer.BindSingleton<ISkillKeyUIDataProviderFactory, SkillKeyUIDataProviderFactory>();
            DIContainer.BindSingleton<IStatusFactory, StatusFactory>();
            DIContainer.BindSingleton<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>();
            DIContainer.BindSingleton<IToughnessStrategyFactory, ToughnessStrategyFactory>();
            DIContainer.BindSingleton<IRoleFactory, RoleFactory>();
            DIContainer.BindSingleton<IMonsterFactory, MonsterFactory>();
            DIContainer.BindSingleton<INpcFactory, NpcFactory>();
        }
    }
}