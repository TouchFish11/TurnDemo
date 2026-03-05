using Core.Log;
using HotUpdate.Battle.Damage.Data;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Property;
using HotUpdate.Extension;
using UnityEngine;

namespace HotUpdate.Battle.Damage.Strategys
{
    /// <summary>
    /// ֱ�˴���������
    /// </summary>
    public class DirectDamageStrategy : IDamageStrategy
    {
        //private IBattleEntityObject attacker;
        //private IBattleEntityObject defender;
        //private SkillInfo skillInfo;

        ////���ܱ�������
        //private int[] skillMuls;

        public void CalcDamage(IBattleEntityObject attacker, IBattleEntityObject defender, SkillInfo skillInfo, out DamageResult damageResult)
        {
            if (attacker == null || defender == null)
            {
                LogManager.LogError("ֱ�˼�����Բ���Ϊnull");
            }

            //this.attacker = attacker;
            //this.defender = defender;
            //this.skillInfo = skill.SkillInfo;
            ////this.skillMuls = TextUtility.SplitToIntArr(skillInfo.f_skill_mul, 2);

            ////�����˺� = �������˺����������˺�(��ѡ) + �˺����� * �������ԣ��� * �������������ʣ�1 + ������ * �����˺�����* �������������ʡ� * �����Գ������ʣ�1 - ��Ч���� + ���Խ��ͣ���
            ////��������˺�
            //int finalDamage = CalcBaseDamageZone();
            ////���㱩���˺�
            //finalDamage = CalcCritDamageZone(finalDamage);
            ////�����������
            //finalDamage = CalcDefendZone(finalDamage);
            ////���㿹�Գ���
            //finalDamage = CalcResistanceZone(finalDamage);
            //return finalDamage;

            var critValue = attacker.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.TotalCrit);
            var critRate = critValue / 100f;
            var isCrit = Random.Range(0, 1) < critRate;
            damageResult = new DamageResult(attacker, defender, Random.Range(30, 70), skillInfo.f_elementType.ToElementType(), skillInfo.f_damageType.ToDamageType(), isCrit, skillInfo.f_id, skillInfo.f_toughenValue);
        }

        ///// <summary>
        ///// ��������˺���
        ///// </summary>
        ///// <param name="damage"></param>
        ///// <returns></returns>
        //private int CalcBaseDamageZone()
        //{
        //    //�����˺��� = ��ɫ��ά �� ��Ӧ���ʡ����н�ɫ��ά�Թ���Ϊ���������� = ��ֵ �� (1 + �󹥻�) + С��������ֵ���������������㷨Ϊ��ɫ���������� + ��׶����������

        //    //��ȡ�˺�ģ��
        //    E_DamageModel damageModel = (E_DamageModel)skillInfo.f_dmg_model;
        //    //��¼��������
        //    int finalPropertyValue = 0;
        //    switch (damageModel)
        //    {
        //        case E_DamageModel.Life:
        //            //�������������ٷֱȼӳɣ�= ��׶�ӳ� + �����ӳ� + BuffЧ��
        //            float totalHpPercentBonus = (0 + 0 + attacker.BuffController.GetTotalHpPercentBonus()) / 100f;
        //            //С�����������̶���ֵ�ӳɣ�= ��׶�ӳ� + �����ӳ� + BuffЧ��
        //            int totalHpBuildBonus = 0 + 0 + attacker.BuffController.GetTotalHpBuildBonus();
        //            //��������ֵ = ����ɫ��������ֵ + ��׶��������ֵ��* (1 + ������) + С����
        //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicHp + 0) * (1 + totalHpPercentBonus) + totalHpBuildBonus);
        //            break;
        //        case E_DamageModel.NormalAttack:
        //            //�󹥻��������ٷֱȼӳɣ�= ��׶�ӳ� + �����ӳ� + BuffЧ��
        //            float totalAtkPercentBonus = (0 + 0 + attacker.BuffController.GetTotalAtkPercentBonus()) / 100f;
        //            //С�����������̶���ֵ�ӳɣ�= ��׶�ӳ� + �����ӳ� + BuffЧ��
        //            int totalAtkBuildBonus = 0 + 0 + attacker.BuffController.GetTotalAtkBuildBonus();
        //            //���չ����� = ����ɫ���������� + ��׶������������* (1 + �󹥻�) + С����
        //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicAtk + 0) * (1 + totalAtkPercentBonus) + totalAtkBuildBonus);
        //            break;
        //        case E_DamageModel.Defend:
        //            //������������ٷֱȼӳɣ�= ��׶�ӳ� + �����ӳ� + BuffЧ��
        //            float totalDefPercentBonus = (0 + 0 + attacker.BuffController.GetTotalDefPercentBonus()) / 100f;
        //            //С�����������̶���ֵ�ӳɣ�= ��׶�ӳ� + �����ӳ� + BuffЧ��
        //            int totalDefBuildBonus = (0 + 0 + attacker.BuffController.GetTotalDefBuildBonus());
        //            //���շ����� = ����ɫ���������� + ��׶������������* (1 + �����) + С����
        //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicDef + 0) * (1 + totalDefPercentBonus) + totalDefBuildBonus);
        //            break;
        //    }

