
using System.Collections;
using UnityEngine;

namespace Game.Battle
{
    /// <summary>
    /// 战斗实体接口
    /// </summary>
    public interface IBattleEntityObject : IEntityObject
    {
        /// <summary>
        /// 子游戏对象
        /// 返回该脚本依附对象的第一个子对象
        /// </summary>
        GameObject SubGameObject { get; }

        /// <summary>
        /// 战斗实体ID
        /// </summary>
        int BattleEntityId { get; }

        /// <summary>
        /// 实体位置索引
        /// </summary>
        int EntityPosIndex { get; }

        /// <summary>
        /// 是否死亡
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// 能否行动
        /// </summary>
        bool CanAct { get; }

        /// <summary>
        /// 启动行动
        /// </summary>
        void EnableAct();

        /// <summary>
        /// 禁用行动
        /// </summary>
        void DisableAct();

        /// <summary>
        /// 战斗上下文
        /// </summary>
        IBattleContext Context { get; }

        /// <summary>
        /// 行动值
        /// </summary>
        float ActionValue { get; }

        /// <summary>
        /// 设置行动值
        /// </summary>
        void SetActionValue(float actionValue);

        /// <summary>
        /// 战斗初始化
        /// </summary>
        /// <param name="id"></param>
        /// <param name="context"></param>
        void BattleInit(int id, IBattleContext context);

        /// <summary>
        /// 执行行动
        /// </summary>
        /// <returns></returns>
        void ExecuteAction();
       
        /// <summary>
        /// 获取速度
        /// </summary>
        /// <returns></returns>
        int GetSpeed();

        /// <summary>
        /// 回血
        /// </summary>
        /// <param name="value"></param>
        void Heal(int value);

        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="value"></param>
        void TakeDamage(DamageResult damageResult);

        /// <summary>
        /// 增加行动次数
        /// </summary>
        void AddActCount();

        /// <summary>
        /// 减少行动次数
        /// </summary>
        void SubActCount();

        /// <summary>
        /// 死亡
        /// 实体死亡逻辑
        /// </summary>
        IEnumerator Die();
    }
}
