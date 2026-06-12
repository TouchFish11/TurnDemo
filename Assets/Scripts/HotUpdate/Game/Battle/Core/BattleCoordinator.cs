using System;
using Core.DI;
using Core.Log;
using HotUpdate.Base;
using HotUpdate.Base.Manager;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Inputs;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Core
{
    /// <summary>
    /// 战斗协调器
    /// </summary>
    public class BattleCoordinator
    {
        private IBattleContext _battleContext;
        
        public IBattleInputHandler BattleInputHandler { get; }
        public IBattleCameraManager BattleCameraManager { get; private set; }
        public ITargetSelectManager TargetSelectManager { get; private set; }

        // private Action<IBattleEntityObject> _OnSelectedObject;
        //
        // /// <summary>
        // /// 选中战斗实体对象的事件（如选中玩家/怪物作为技能目标）
        // /// 事件参数：选中的战斗实体对象接口
        // /// </summary>
        // public event Action<IBattleEntityObject> OnSelectedObject
        // {
        //     add
        //     {
        //         if (_OnSelectedObject != null)
        //         {
        //             Logger.LogError($"{nameof(OnSelectedObject)}重复添加");
        //             return;
        //         }
        //         _OnSelectedObject += value;
        //     }
        //     remove => _OnSelectedObject -= value;
        // }
        
        public BattleCoordinator(IBattleInputHandler battleInputHandler)
        {
            BattleInputHandler = battleInputHandler;
        }

        public void Init(IBattleContext battleContext)
        {
            BattleCameraManager = DIContainer.Create<IBattleCameraManager>(parameterValues: new object[] { this, BattleInputHandler });
            TargetSelectManager = DIContainer.Create<ITargetSelectManager>(parameterValues: new object[] { this, BattleInputHandler, battleContext });
            _battleContext = battleContext;
        }

        public void SelectedEntity(BattleObject battleObject)
        {
            TargetSelectManager.SelectClickMainTarget(battleObject);
        }
    }
}
