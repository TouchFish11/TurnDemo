using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.UI;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;
using HotUpdate.Game.Battle.Toughness;
using HotUpdate.Game.Battle.Utility;
using UnityEngine;
using UnityEngine.UI;
using StatsComponent = HotUpdate.Game.Battle.Property.StatsComponent;

namespace HotUpdate.UI.Battle.MonsterStateUI
{
    public class MonsterStatusBarLogic : IUILogic<MonsterStatusBar, MonsterStatusBarLogic>, IPoolData
    {
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IBattleCameraManager _battleCameraManager;
        [Inject] private IUIManager _uiManager;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IPoolManager _poolManager;

        // 绑定的战斗实体对象（当前UI对应的怪物实体）
        public IBattleEntityObject BattleEntity { get; private set; }
        // 上一帧的位置（暂未使用，预留用于位置平滑处理）
        private Vector3 lastPos;
        // 血条UI的Y轴偏移量（根据怪物配置调整血条在怪物上方的显示位置）
        private float _bloodUiYOffset;
        
        public MonsterStatusBar View { get; private set; }

        /// <summary>
        /// 初始化普通怪物状态UI
        /// </summary>
        /// <param name="view"></param>
        /// <param name="battleEntity">绑定的怪物战斗实体</param>
        /// <param name="monsterStateArea">UI父节点</param>
        public async Task Init(MonsterStatusBar view, IBattleEntityObject battleEntity, Transform monsterStateArea)
        {
            View = view;
            
            // 回收已存在的弱点图标（避免重复创建，复用对象池）
            foreach (var weaknessIcon in View.Weakneses)
            {
                _objectSpawner.Release(weaknessIcon.gameObject);
            }
            View.Weakneses.Clear(); // 清空弱点图标集合
            
            // 绑定战斗实体和UI父节点
            BattleEntity = battleEntity;
            View.monsterStateArea = monsterStateArea;
            _bloodUiYOffset = ((MonsterObject)battleEntity).MonsterInfo.f_statesUiY0ffset;
            
            // 获取怪物属性组件，初始化血量显示
            var statsComponent = BattleEntity.GetComponent<StatsComponent>();
            var currentHp = statsComponent.CurrentHp;
            var maxHp = statsComponent.GetFinalValue(EStatType.Hp);
            View.imgHp.fillAmount = View.imgFade.fillAmount = currentHp / maxHp; // 同步血量填充值和渐变遮罩值

            // 获取怪物韧性组件，初始化韧性显示
            var toughnessComponent = battleEntity.GetComponent<ToughnessComponent>();
            var currentToughnessValue = toughnessComponent.CurrentToughnessValue;
            var maxToughnessVaue = toughnessComponent.MaxToughnessVaue;
            View.imgToughness.fillAmount = currentToughnessValue / (float)maxToughnessVaue;

            // 初始化弱点图标：遍历怪物的弱点属性，创建对应元素类型的弱点图标
            foreach (var elementType in toughnessComponent.WeakPropertys)
            {
                // 从资源包加载弱点UI预制体，并挂载到弱点容器下
                var weaknessIconObj = await _objectSpawner.SpawnAsync<GameObject>(AssetKeys.WeaknessUI, View.WeaknessBar);
                var weaknessIcon = weaknessIconObj.GetComponent<Image>();
                // 设置弱点图标颜色（根据元素类型转换为对应颜色）
                weaknessIcon.color = ((int)elementType).ToElementTypeColor();
                View.Weakneses.Add(weaknessIcon); // 将图标加入集合，便于后续回收
            }
        }

        public void OnActive()
        {
            _monoAdapter.AddUpdateListener(OnUpdate);
            // 获取战斗管理器的事件总线，注册血量变化事件监听
            BattleEntity.Context.EventBus.AddListener<CurrentHpChangedEvent>(OnHpChangedEvent);
            // 注册韧性变化事件监听
            BattleEntity.Context.EventBus.AddListener<ToughnessChangedEvent>(OnToughnessChangedEvent);
            // 注册韧性击破事件监听
            BattleEntity.Context.EventBus.AddListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
        }
        
