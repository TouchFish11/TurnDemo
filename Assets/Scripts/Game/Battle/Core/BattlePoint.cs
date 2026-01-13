using Framework;
using Game;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 战斗点
/// </summary>
public class BattlePoint : SingletonMono<BattlePoint>
{
    // 玩家位置列表
    [SerializeField] private List<Transform> playerTrans;
    // 怪物位置列表
    [SerializeField] private List<Transform> monsterTrans;
    // 怪物位置中心点
    [SerializeField] private Transform monsterPointCenter;
    // 角色相机列表
    [SerializeField] private List<Camera> roleCameras;
    // 当前激活的相机
    private Camera currentCamera;

    /// <summary>
    /// 激活的相机
    /// </summary>
    public Camera CurrentActiveCamera => currentCamera;

    /// <summary>
    /// 初始化战斗点
    /// </summary>
    /// <returns></returns>
    public BattlePoint InitBattlePoint()
    {
        // 监听角色回合开始事件
        BattleManager.Instance.GetContext().GetEventBus().AddListener<TurnStartEvent>(OnTurnStartEvent);
        return Instance;
    }

    /// <summary>
    /// 获取所有的角色位置点
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Transform> GetPlayerTransforms()
    {
        return playerTrans;
    }

    /// <summary>
    /// 获取所有的怪物位置点
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Transform> GetMonsterTransforms()
    {
        return monsterTrans;
    }

    public Transform GetPlayerTransByIndex(int index)
    {
        return playerTrans[index];
    }

    public Transform GetMonsterTransByIndex(int index)
    {
        return monsterTrans[index];
    }

    /// <summary>
    /// 获取怪物点中心
    /// </summary>
    /// <returns></returns>
    public Transform GetMonsterPointCenter() => monsterPointCenter;

    /// <summary>
    /// 激活指定玩家角色相机
    /// 玩家行动时激活指定相机、怪物攻击玩家激活被攻击玩家的相机
    /// </summary>
    /// <param name="battleEntity"></param>
    public void ActiveCamera(IBattleEntityObject battleEntity)
    {
        battleEntity.Context.GetTurnManager().UpdateMonsterEntityPoses();

        if (battleEntity is PlayerObject)
        {
            Transform[] transforms = battleEntity.GameObject.GetComponentsInParent<Transform>();
            // transforms[1]是获取父对象位置，而GetComponentsInParent会包含自己的位置
            int index = playerTrans.IndexOf(transforms[1]);
            if (index != -1)
            {
                if (currentCamera != null)
                {
                    currentCamera.gameObject.SetActive(false);
                }
                currentCamera = roleCameras[index];
                currentCamera.gameObject.SetActive(true);
            }
            else
            {
                LogManager.LogError($"[BattlePoint]：未找到位置");
            }
        }
    }

    /// <summary>
    /// 回合开始事件回调
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private void OnTurnStartEvent(TurnStartEvent turnStartEvent)
    {
        ActiveCamera(turnStartEvent.CurrentBattleEntity);
    }
}
