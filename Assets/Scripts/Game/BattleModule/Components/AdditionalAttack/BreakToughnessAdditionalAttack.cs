using Framework;
using GameLogic.BattleMoudule.Core;
using GameLogic.BattleMoudule.Entity;
using GameLogic.BattleMoudule.Toughness;

namespace GameLogic.BattleMoudule.AdditionalAttack
{
    /// <summary>
    /// 破盾后追加穿刺
    /// </summary>
    public class BreakToughnessAdditionalAttack : IAdditionalAttack
    {
        public bool CanTrigger(IBattleContext context, IBattleEntity attacker, IBattleEntity target)
        {
            // 触发条件：目标已破盾 + 攻击者是破盾者
            if (target.GetBattleComponent<ToughnessComponent>(out var toughnessComponent))
            {
                return toughnessComponent.IsToughnessBroken();
            }
            return false;
        }

        public void Execute(IBattleContext context, IBattleEntity attacker, IBattleEntity target)
        {
            // 计算追加攻击伤害（配置表读取系数）
            int additionalDamage = (int)(attacker.GetField(E_FieldType.Attack) * 0.8f);
            target.TakeDamage(additionalDamage, E_PropertyType.Physical);
            LogMgr.Log($"{attacker.Name}触发破盾追加攻击！{target.Name}额外受到{additionalDamage}点伤害");
        }
    }
}
