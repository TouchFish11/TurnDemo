using Framework;

namespace Game.Battle
{
    /// <summary>
    /// FireFly技能工厂类
    /// </summary>
    public class FireFlySkillFactory : SkillFactory
    {
        public override ISkill CreateSkill(int skillId)
        {
            switch (skillId)
            {
                case 10:
                    return new WeakPointAttackSkill(skillId);
                case 11:
                    return new SummonMimiSkill(skillId);
                case 12:
                    return new SummonMimiSkill(skillId);
                default:
                    LogManager.Log($"未找到技能ID， skillId = {skillId}");
                    return null;
            }
        }
    }

    /// <summary>
    /// FireFly角色类
    /// </summary>
    public class FireFly : PlayerObject
    {
        public override void BattleInit(int roleId, IBattleContext context)
        {
            base.BattleInit(roleId, context);

            // 初始化技能组件
            this.GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new FireFlySkillFactory());
        }


        //public override void BaseInit(int id)
        //{
        //    base.BaseInit(id);

        //    // 测试
        //    // 添加移动、输入组件

        //    CreateCamera();
        //    this.AddComponent<InputComponent>();

        //    this.AddComponent<AnimComponent>();
        //    this.AddComponent<MoveComponent>();
        //    this.AddComponent<InteractComponent>();
        //    this.AddComponent<DialogueComponent>();

        //    // 相机跟随
        //    OrbitCameraController.Instance.SetTarget(this.transform);
        //}

        //private async void CreateCamera()
        //{
        //    await ObjectBuilder.GetOrCreateInstance<OrbitCameraController>(E_AssetBundleType.Camera, ResKeyCollection.MainCamera, null);
        //}
    }
}
