using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.Config;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using Core.Utility;
using Game.Battle;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Toughness;
using Game.Objects;
using Game.Tasks;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Property;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Battle
{
    /// <summary>
    /// ��ͨ����״̬UI
    /// </summary>
    public class NormalMonsterStateUI : BaseUIBehaviour
    {
        // ����Ѫ��
        [Inject] private Image imgFade;
        // Ѫ��                                
        [Inject] private Image imgHp;
        // ������
        [Inject] private Image imgToughness;

        /// <summary>
        /// ������
        /// </summary>
        [Inject(1)] private RectTransform WeaknessBar { get; set; }

        // �����ٶ�
        private const float fadeSpeed = 1f;

        // ����ͼ���б�
        private readonly List<Image> weakneses = new();  
        // ս��ʵ��ӿ�
        public IBattleEntityObject BattleEntity { get; private set; }
        // UI������
        private Transform monsterStateArea;
        // ��һ�ε�λ��
        private Vector3 lastPos;
        
        protected override void Awake()
        {
            base.Awake();
            // �����¼�
            ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<HpChangedEvent>(OnHpChangedEvent);
            ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessChangedEvent>(OnToughnessChangedEvent);
            ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
        }

        protected override void OnEnable()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ��ʼ����ͨ����״̬UI
        /// </summary>
        /// <param name="battleEntity"></param>
        /// <param name="monsterStateArea"></param>
        public async System.Threading.Tasks.Task Init(IBattleEntityObject battleEntity, Transform monsterStateArea)
        {
            foreach (var weaknessIcon in weakneses)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(weaknessIcon.gameObject);
            }
            weakneses.Clear();

            BattleEntity = battleEntity;
            this.monsterStateArea = monsterStateArea;

            PropertyComponent propertyComponent = BattleEntity.GetComponent<PropertyComponent>();
            // ��ʼ��Ѫ��
            imgHp.fillAmount = imgFade.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
            imgToughness.fillAmount = 1;

            // ��ʼ������
            IToughnessComponent toughnessComponent = battleEntity.GetComponent<IToughnessComponent>();
            int currentToughnessValue = toughnessComponent.CurrentToughnessValue;
            int maxToughnessVaue = toughnessComponent.MaxToughnessVaue;
            imgToughness.fillAmount = currentToughnessValue / (float)maxToughnessVaue;

            // ��ʼ������
            foreach (E_ElementType elementType in toughnessComponent.WeakPropertys)
            {
                GameObject weaknessIconObj = await ServiceLocator.Get<IObjectBuilder>().GetGameobject(EAssetBundleType.UI, ResKeyCollection.WeaknessUI, WeaknessBar);
                Image weaknessIcon = weaknessIconObj.GetComponent<Image>();
                weaknessIcon.color = ((int)elementType).ToElementTypeColor();
                weakneses.Add(weaknessIcon);
            }
        }

        /// <summary>
        /// Ѫ���仯�¼��ص�
        /// </summary>
        /// <param name="hpChangedEvent"></param>
        private void OnHpChangedEvent(HpChangedEvent hpChangedEvent)
        {
            if (hpChangedEvent.Target != BattleEntity)
            {
                return;
            }

            // ���µ�ǰѪ��UI
            imgHp.fillAmount = hpChangedEvent.CurrentHp / (float)hpChangedEvent.MaxHp;
        }

        /// <summary>
        /// ���Ա仯�¼��ص�
        /// </summary>
        /// <param name="toughnessChangedEvent"></param>
        private void OnToughnessChangedEvent(ToughnessChangedEvent toughnessChangedEvent)
        {
            if (toughnessChangedEvent.Target != BattleEntity)
            {
                return;
            }

            // ���µ�ǰ������UI
            imgToughness.fillAmount = toughnessChangedEvent.CurrentToughness / (float)toughnessChangedEvent.MaxToughness;
        }

        /// <summary>
        /// �����ƻ��¼��ص�
        /// </summary>
        /// <param name="toughnessBrokenEvent"></param>
        private void OnToughnessBrokenEvent(ToughnessBrokenEvent toughnessBrokenEvent)
        {
            if (toughnessBrokenEvent.Target != BattleEntity)
            {
                return;
            }

            // ����Ϊ0����ʾ����Ч��
            // ...
        }

        /// <summary>
        /// ֡�����¼��ص�
        /// </summary>
        private void OnUpdate()
        {
            FadeBllood();
            FollowTarget();
        }

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        private void FollowTarget()
        {
            if (BattleEntity == null)
            {
                return;
            }

            UIUtility.WorldToLocalPointInRectangle(ServiceLocator.Get<IBattlePoint>().CurrentActiveCamera, ServiceLocator.Get<IUIManager>().UICamera, monsterStateArea, gameObject, BattleEntity.GameObject.transform.position, Vector2.up * 250);
        }

        /// <summary>
        /// Ѫ������
        /// </summary>
        private void FadeBllood()
        {
            if (imgFade.fillAmount > imgHp.fillAmount)
            {
                imgFade.fillAmount -= Time.deltaTime * fadeSpeed;
                if (imgFade.fillAmount < imgHp.fillAmount)
                {
                    imgFade.fillAmount = imgHp.fillAmount;
                }
            }
        }

        protected override void OnDisable()
        {
            ServiceLocator.Get<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
