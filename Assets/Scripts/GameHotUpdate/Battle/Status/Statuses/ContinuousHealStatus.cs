using Core.Log;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Property;
using GameHotUpdate.Property;

namespace GameHotUpdate.Battle.Status.Statuses
{
    /// <summary>
    /// ������Ѫ״̬
    /// </summary>
    public class ContinuousHealStatus : Status
    {
        // ʣ������غ�
        private int _remainingTurns;
        // ��Ѫ���������ñ���ȡ��
        private float _healRatio;

        public ContinuousHealStatus()
        {

        }

        protected override void OnAdd()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRemove()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPineChanged()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnTurnStart(IBattleEntityObject owner, IBattleContext context)
        {
            if (!IsValid)
            {
                return;
            }

            // ���ý�ɫ�ġ���ѪAPI��ִ�о����߼���ģ���ڲ�/����ģ��API���ã�
            int healValue = (int)(owner.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().MaxHp * _healRatio);
            owner.Heal(healValue);
            LogManager.Log($"{owner.GameObject.name}����������Ѫ���ָ�{healValue}��HP");

            // ���ٳ����غϣ�������ʧЧ
            _remainingTurns--;
            if (_remainingTurns <= 0)
            {
                IsValid = false;
            }
        }
    }
}
