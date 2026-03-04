using System.Collections.Generic;
using Core.Loader.Object;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using Core.Utility;
using GameHotUpdate.Battle.Core;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Property;
using GameHotUpdate.Battle.Toughness;
using GameHotUpdate.Camera;
using GameHotUpdate.Config;
using GameHotUpdate.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Battle.UI.MonsterStateUI
{
    /// <summary>
    /// 普通怪物状态UI控制器
    /// 负责显示怪物的血量、韧性、弱点等状态，并跟随怪物位置更新UI
    /// </summary>
    public class NormalMonsterStateUI : UIBehaviourBase
    {
        // 血量渐变遮罩（用于血量变化时的渐变动画效果）
        [Inject] private Image imgFade;
        // 血量填充图（实时显示当前血量比例）
        [Inject] private Image imgHp;
        // 韧性填充图（显示当前韧性比例）
        [Inject] private Image imgToughness;

        /// <summary>
        /// 弱点图标容器（用于挂载多个弱点图标）
        /// Inject(1) 表示注入索引为1的RectTransform组件
        /// </summary>
        [Inject(1)] private RectTransform WeaknessBar { get; set; }

        private readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        // 血量渐变动画速度（控制fade遮罩的动画速率）
        private const float fadeSpeed = 1f;
        // 弱点图标集合（存储当前怪物的所有弱点图标，便于后续回收）
        private readonly List<Image> weakneses = new();  
        // 绑定的战斗实体对象（当前UI对应的怪物实体）
        public IBattleEntityObject BattleEntity { get; private set; }
        // 怪物状态UI的父节点（用于UI的层级管理和坐标转换）
        private Transform monsterStateArea;
        // 上一帧的位置（暂未使用，预留用于位置平滑处理）
        private Vector3 lastPos;
        // 血条UI的Y轴偏移量（根据怪物配置调整血条在怪物上方的显示位置）
        private float _bloodUiYOffset;

        /// <summary>
        /// 初始化（Awake）：注册战斗事件监听
        /// 在对象创建时执行，订阅血量、韧性相关事件
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            // 获取战斗管理器的事件总线，注册血量变化事件监听
            ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<HpChangedEvent>(OnHpChangedEvent);
            // 注册韧性变化事件监听
            ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessChangedEvent>(OnToughnessChangedEvent);
            // 注册韧性破碎（破防）事件监听
            ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
        }

        /// <summary>
        /// 启用（OnEnable）：注册帧更新回调
        /// UI激活时执行，添加Update监听用于实时更新UI位置和血量动画
        /// </summary>
        protected override void OnEnable()
        {
            // 注册帧更新事件，每帧执行OnUpdate方法
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 初始化普通怪物状态UI
        /// </summary>
        /// <param name="battleEntity">绑定的怪物战斗实体</param>
        /// <param name="monsterStateArea">UI父节点</param>
        public async System.Threading.Tasks.Task Init(IBattleEntityObject battleEntity, Transform monsterStateArea)
        {
            // 回收已存在的弱点图标（避免重复创建，复用对象池）
            foreach (var weaknessIcon in weakneses)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(weaknessIcon.gameObject);
            }
            weakneses.Clear(); // 清空弱点图标集合

            // 绑定战斗实体和UI父节点
            BattleEntity = battleEntity;
            this.monsterStateArea = monsterStateArea;
            _bloodUiYOffset = ((MonsterObject)battleEntity).MonsterInfo.f_statesUiY0ffset;
            
            // 获取怪物属性组件，初始化血量显示
            var propertyComponent = BattleEntity.GetComponent<PropertyComponent>();
            float currentHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp);
            float maxHp = propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
            imgHp.fillAmount = imgFade.fillAmount = currentHp / maxHp; // 同步血量填充值和渐变遮罩值

            // 获取怪物韧性组件，初始化韧性显示
            var toughnessComponent = battleEntity.GetComponent<IToughnessComponent>();
            var currentToughnessValue = toughnessComponent.CurrentToughnessValue;
            var maxToughnessVaue = toughnessComponent.MaxToughnessVaue;
            imgToughness.fillAmount = currentToughnessValue / (float)maxToughnessVaue;

            // 初始化弱点图标：遍历怪物的弱点属性，创建对应元素类型的弱点图标
            foreach (var elementType in toughnessComponent.WeakPropertys)
            {
                // 从资源包加载弱点UI预制体，并挂载到弱点容器下
                var weaknessIconObj = await _prefabLoader.GetGameObjectAsync(AbKeyCollection.Ui, ResKeyCollection.WeaknessUI, WeaknessBar);
                var weaknessIcon = weaknessIconObj.GetComponent<Image>();
                // 设置弱点图标颜色（根据元素类型转换为对应颜色）
                weaknessIcon.color = ((int)elementType).ToElementTypeColor();
                weakneses.Add(weaknessIcon); // 将图标加入集合，便于后续回收
            }
        }

        /// <summary>
        /// 血量变化事件回调
        /// 当监听的血量变化事件触发时，更新血量UI显示
        /// </summary>
        /// <param name="hpChangedEvent">血量变化事件数据</param>
        private void OnHpChangedEvent(HpChangedEvent hpChangedEvent)
        {
            // 过滤事件：仅处理当前绑定怪物的血量变化
            if (hpChangedEvent.Target != BattleEntity)
            {
                return;
            }

            // 更新当前血量填充比例（实时同步血量变化）
            imgHp.fillAmount = hpChangedEvent.CurrentHp / (float)hpChangedEvent.MaxHp;
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
            imgToughness.fillAmount = toughnessChangedEvent.CurrentToughness / (float)toughnessChangedEvent.MaxToughness;
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
            {
                return;
            }

            // 将怪物世界坐标转换为UI本地坐标，并应用Y轴偏移，更新UI位置
            UIUtility.WorldToLocalPointInRectangle(
                ServiceLocator.Get<IBattleCameraManager>().CurrentActiveCamera, // 战斗主相机
                ServiceLocator.Get<IUIManager>().UICamera, // UI相机
                monsterStateArea,    // UI父节点
                gameObject, // 当前UI对象
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
            if (imgFade.fillAmount > imgHp.fillAmount)
            {
                imgFade.fillAmount -= Time.deltaTime * fadeSpeed;
                // 防止过度减少，保证最终值与血量填充量一致
                if (imgFade.fillAmount < imgHp.fillAmount)
                {
                    imgFade.fillAmount = imgHp.fillAmount;
                }
            }
        }

        /// <summary>
        /// 禁用（OnDisable）：移除帧更新监听
        /// UI隐藏时执行，避免无效的帧更新消耗性能
        /// </summary>
        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}