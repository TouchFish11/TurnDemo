using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 玩家角色
    /// </summary>
    public class PlayerObject : BattleObject
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        public RoleInfo RoleInfo { get; private set; }

        public override void BaseInit(int id)
        {
            base.BaseInit(id);

            RoleInfo = BinaryDataMgr.Instance.GetConfig<RoleInfoContainer>(E_ConfigLoadType.Editor).dataDic[id];
            // 初始化与战斗无关的属性
            Name = RoleInfo.f_name;
        }

        public override void BattleInit(int roleId, IBattleContext context)
        {
            base.BattleInit(roleId, context);
            // 添加战斗相关组件
            AddComponents(TextUtility.SplitToIntArr(RoleInfo.f_comIds, 2));
        }

        public override IEnumerator ExecuteAction()
        {
            while (true)
            {

                // 等待玩家输入指令




                yield return null;
            }
        }

        public override int GetSpeed()
        {
            return 100;
        }
    }
}
