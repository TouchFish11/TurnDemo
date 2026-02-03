using Game.Battle.Enum;
using Game.Battle.Skill.Enum;
using Game.Tasks;
using UnityEngine;

namespace GameHotUpdate.Tasks
{
    public static class IntExtension
    {
        /// <summary>
        /// ��������ת��Ϊ�ַ���
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string TaskTypeToStr(this int i)
        {
            E_TaskType taskType = (E_TaskType)i;
            return taskType switch
            {
                E_TaskType.MainStory => "����",
                E_TaskType.SideStroy => "֧��",
                _ => "ת��ʧ��"
            };
        }

        /// <summary>
        /// ������������ת��Ϊö��
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static E_TaskContentType ToTaskContentType(this int i)
        {
            return (E_TaskContentType)i;
        }

        /// <summary>
        /// ת�����ܷ�Χ����Ϊ�ı�
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string ToSkillRangeTypeText(this int i)
        {
            E_SkillRangeType skillRangeType = (E_SkillRangeType)i;
            return skillRangeType switch
            {
                E_SkillRangeType.Single => "����",
                E_SkillRangeType.Diffusion => "��ɢ",
                E_SkillRangeType.All => "ȫ��",
                _ => "None"
            };
        }

        /// <summary>
        /// ת��Ϊ��������ö��
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static E_SkillType ToSkillType(this int i)
        {
            return (E_SkillType)i;
        }

        /// <summary>
        /// Ԫ������ת��Ϊ��ɫ
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Color ToElementTypeColor(this int i)
        {
            E_ElementType elementType = (E_ElementType)i;
            return elementType switch
            {
                E_ElementType.Fire => Color.red,
                E_ElementType.Ice => Color.blue,
                E_ElementType.Physical => Color.white,
                E_ElementType.Quantum => new Color(128, 0, 128),    // ��ɫ
                _ => Color.white
            };
        }

        /// <summary>
        /// ת��ΪԪ������
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static E_ElementType ToElementType(this int i)
        {
            return (E_ElementType)i;
        }

        /// <summary>
        /// ת��Ϊ�˺�����
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static E_DamageType ToDamageType(this int i)
        {
            return (E_DamageType)i;
        }

        /// <summary>
        /// ת��Ϊ����Ŀ������
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static E_SkillTargetType ToSkillTargetType(this int i)
        {
            return (E_SkillTargetType)i;
        }
    }
}
