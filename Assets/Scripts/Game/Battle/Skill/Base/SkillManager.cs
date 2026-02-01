using System.Collections.Generic;
using Core.Reflection;
using Core.Service;
using Core.Singleton;
using Game.Battle.Command;
using Game.Battle.Objects;
using Game.Battle.Skill.Interface;
using Game.Battle.TargetSelect;

namespace Game.Battle.Skill.Base
{
    /// <summary>
    /// ���ܹ�����
    /// </summary>
    public class SkillManager : SingletonBase<SkillManager>, ISkillManager
    {
        private SkillManager()
        {

        }

        public void InitSkillTarget(ISkill skill)
        {
            // ͨ��Ŀ��ѡ���������ȡ������Ŀ��
            IBattleEntityObject mainTaget = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();
            // ͨ��Ŀ��ѡ���������ȡ��������Ŀ��
            List<IBattleEntityObject> selectedTargets = ServiceLocator.Get<ITargetSelectManager>().GetTargets();
            // ��ʼ������
            skill.Init(mainTaget, selectedTargets);
        }


        public void AddSkillCommand(ISkill skill)
        {
            // ��װ����
            var skillCommand = ServiceLocator.Get<IFactoryManager>().GetFactory<ICommandFactory, CommandFactory>().GetSkillCommand(skill);
            // ����ָ��
            ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(skillCommand);
        }
    }
}
