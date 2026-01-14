using Framework;
using Game;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ս����
/// </summary>
public class BattlePoint : SingletonMono<BattlePoint>
{
    // ���λ���б�
    [SerializeField] private List<Transform> playerTrans;
    // ����λ���б�
    [SerializeField] private List<Transform> monsterTrans;
    // ����λ�����ĵ�
    [SerializeField] private Transform monsterPointCenter;
    // ��ɫ����б�
    [SerializeField] private List<Camera> roleCameras;
    // ��ǰ��������
    private Camera _currentCamera;

    /// <summary>
    /// ��������
    /// </summary>
    public Camera CurrentActiveCamera => _currentCamera;

    /// <summary>
    /// ��ʼ��ս����
    /// </summary>
    /// <returns></returns>
    public BattlePoint InitBattlePoint()
    {
        // ������ɫ�غϿ�ʼ�¼�
        BattleManager.Instance.GetContext().GetEventBus().AddListener<TurnStartEvent>(OnTurnStartEvent);
        return Instance;
    }

    /// <summary>
    /// ��ȡ���еĽ�ɫλ�õ�
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Transform> GetPlayerTransforms()
    {
        return playerTrans;
    }

    /// <summary>
    /// ��ȡ���еĹ���λ�õ�
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
    /// ��ȡ���������
    /// </summary>
    /// <returns></returns>
    public Transform GetMonsterPointCenter() => monsterPointCenter;

    /// <summary>
    /// ����ָ����ҽ�ɫ���
    /// ����ж�ʱ����ָ����������﹥����Ҽ��������ҵ����
    /// </summary>
    /// <param name="battleEntity"></param>
    public void ActiveCamera(IBattleEntityObject battleEntity)
    {
        battleEntity.Context.GetTurnManager().UpdateMonsterEntityPoses();

        if (battleEntity is PlayerObject)
        {
            Transform[] transforms = battleEntity.GameObject.GetComponentsInParent<Transform>();
            // transforms[1]�ǻ�ȡ������λ�ã���GetComponentsInParent������Լ���λ��
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
                LogManager.LogError($"[BattlePoint]��δ�ҵ�λ��");
            }
        }
    }

    /// <summary>
    /// �غϿ�ʼ�¼��ص�
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private void OnTurnStartEvent(TurnStartEvent turnStartEvent)
    {
        ActiveCamera(turnStartEvent.CurrentBattleEntity);
    }
}
