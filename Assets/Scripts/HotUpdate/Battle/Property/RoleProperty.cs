using Core.Serialize.Binary;
using Core.Service;
using HotUpdate.Core.Battle.Property;
using UnityEngine;

namespace HotUpdate.Battle.Property
{
    /// <summary>
    /// ��ɫ����
    /// </summary>
    public class RoleProperty : BattleProperty
    {
        // ��������
        protected int baseEnergy;   // ��������



        // ��̬����
        protected int currentEnergy;    // ��ǰ����


        public override void InitProperty(int id)
        {
            base.InitProperty(id);
            RoleInfo roleInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Excel).dataDic[id];

            baseHp = roleInfo.f_baseHp;
            baseAtk = roleInfo.f_baseAtk;
            baseDef = roleInfo.f_baseDef;
            baseSpeed = roleInfo.f_baseSpeed;
            baseEnergy = roleInfo.f_maxEnergy;

            currentHp = maxHp = baseHp;
            currentSpeed = baseSpeed;
            totalAtk = baseAtk;
            totalDef = baseDef;
            totalCrit = baseCrit;
            totalCritDmg = baseCritDmg;

            currentEnergy = 0;
        }


        public int BaseEnergy => baseEnergy;



        public int CurrentEnergy { get => currentEnergy; set => currentEnergy = Mathf.Clamp(value, default, baseEnergy); }

    }
}