        public void OnInActive()
        {
            _monoAdapter.RemoveUpdateListener(OnUpdate);
            // 获取战斗管理器的事件总线，注册血量变化事件监听
            BattleEntity.Context.EventBus.RemoveListener<CurrentHpChangedEvent>(OnHpChangedEvent);
            // 注册韧性变化事件监听
            BattleEntity.Context.EventBus.RemoveListener<ToughnessChangedEvent>(OnToughnessChangedEvent);
            // 注册韧性击破事件监听
            BattleEntity.Context.EventBus.RemoveListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
        }
        
        /// <summary>
        /// 帧更新回调
        /// 每帧执行，处理血量渐变动画和UI跟随逻辑
        /// </summary>
        private void OnUpdate()
        {
            FadeBllood();    // 处理血量渐变动画
            FollowTarget();  // 处理UI跟随怪物位置
        }

        /// <summary>
        /// 跟随目标位置
        /// 将UI位置同步到怪物世界坐标对应的UI坐标，并添加Y轴偏移
        /// </summary>
        private void FollowTarget()
        {
            // 未绑定战斗实体时直接返回
            if (BattleEntity == null)
                return;

            // 将怪物世界坐标转换为UI本地坐标，并应用Y轴偏移，更新UI位置
            UIUtility.WorldToLocalPointInRectangle(
                _battleCameraManager.CurrentActiveCamera, // 战斗主相机
                _uiManager.UICamera, // UI相机
                View.monsterStateArea,    // UI父节点
                View.gameObject, // 当前UI对象
                BattleEntity.GameObject.transform.position + Vector3.up * _bloodUiYOffset  // 怪物世界坐标 
            );
        }

        /// <summary>
        /// 血量渐变动画
        /// 控制fade遮罩的填充量，实现血量变化后的渐变追平效果
        /// </summary>
        private void FadeBllood()
        {
            // 当渐变遮罩的填充量大于当前血量填充量时，逐步减少（追平）
            if (View.imgFade.fillAmount > View.imgHp.fillAmount)
            {
                View.imgFade.fillAmount -= Time.deltaTime * View.fadeSpeed;
                // 防止过度减少，保证最终值与血量填充量一致
                if (View.imgFade.fillAmount < View.imgHp.fillAmount)
                {
                    View.imgFade.fillAmount = View.imgHp.fillAmount;
                }
            }
        }

        /// <summary>
        /// 血量变化事件回调
        /// 当监听的血量变化事件触发时，更新血量UI显示
        /// </summary>
        /// <param name="currentHpChangedEvent">血量变化事件数据</param>
        private void OnHpChangedEvent(CurrentHpChangedEvent currentHpChangedEvent)
        {
            // 过滤事件：仅处理当前绑定怪物的血量变化
            if (currentHpChangedEvent.Target != BattleEntity)
            {
                return;
            }

            // 更新当前血量填充比例（实时同步血量变化）
            View.imgHp.fillAmount = currentHpChangedEvent.CurrentHp / currentHpChangedEvent.MaxHp;
        }

        /// <summary>
        /// 韧性变化事件回调
        /// 当监听的韧性变化事件触发时，更新韧性UI显示
        /// </summary>
        /// <param name="toughnessChangedEvent">韧性变化事件数据</param>
        private void OnToughnessChangedEvent(ToughnessChangedEvent toughnessChangedEvent)
        {
            // 过滤事件：仅处理当前绑定怪物的韧性变化
            if (toughnessChangedEvent.Target != BattleEntity)
            {
                return;
            }

            // 更新当前韧性填充比例（实时同步韧性变化）
            View.imgToughness.fillAmount = toughnessChangedEvent.CurrentToughness / (float)toughnessChangedEvent.MaxToughness;
        }

        /// <summary>
        /// 韧性破碎（破防）事件回调
        /// 当怪物韧性被打空时触发，可在此处理破防后的UI特效/状态变化
        /// </summary>
        /// <param name="toughnessBrokenEvent">韧性破碎事件数据</param>
        private void OnToughnessBrokenEvent(ToughnessBrokenEvent toughnessBrokenEvent)
        {
            // 过滤事件：仅处理当前绑定怪物的韧性破碎
            if (toughnessBrokenEvent.Target != BattleEntity)
            {
                return;
            }

            // 韧性归0时的UI效果处理（预留逻辑，如播放破防动画、隐藏韧性条等）
            // ...
        }
        
        void IPoolData.ResetData()
        {

        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }
    }
}