        //    //�����˺� = ����ģ������ * ���ܱ���(���ݽ�ɫ��ǰ���ܵȼ���ȡ)
        //    return (int)(finalPropertyValue * (this.skillMuls[0] / 100f));
        //}

        ///// <summary>
        ///// ���㱩����
        ///// </summary>
        //private int CalcCritDamageZone(int damage)
        //{
        //    //��ȡ������
        //    float critRate = attacker.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().F_crit / 100f;
        //    float critDmgRate = attacker.GetProperty<BaseProperty>().F_critDmg / 100f;
        //    //�Ƿ񱩻�
        //    bool isCrit = Random.Range(0, 1f) < critRate;
        //    //����
        //    if(isCrit)
        //    {
        //        //�����˺� = ���˺� *��1 + �����˺����ʣ�
        //        return (int)(damage * (1 + critDmgRate));
        //    }
        //    //�ޱ���
        //    else
        //    {
        //        return damage;
        //    }
        //}

        ///// <summary>
        ///// ���������
        ///// </summary>
        ///// <param name="damage"></param>
        ///// <returns></returns>
        //private int CalcDefendZone(int damage)
        //{
        //    /*
        //     * ������ת��Ϊ ���˺����ʡ���ʽ���˺����ʣ�����������= �������ȼ�ϵ�� / (�з�ʵ�ʷ��� + �������ȼ�ϵ��)
        //     * 
        //     * ���У�
        //     * �������ȼ�ϵ�� = 200 + �������ȼ� �� 10
        //     * �з�ʵ�ʷ��� = �з��������� �� (1 + �з������ӳ�) �� (1 - �����ٷֱ�) �� (1 - ���ӷ����ٷֱ�)
        //    */

        //    //������������ٷֱȼӳɣ�= BuffЧ���ӳ�
        //    float totalDefPercentBonus = defender.BuffController.GetTotalDefPercentBonus() / 100f;
        //    //С�����������̶���ֵ�ӳɣ�= BuffЧ���ӳ�
        //    int totalDefBuildBonus = defender.BuffController.GetTotalDefBuildBonus();
        //    //���շ����� = ��ɫ���������� * (1 + �����) + С����
        //    int totalDefValue = (int)(defender.GetProperty<BaseProperty>().F_basicDef + (1 + totalDefPercentBonus) + totalDefBuildBonus);
        //    //�����ٷֱ�֮�� = BuffЧ��Ӱ��
        //    float totalSubDefPercent = defender.BuffController.GetTotalSubDefPercent() / 100f;
        //    //���ӷ����ٷֱ�֮�� = BuffЧ��Ӱ��
        //    float totalIgnoreDefPercent = attacker.BuffController.GetTotalIgnoreDefPercent() / 100f;
        //    //��������
        //    float damageRate = (200 + attacker.GetProperty<BaseProperty>().F_lev * 10) / 
        //                       (totalDefValue * (1 - totalSubDefPercent) * (1 - totalIgnoreDefPercent) + 200 + attacker.GetProperty<BaseProperty>().F_lev * 10);

        //    return (int)(damage * damageRate);
        //}

        ///// <summary>
        ///// ���㿹����
        ///// </summary>
        ///// <param name="damage"></param>
        ///// <returns></returns>
        //private int CalcResistanceZone(int damage)
        //{
        //    /*
        //     * ��Ӧ���Կ��ԣ�
        //     * ���Գ�������(�ٷֱ�) = Clamp(-100, 1 - (�з��������� + ���������� - �����߿��Դ�͸), 90)
        //     * �з��������� = ����������������
        //     * ���������� = BuffЧ������ֵΪ��������ֵΪ���ͣ�
        //     * �����߿��Դ�͸ = BuffЧ��
        //     */

        //    return damage;
        //}
    }
}
