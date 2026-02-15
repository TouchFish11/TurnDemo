using System.Collections.Generic;
using Core.Log;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;

namespace GameHotUpdate.Battle.Utility
{
    /// <summary>
    /// ս����������
    /// </summary>
    public class BattleUtil
    {
        /// <summary>
        /// ��ȡ���ܷ�Χ����Ŀ��
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <param name="rangeType"></param>
        /// <param name="filterObjects"></param>
        /// <param name="finalTargets"></param>
        /// <returns></returns>
        public static void GetRangeTargets(IBattleEntityObject mainTarget, int rangeType, List<IBattleEntityObject> filterObjects, List<IBattleEntityObject> finalTargets)
        {
            switch ((E_SkillRangeType)rangeType)
            {
                case E_SkillRangeType.Single:
                    // ֻ������Ŀ��
                    finalTargets.Add(mainTarget);
                    break;
                case E_SkillRangeType.Diffusion:
                    // ������Ŀ�������Ŀ��
                    finalTargets.Add(mainTarget);
                    if (filterObjects.Count > 1)
                    {
                        var mainIndex = filterObjects.IndexOf(mainTarget);
                        // �����
                        if (mainIndex == 0)
                        {
                            finalTargets.Add(filterObjects[mainIndex + 1]);
                        }
                        // ���Ҷ�
                        else if (mainIndex == filterObjects.Count - 1)
                        {
                            finalTargets.Add(filterObjects[mainIndex - 1]);
                        }
                        // ��������/��
                        else
                        {
                            finalTargets.Add(filterObjects[mainIndex - 1]);
                            finalTargets.Add(filterObjects[mainIndex + 1]);
                        }
                    }
                    break;
                case E_SkillRangeType.All:
                    //����ȫ��Ŀ��
                    finalTargets.AddRange(filterObjects);
                    break;
                default:
                    LogManager.LogError($"{nameof(rangeType)}, {rangeType}");
                    break;
            }
        }
    }
}