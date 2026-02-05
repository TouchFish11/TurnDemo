using System.Collections;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Toughness;

namespace GameHotUpdate.Battle.Command
{
    /// <summary>
    /// �����������
    /// Ŀǰ���ڹ����������ָ�
    /// </summary>
    public class ToughnessCommand : Game.Battle.Command.Command, IToughnessCommand
    {

        /// <summary>
        /// �������
        /// </summary>
        public IToughnessComponent ToughnessComponent { get; private set; }

        public override int Priority { get; protected set; }

        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="toughnessComponent"></param>
        public void Init(IToughnessComponent toughnessComponent)
        {
            Sender = toughnessComponent.BattleEntity;
            ToughnessComponent = toughnessComponent;
        }

        public override IEnumerator Execute(IBattleContext context)
        {
            float currentValue = 0;
            while (ToughnessComponent.CurrentToughnessValue < ToughnessComponent.MaxToughnessVaue)
            {
                //currentValue += UnityEngine.Time.deltaTime * recoverySpeed;
                ToughnessComponent.SetToughnessValue((int)currentValue, ToughnessComponent.MaxToughnessVaue);
                yield return null;
            }
        }

        public override IEnumerator ExcutePostProcess(IBattleContext context)
        {
            yield break;
        }

        public override void ResetData()
        {
            base.ResetData();
            ToughnessComponent = null;
        }
    }
}
