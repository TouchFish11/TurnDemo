using System.Collections.Generic;
using Core.Log;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Objects;

namespace GameHotUpdate.Utility
{
    public enum E_CharacterType
    {
        /// <summary>
        /// ��ҽ�ɫ
        /// </summary>
        PlayerCharacter,
        /// <summary>
        /// �����ɫ
        /// </summary>
        MonsterCharacter,
    }

    /// <summary>
    /// ս����������
    /// </summary>
    public class BattleUtil
    {
        /// <summary>
        /// ��ȡ���ܷ�Χ����Ŀ��
        /// </summary>
        /// <param name="characterType"></param>
        /// <param name="mainTarget"></param>
        /// <param name="rangeType"></param>
        /// <param name="finalTargets"></param>
        /// <returns></returns>
        public static void GetRangeTargets(E_CharacterType characterType, IBattleEntityObject mainTarget, int rangeType, List<IBattleEntityObject> finalTargets)
        {
            var allTargets = new List<IBattleEntityObject>();
            var context = mainTarget.Context;

            // �ж�ѡ����
            switch (characterType)
            {
                case E_CharacterType.PlayerCharacter:
                    // ��Ŀ������ҽ�ɫ
                    if (mainTarget is PlayerObject)
                    {
                        context.GetAlivePlayerEntitys(allTargets);
                    }
                    // ��Ŀ���ǹ����ɫ
                    else
                    {
                        context.GetAliveMonsterEntitys(allTargets);
                    }
                    break;
                case E_CharacterType.MonsterCharacter:
                    // ��Ŀ���ǹ����ɫ
                    if (mainTarget is MonsterObject)
                    {
                        context.GetAliveMonsterEntitys(allTargets);
                    }
                    // ��Ŀ������ҽ�ɫ
                    else
                    {
                        context.GetAlivePlayerEntitys(allTargets);
                    }
                    break;
                default:
                    LogManager.LogError($"{nameof(characterType)}, {characterType}");
                    break;
            }

            switch ((E_SkillRangeType)rangeType)
            {
                case E_SkillRangeType.Single:
                    // ֻ������Ŀ��
                    finalTargets.Add(mainTarget);
                    break;
                case E_SkillRangeType.Diffusion:
                    // ������Ŀ�������Ŀ��
                    finalTargets.Add(mainTarget);
                    if (allTargets.Count > 1)
                    {
                        var mainIndex = allTargets.IndexOf(mainTarget);
                        // �����
                        if (mainIndex == 0)
                        {
                            finalTargets.Add(allTargets[mainIndex + 1]);
                        }
                        // ���Ҷ�
                        else if (mainIndex == allTargets.Count - 1)
                        {
                            finalTargets.Add(allTargets[mainIndex - 1]);
                        }
                        // ��������/��
                        else
                        {
                            finalTargets.Add(allTargets[mainIndex - 1]);
                            finalTargets.Add(allTargets[mainIndex + 1]);
                        }
                    }
                    break;
                case E_SkillRangeType.All:
                    //����ȫ��Ŀ��
                    finalTargets.AddRange(allTargets);
                    break;
                default:
                    LogManager.LogError($"{nameof(rangeType)}, {rangeType}");
                    break;
            }
        }
    }
}