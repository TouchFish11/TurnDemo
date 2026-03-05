// using Core.Log;
//
// namespace HotUpdate.Battle.AdditionalAttack
// {
//     /// <summary>
//     /// �ƶܺ�׷�Ӵ���
//     /// </summary>
//     public class BreakToughnessAdditionalAttack : IAdditionalAttack
//     {
//         public bool CanTrigger(IBattleContext context, IBattleEntityObject attacker, IBattleEntityObject target)
//         {
//             // ����������Ŀ�����ƶ� + ���������ƶ���
//             return target.GetComponent<ToughnessComponent>().IsToughnessBroken();
//         }
//
//         public void Execute(IBattleContext context, IBattleEntityObject attacker, IBattleEntityObject target)
//         {
//             // ����׷�ӹ����˺������ñ���ȡϵ����
//             int additionalDamage = (int)(attacker.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().TotalAtk * 0.8f);
//
//             DamageCalcManager.Instance.CalcSkillDamage(attacker, target, null, out DamageResult result);
//             target.TryTakeDamage(result);
//             LogManager.Log($"{attacker.GameObject.name}�����ƶ�׷�ӹ�����{target.GameObject.name}�����ܵ�{additionalDamage}���˺�");
//         }
//     }
// }
