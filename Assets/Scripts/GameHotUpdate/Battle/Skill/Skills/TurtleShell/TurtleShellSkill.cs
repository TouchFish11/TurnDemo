using System.Collections;
using System.Text;
using Core.Config;
using Core.Log;
using Core.Service;
using Game.Animation;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Animation;
using UnityEngine;

namespace GameHotUpdate.Battle.Skill.Skills.TurtleShell
{
    public class TurtleShellSkill : MonsterSkill
    {
        /// <summary>
        /// ����
        /// Ŀǰ�ǹ���ʹ��
        /// </summary>
        public string Attack { get; } = "Attack";

        public TurtleShellSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {
            Caster.GetComponentInChildren<AnimationTrigger>().OnAttack += OnAttack;
        }

        private void OnAttack(int skillId)
        {
            if (skillId != SkillInfo.f_id)
            {
                return;
            }

            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);

            Vector3 mainTarget = MainTarget.GameObject.transform.position;
            Vector3 realTarget = new Vector3(mainTarget.x, 0, mainTarget.z);
            Vector3 caster = Caster.GameObject.transform.position;
            Vector3 realCaster = new Vector3(caster.x, 0, caster.z);

            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position + Vector3.forward, Quaternion.LookRotation(realTarget - realCaster));
            // ������Ч
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_MonsterAttackSkill, projectileTrans, projectileData, vFXInfo);
            StatusAddStrategy?.ToAdd(Caster, AllTargets, statusIds);
        }

        protected override void OnPreCast(IBattleContext context)
        {
            base.OnPreCast(context);
            vFXInfo = new VFXInfo();
            
            StringBuilder sb = new StringBuilder();
            foreach (var battleEntityObject in AllTargets)
            {
                sb.AppendLine($"怪物选择目标：{battleEntityObject}");
            }
            LogManager.Log($"{sb}");
        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            yield return new WaitForSeconds(0.1f);
            yield return ServiceLocator.Get<IAnimationPlayManager>().PlayAnimation(Caster, (E_AnimationType)SkillInfo.f_animationType, AnimationComponent.Skill_Layer_Name, Attack);
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
            // �Ż�����
            yield return new WaitForSeconds(0.8f);
        }
    }
}
