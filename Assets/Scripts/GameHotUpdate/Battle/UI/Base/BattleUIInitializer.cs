using System;
using System.Collections.Generic;
using Core.Loader.Sprite;
using Core.Loader.UI;
using Core.Log;
using Core.Service;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Property;
using GameHotUpdate.Battle.Skill.Component;
using GameHotUpdate.Battle.Skill.Enum;
using GameHotUpdate.Battle.UI.MonsterStateUI;
using GameHotUpdate.Battle.UI.Role;
using GameHotUpdate.Config;

namespace GameHotUpdate.Battle.UI.Base
{
    /// <summary>
    /// 战斗UI初始化器
    /// 负责初始化战斗场景中玩家和怪物的UI组件
    /// </summary>
    public class BattleUIInitializer
    {
        // 战斗视图接口，用于获取UI挂载节点等视图相关信息
        private readonly BattleView _view;
        // 战斗数据模型接口，用于缓存和管理UI相关数据
        private readonly BattleModel _model;
        //
        private readonly BattleController _battleController;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="view">战斗视图实例</param>
        /// <param name="model">战斗数据模型实例</param>
        /// <param name="controller"></param>
        public BattleUIInitializer(BattleView view, BattleModel model, BattleController controller)
        {
            _view = view;
            _model = model;
            _battleController = controller;
        }

        /// <summary>
        /// 初始化玩家角色UI
        /// 为每个玩家实体创建并初始化角色状态UI，包括属性、图标、必杀技等信息
        /// </summary>
        /// <param name="battleEntities">玩家战斗实体集合</param>
        /// <returns>异步任务</returns>
        public async System.Threading.Tasks.Task InitPlayerUIs(IEnumerable<IBattleEntityObject> battleEntities)
        {
            // 遍历所有玩家战斗实体，逐个创建角色状态UI
            foreach (var battleEntity in battleEntities)
            {
                // 从资源包加载角色状态UI预制体，并挂载到玩家UI区域
                //var roleStateUI = await ObjectBuilder.GetObject<IRoleStateUI>(AbKeyCollection.Ui, ResKeyCollection.RoleStateUI, _view.PlayerArea);
                var roleStateUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<RoleStateUI>(AbKeyCollection.Ui, ResKeyCollection.RoleStateUI, _view.PlayerArea);
                LogManager.Log($"{nameof(InitPlayerUIs)}：{roleStateUI}-{roleStateUI}");
                // 获取当前实体的技能组件，用于查找必杀技
                var skillComponent = battleEntity.GetComponent<SkillComponent>();
                var skillId = -1;
                // 遍历技能列表，筛选出必杀技（终极技能）并记录其ID
                foreach (var skill in skillComponent.GetSkills())
                {
                    if (skill.SkillInfo.f_SkillType != (byte)E_SkillType.UltimateSkill)
                    {
                        continue;
                    }
                    
                    skillId = skill.SkillInfo.f_id;
                    break;
                }

                // 若未找到必杀技，则跳过当前实体的UI初始化
                if (skillId == -1)
                {
                    continue;
                }
                
                // 根据战斗实体获取对应的图标名称
                var iconName = BattleUIManager.GetIconByEntity(battleEntity);
                // 从图集加载角色图标
                var icon = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Icon_BattleEntity, iconName);
                // 获取当前实体的玩家属性组件
                var playerPropertyComponent = battleEntity.GetComponent<PlayerPropertyComponent>();
                // 获取角色核心属性数据
                var roleProperty = playerPropertyComponent.GetProperty<RoleProperty>();
                
                // 初始化角色状态UI（传入属性、图标、必杀技ID、战斗实体）
                roleStateUI.Init(roleProperty, icon, skillId, battleEntity);
                // 将初始化后的角色状态UI缓存到数据模型中
                _model.InitRoleStateUI(roleStateUI);
            }
        }

        /// <summary>
        /// 初始化怪物UI
        /// 为每个怪物实体创建并初始化普通怪物状态UI（如血条等），支持空参数传入
        /// </summary>
        /// <param name="battleEntities">怪物战斗实体集合</param>
        public async System.Threading.Tasks.Task InitMonsterUIs(IEnumerable<IBattleEntityObject> battleEntities)
        {
            try
            {
                foreach (var battleEntity in battleEntities)
                {
                    // 从资源包加载怪物状态UI预制体，并挂载到怪物UI区域
                    var monsterStateUI = await ServiceLocator.Get<IUiLoader>().GetUIObject<NormalMonsterStateUI>(AbKeyCollection.Ui, ResKeyCollection.MonsterStateUI, _view.MonsterStateArea);
                    // 初始化怪物状态UI（传入战斗实体、UI挂载区域）
                    await monsterStateUI.Init(battleEntity, _view.MonsterStateArea);
                    // 将初始化后的怪物UI缓存
                    _battleController.MonsterStateUIManager.AddNormalMonsterStateUI(battleEntity, monsterStateUI);
                }
            }
            catch (Exception e)
            {
                // 捕获初始化过程中的异常并记录错误日志
                LogManager.LogError($"{nameof(BattleUIInitializer)}.{nameof(InitMonsterUIs)}: {e.Message}");
            }
        }
    }
}