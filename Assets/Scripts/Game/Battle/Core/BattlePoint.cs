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

    [SerializeField] private List<Transform> playerTrans;

    [SerializeField] private List<Transform> monsterTrans;

    [SerializeField] private Transform monsterPointCenter;

    [SerializeField] private List<Camera> roleCameras;
    // 当前相机
    private Camera _currentCamera;

    /// <summary>
    /// 当前激活相机
    /// </summary>
    public Camera CurrentActiveCamera => _currentCamera;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public void InitBattlePoint()
    {
        BattleManager.Instance.GetContext().GetEventBus().AddListener<TurnStartEvent>(OnTurnStartEvent);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Transform> GetPlayerTransforms()
    {
        return playerTrans;
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <param name="battleEntity"></param>
    public void ActiveCamera(IBattleEntityObject battleEntity)
    {
        battleEntity.Context.GetTurnManager().UpdateMonsterEntityPoses();

        if (battleEntity is PlayerObject)
        {
            Transform[] transforms = battleEntity.GameObject.GetComponentsInParent<Transform>();

            int index = playerTrans.IndexOf(transforms[1]);
            if (index != -1)
            {
                if (_currentCamera != null)
                {
                    _currentCamera.gameObject.SetActive(false);
                }
                _currentCamera = roleCameras[index];
                _currentCamera.gameObject.SetActive(true);
            }
            else
            {
                LogManager.LogError($"[BattlePoint]");
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private void OnTurnStartEvent(TurnStartEvent turnStartEvent)
    {
        ActiveCamera(turnStartEvent.CurrentBattleEntity);
    }
}
