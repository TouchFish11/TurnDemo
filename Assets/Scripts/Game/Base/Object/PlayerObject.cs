using Framework;
using Game.Battle;
using System.Collections;
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

        public override void BattleInit(int battleEntityId, IBattleContext context)
        {
            base.BattleInit(battleEntityId, context);
            // 添加战斗相关组件
            AddComponents(TextUtility.SplitToIntArr(RoleInfo.f_comIds, 2));
        }

        protected override IEnumerator OnExceuteAction()
        {
            // 等待玩家行动结束
            yield return new WaitWhile(() => CanAct);
        }

        public override int GetSpeed()
        {
            return 100;
        }
    }
}
