using Core.Reflection;
using Core.Service;
using Core.Singleton;
using Game.Battle;
using Game.Battle.Command;
using Game.Battle.Skill;
using Game.Battle.Skill.Interface;
using Game.Battle.TargetSelect;
using GameHotUpdate.Battle.Command;

namespace GameHotUpdate.Battle.Skill.Base
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
            var mainTaget = ServiceLocator.Get<ITargetSelectManager>().GetMainTarget();
            // ͨ��Ŀ��ѡ���������ȡ��������Ŀ��
            var selectedTargets = ServiceLocator.Get<ITargetSelectManager>().GetTargets();
            // ��ʼ������
            skill.Init(mainTaget, selectedTargets);
        }
        
        public void AddSkillCommand(ISkillData skilldata)
        {
            // ��װ����
            var skillCommand = ServiceLocator.Get<IFactoryManager>().GetFactory<ICommandFactory, CommandFactory>().GetSkillCommand(skilldata);
            // ����ָ��
            ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(skillCommand);
        }
    }
}
