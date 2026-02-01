// using System.Collections.Generic;
// using Game.Battle.Event.General;
//
// namespace GameHotUpdate.Battle.AdditionalAttack
// {
//     /// <summary>
//     /// ��ɫ׷�ӹ����������������׷�ӹ������ƣ�
//     /// </summary>
//     public class AdditionalAttackComponent : BattleComponent, IAdditionalAttackComponent
//     {
//         // ׷�ӹ����б�
//         private readonly List<IAdditionalAttack> _additionalAttacks = new List<IAdditionalAttack>();
//
//         public override void BattleInit(IBattleEntityObject battleEntity)
//         {
//             base.BattleInit(battleEntity);
//
//             // ����׷�ӹ������ƣ��ɴ����ñ��󶨣��������ƽ�������ʵ���ࣩ
//             _additionalAttacks.Add(new BreakToughnessAdditionalAttack());
//
//             // ���ġ��ƶ��¼����������ƶܣ�����׷�ӹ�����
//             battleEntity.Context.GetEventBus().AddListener<ToughnessBrokenEvent>(OnToughnessBrokenHandler);
//         }
//
//         /// <summary>
//         /// �¼��ص����ƶܺ󴥷�׷�ӹ���
//         /// </summary>
//         /// <param name="evt"></param>
//         private void OnToughnessBrokenHandler(ToughnessBrokenEvent toughnessBrokenEvent)
//         {
//             // ֻ������ǰ���������ɫ��׷�ӹ��������ƶ��ߣ�
//             if (toughnessBrokenEvent.Breaker != BattleEntity)
//             {
//                 return;
//             }
//
//             // ��������׷�ӹ������ж��Ƿ����㴥������
//             foreach (var attack in _additionalAttacks)
//             {
//                 if (attack.CanTrigger(toughnessBrokenEvent.Context, BattleEntity, toughnessBrokenEvent.Target))
//                 {
//                     attack.Execute(toughnessBrokenEvent.Context, BattleEntity, toughnessBrokenEvent.Target);
//                 }
//             }
//         }
//
//         public override void Destroy()
//         {
//             base.Destroy();
//             _additionalAttacks.Clear();
//             // �Ƴ�����
//             BattleEntity.Context.GetEventBus().RemoveListener<ToughnessBrokenEvent>(OnToughnessBrokenHandler);
//         }
//     }
// }
