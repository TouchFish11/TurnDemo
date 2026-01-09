using Framework;
using Game.Battle;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 战斗界面初始化器
/// </summary>

public class BattleUIInitializer
{
    private BattleView _view;
    private BattleModel _model;

    public BattleUIInitializer(BattleView view, BattleModel model)
    {
        _view = view;
        _model = model;
    }

    /// <summary>
    /// 初始化玩家UI
    /// </summary>
    /// <param name="battleEntities"></param>
    /// <returns></returns>
    public async Task InitPlayerUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<RoleStateUI> roleStateUIs = new List<RoleStateUI>();
        // 玩家角色显示UI
        foreach (IBattleEntityObject battleEntity in battleEntities)
        {
            RoleStateUI roleStateUI = await ObjectBuilder.GetObject<RoleStateUI>(E_AssetBundleType.UI, ResKeyCollection.RoleStateUI, null);
            int skillId = battleEntity.GetComponent<SkillComponent>().GetUltimateSkill();
            if (skillId != -1)
            {
                Sprite icon = await AssetBundleManager.Instance.LoadAssetAsync<Sprite>(E_AssetBundleType.Texture, ResKeyCollection.WhiteImage);
                roleStateUI.Init(battleEntity.GetComponent<PlayerPropertyComponent>().GetProperty<RoleProperty>(), icon, skillId, battleEntity);
                roleStateUIs.Add(roleStateUI);
            }
        }

        _model.InitRoleStateUI(roleStateUIs);
    }

    /// <summary>
    /// 初始化怪物UI
    /// 依赖玩家相机初始化完毕
    /// </summary>
    /// <param name="battleEntities">传null为隐藏</param>
    /// <returns></returns>
    public async Task InitMonsterUI(IEnumerable<IBattleEntityObject> battleEntities)
    {
        List<NormalMonsterStateUI> normalMonsterStateUIs = new List<NormalMonsterStateUI>();
        if (battleEntities != null)
        {
            // 怪物血量UI
            foreach (IBattleEntityObject battleEntity in battleEntities)
            {
                NormalMonsterStateUI monsterStateUI = await ObjectBuilder.GetObject<NormalMonsterStateUI>(E_AssetBundleType.UI, ResKeyCollection.MonsterStateUI, null);
                if (UIManager.WorldToLocalPointInRectangle(BattlePoint.Instance.CurrentActiveCamera, UIManager.Instance.UICamera, _view.MonsterStateArea, monsterStateUI.gameObject, battleEntity.GameObject.transform.position, Vector2.up * 250))
                {
                    monsterStateUI.Init(battleEntity);
                    normalMonsterStateUIs.Add(monsterStateUI);
                }
            }
        }
        _model.UpdateNormalMonsterState(normalMonsterStateUIs);
    }
}
