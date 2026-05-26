using System;
using System.Collections.Generic;
using Core.DI;
using Core.HotUpdate;
using Core.Log;
using HotUpdate.Base.Factory;
using HotUpdate.Game.Activity.Core;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Conditions;
using HotUpdate.Game.Battle.Skill.Factory;
using HotUpdate.Game.Battle.Status;
using HotUpdate.Game.Battle.TargetSelect;
using HotUpdate.Game.Battle.Toughness;
using HotUpdate.Game.Battle.UI;

namespace HotUpdate.Game.Core
{
    /// <summary>
    /// 工厂管理器
    /// 管理器所有实现IFactory的工厂
    /// </summary>
    public class FactoryManager : IFactoryManager
    {
        public void BindFactory()
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
        }
    }
}
