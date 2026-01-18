using Framework;
using Game;
using Game.Battle;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 战斗点
/// </summary>
public class BattlePoint : SingletonMono<BattlePoint>
{
    private readonly struct PointInfo
    {
        /// <summary>
        /// 点变换
        /// </summary>
        public Transform Point { get; }

        /// <summary>
        /// 该位置对应的角色
        /// </summary>
        public IBattleEntityObject BattleEntity { get; }

        /// <summary>
        /// 怪物中心点x值
        /// </summary>
        public byte MonsterCenterX { get; }

        public PointInfo(Transform point, IBattleEntityObject battleEntity, byte monsterCenterX)
        {
            this.Point = point;
            this.BattleEntity = battleEntity;
            this.MonsterCenterX = monsterCenterX;
        }
    }

    [SerializeField] private List<Transform> playerTrans;

    [SerializeField] private List<Transform> monsterTrans;

    [SerializeField] private Transform monsterPointCenter;

    [SerializeField] private List<Camera> roleCameras;

    // 点信息列表
    private List<PointInfo> pointInfos = new List<PointInfo>();
    // 战斗上下文
    private IBattleContext context;
    // 当前相机
    private Camera _currentCamera;
    // 当前相机旋转角度
    private float currentXAngle;
    // X轴旋转角度限制
    private float minXAngle = -3f;
    private float maxXAngle = 3f;
    // 旋转叠加速度
    [SerializeField] private float rotateAddSpeed = 5f;
    // 旋转灵敏度
    [SerializeField] private float rotateSpeed = 1.5f;
    // 当前怪物数量
    private int currentMonsterCount;

    /// <summary>
    /// 当前激活相机
    /// </summary>
    public Camera CurrentActiveCamera => _currentCamera;

    /// <summary>
    ///  初始化战斗点对象
    /// </summary>
    /// <returns></returns>
    public void InitBattlePoint(IBattleContext context, List<IBattleEntityObject> players)
    {
        this.context = context;

        byte[] bytes = new byte[4] { 6, 4, 2, 0 };
        for (int i = 0; i < players.Count; i++)
        {
            PointInfo pointInfo = new PointInfo(playerTrans[i], players[i], bytes[i]);
            pointInfos.Add(pointInfo);
        }

        // 失活所有相机
        foreach (var camera in roleCameras)
        {
            camera.gameObject.SetActive(false);
        }

        context.GetEventBus().AddListener<TurnStartEvent>(OnTurnStartEvent);
        BattleInputHandler.Instance.OnDrag += OnDrag;
    }

    /// <summary>
    /// 滑动事件回调
    /// </summary>
    /// <param name="deltaX"></param>
    private void OnDrag(float deltaX)
    {
        // 转换为旋转角度
        currentXAngle += deltaX * rotateAddSpeed * Time.deltaTime;
        currentXAngle = Mathf.Clamp(currentXAngle, minXAngle, maxXAngle);
        // 应用旋转（使用欧拉角，直观且易控制）
        Quaternion targetRot = Quaternion.Euler(0, currentXAngle, 0f);
        _currentCamera.transform.localRotation = Quaternion.Slerp(_currentCamera.transform.localRotation, targetRot, Time.deltaTime * rotateSpeed);
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

    public Transform MonsterCenter => monsterPointCenter;

    /// <summary>
    /// 激活指定相机
    /// 传入行动的玩家或被攻击的玩家
    /// </summary>
    /// <param name="battleEntity">当前操作的玩家对象</param>
    public void ActiveCamera(IBattleEntityObject battleEntity)
    {
        if (battleEntity is PlayerObject)
        {
            // 更新相应的玩家点激活/失活
            UpdatePoints(battleEntity.EntityPosIndex);

            // 先更新怪物中心位置
            foreach (var pointInfo in pointInfos)
            {
                if (pointInfo.BattleEntity == battleEntity)
                {
                    Vector3 nowPos = monsterPointCenter.transform.position;
                    nowPos.x = pointInfo.MonsterCenterX;
                    monsterPointCenter.transform.position = nowPos;
                    break;
                }
            }

            // 更新怪物之间的相对位置，居中显示
            int newLiveCount = context.GetLiveMonsterObjects().Count;
            if (currentMonsterCount != newLiveCount)
            {
                List<IBattleEntityObject> monsters = context.GetLiveMonsterObjects();
                // 居中显示，放在索引2位置
                if (newLiveCount == 1)
                {
                    // 更新位置索引
                    MonsterObject monster = (MonsterObject)monsters[0];
                    monster.EntityPosIndex = 2;
                    // 设置父对象
                    monster.GameObject.transform.SetParent(monsterTrans[2], false);
                }
                else if (newLiveCount == 2 || newLiveCount == 3 || newLiveCount == 4)
                {
                    // 从中间往右放
                    for (int i = 0; i < monsters.Count; i++)
                    {
                        int index = i + 1;
                        MonsterObject monster = (MonsterObject)monsters[i];
                        monster.EntityPosIndex = index;
                        // 设置父对象
                        monster.GameObject.transform.SetParent(monsterTrans[index], false);
                    }
                }
                currentMonsterCount = newLiveCount;
            }

            // 激活指定玩家相机
            if (_currentCamera != null)
            {
                _currentCamera.transform.localRotation = Quaternion.identity;
                _currentCamera.gameObject.SetActive(false);
            }
            _currentCamera = roleCameras[battleEntity.EntityPosIndex];
            _currentCamera.gameObject.SetActive(true);

            // 初始化当前旋转角度为相机初始角度
            currentXAngle = 0;
        }
    }

    private void UpdatePoints(int currentPosIndex)
    {
        // TODO：暂时这样写死
        if (currentPosIndex == 0)
        {
            playerTrans[0].gameObject.SetActive(true);
            playerTrans[1].gameObject.SetActive(true);
            playerTrans[2].gameObject.SetActive(true);
            playerTrans[3].gameObject.SetActive(true);
        }
        else if (currentPosIndex == 1)
        {
            playerTrans[0].gameObject.SetActive(false);
            playerTrans[1].gameObject.SetActive(true);
            playerTrans[2].gameObject.SetActive(true);
            playerTrans[3].gameObject.SetActive(true);
        }
        else if (currentPosIndex == 2)
        {
            playerTrans[0].gameObject.SetActive(false);
            playerTrans[1].gameObject.SetActive(false);
            playerTrans[2].gameObject.SetActive(true);
            playerTrans[3].gameObject.SetActive(true);
        }
        else if (currentPosIndex == 3)
        {
            playerTrans[0].gameObject.SetActive(false);
            playerTrans[1].gameObject.SetActive(false);
            playerTrans[2].gameObject.SetActive(false);
            playerTrans[3].gameObject.SetActive(true);
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
