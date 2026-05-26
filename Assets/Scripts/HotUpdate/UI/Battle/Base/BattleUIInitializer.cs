using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using HotUpdate.Base;
using HotUpdate.Common;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.UI;
using HotUpdate.Game.Battle.UI.Role;
using UnityEngine;

namespace HotUpdate.UI.Battle.Base
{
    /// <summary>
    /// 战斗UI初始化器
    /// 负责初始化战斗场景中玩家和怪物的UI组件
    /// </summary>
    public class BattleUIInitializer : IBattleUIInitializer
    {
        [Inject] private ObjectSpawner _objectSpawner;
        
        // 战斗视图接口，用于获取UI挂载节点等视图相关信息
        private readonly BattleView _view;
        // 战斗控制器
        private readonly BattleController _battleController;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="view">战斗视图实例</param>
        /// <param name="controller"></param>
        public BattleUIInitializer(BattleView view, BattleController controller)
        {
            _view = view;
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
                var roleStateUI = await _objectSpawner.SpawnAsync<RoleStateUI>(ResKeyCollection.RoleStateUI, _view.PlayerArea);

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
                using var iconHandle = await GameAsset.LoadAssetAsync<Sprite>(iconName);
                // 获取当前实体的玩家属性组件
                var playerPropertyComponent = battleEntity.GetComponent<PlayerPropertyComponent>();
                // 获取角色核心属性数据
                var roleProperty = playerPropertyComponent.GetProperty<RoleProperty>();
                
                // 初始化角色状态UI（传入属性、图标、必杀技ID、战斗实体）
                roleStateUI.Obj.Init(roleProperty, iconHandle.Asset, skillId, battleEntity);
                // 将初始化后的角色状态UI缓存到数据模型中
                _view.InitRoleStateUI(roleStateUI);
            }
        }

        /// <summary>
        /// 初始化怪物UI
        /// 为每个怪物实体创建并初始化普通怪物状态UI（如血条等），支持空参数传入
        /// </summary>
        /// <param name="battleEntities">怪物战斗实体集合</param>
        public async System.Threading.Tasks.Task InitMonsterUIs(IEnumerable<IBattleEntityObject> battleEntities)
        {
            foreach (var battleEntity in battleEntities)
            {
                // 将初始化后的怪物UI缓存
                await _battleController.MonsterStateUIManager.CreateNormalMonsterStateUI(battleEntity, _view.MonsterStateArea);
            }
        }
    }
}