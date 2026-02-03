using System.Collections.Generic;
using System.Linq;
using Core.AssetBundles.Management;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.UI;
using Game.Battle;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Property;
using Game.Battle.Status;
using Game.Battle.Status.Enum;
using Game.Objects;
using Game.Tasks;
using GameHotUpdate.Battle.Event.General;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Objects;
using GameHotUpdate.Property;
using GameHotUpdate.Tasks;
using GameHotUpdate.UI.Battle.Status;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.UI.Battle.Role
{
    /// <summary>
    /// ��ɫ״̬UI
    /// </summary>
    public class RoleStateUI : BaseUIBehaviour
    {
        // UI�ؼ����
        [Inject] private Image imgIcon;
        [Inject] private Image imgFade;
        [Inject] private Image imgHp;
        [Inject] private Image imgEnergy;
        [Inject] private Image imgShield;
        [Inject] private ScrollRect svBuffBox;
        [Inject] private TextMeshProUGUI txtBlood;

        // Ѫ����������
        private readonly float fadeSpeed = 1f;    
        // �������
        private int currentShield;
        private int maxShield;
        // �������
        private const float nonFullAhpha = 0.35f;
        // �սἼ����ID
        private int ultimateSkillId;    
        // ��ɫ���
        // �Ƿ񴥷����սἼ����ֹ�ظ�����
        private bool isTriggerUltimate;
        // ս�������Ľӿ�
        private IBattleContext battleContext;
        // ս��ʵ��ӿ�
        private IBattleEntityObject battleEntity;
        // ״̬UI�б�
        private readonly List<StatusGridUI> statusGridUIs = new();

        /// <summary>
        /// ������ɫID
        /// </summary>
        public int RoleId { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            battleContext = ServiceLocator.Get<IBattleManager>().GetContext();
            // ����ս������¼�
            battleContext.GetEventBus().AddListener<HpChangedEvent>(OnHpChanged);
            battleContext.GetEventBus().AddListener<ShieldChangedEvent>(OnShieldChanged);
            battleContext.GetEventBus().AddListener<EnergyChangedEvent>(OnEnergyChangedEvent);
            battleContext.GetEventBus().AddListener<StatusAddedEvent>(OnStatusAddedEvent);
        }

        protected override void OnEnable()
        {
            ServiceLocator.Get<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="ultimateSkillId"></param>
        /// <param name="playerProperty"></param>
        /// <param name="battleEntity"></param>
        public void Init(RoleProperty playerProperty, Sprite icon, int ultimateSkillId, IBattleEntityObject battleEntity)
        {
            this.battleEntity = battleEntity;
            // ��¼�սἼID
            this.ultimateSkillId = ultimateSkillId;
            // ��¼��ɫID
            RoleId = playerProperty.Id;

            var roleInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<RoleInfoContainer>(EConfigLoadType.Editor).dataDic[playerProperty.Id];
            // ����ͼ��
            imgIcon.sprite = icon;
            var propertyComponent = this.battleEntity.GetComponent<PropertyComponent>();

            // ����Ѫ��
            imgHp.fillAmount = imgFade.fillAmount = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp) / (float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp);
            txtBlood.text = $"{propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentHp)}/{(float)propertyComponent.GetPropertyValue(E_DynamicPropertyType.MaxHp)}";

            // ��������
            imgEnergy.color = roleInfo.f_elementType.ToElementTypeColor();
            var currentEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
            var baseEnergy = propertyComponent.GetPropertyValue(E_DynamicPropertyType.BaseEnergy);
            imgEnergy.fillAmount = currentEnergy / (float)baseEnergy;
            imgEnergy.color = new Color(imgEnergy.color.r, imgEnergy.color.g, imgEnergy.color.b, currentEnergy == baseEnergy ? 1 : nonFullAhpha);

            // ����buff�б�
            UpdateStatus();

            // ���û�����
            currentShield = maxShield = 0;
            UpdateShield(currentShield, maxShield);
        }

        /// <summary>
        /// Ѫ���仯�¼��ص�
        /// </summary>
        /// <param name="onHpChangedEvent"></param>
        private void OnHpChanged(HpChangedEvent onHpChangedEvent)
        {
            if (onHpChangedEvent.Target is not PlayerObject || onHpChangedEvent.Target.BattleEntityId != RoleId)
            {
                return;
            }
            
            imgHp.fillAmount = onHpChangedEvent.CurrentHp / (float)onHpChangedEvent.MaxHp;
            txtBlood.text = $"{onHpChangedEvent.CurrentHp}/{onHpChangedEvent.MaxHp}";
        }

        /// <summary>
        /// �����仯�¼��ص�
        /// </summary>
        /// <param name="energyChangedEvent"></param>
        private void OnEnergyChangedEvent(EnergyChangedEvent energyChangedEvent)
        {
            if (energyChangedEvent.Target != battleEntity)
            {
                return;
            }
            imgEnergy.fillAmount = energyChangedEvent.CurrentEnergy / (float)energyChangedEvent.MaxEnergy;
            imgEnergy.color = new Color(imgEnergy.color.r, imgEnergy.color.g, imgEnergy.color.b, energyChangedEvent.CurrentEnergy == energyChangedEvent.MaxEnergy ? 1 : nonFullAhpha);
            if (energyChangedEvent.CurrentEnergy == energyChangedEvent.MaxEnergy)
            {
                isTriggerUltimate = false;
            }
        }

        /// <summary>
        /// ���ܱ仯�¼��ص�
        /// </summary>
        /// <param name="onShieldChangedEvent"></param>
        private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
        {
            if (onShieldChangedEvent.Target != battleEntity)
            {
                return;
            }

            UpdateShield(onShieldChangedEvent.CurrentShield, onShieldChangedEvent.ReferenceShield);
        }

        /// <summary>
        /// ���»�����
        /// </summary>
        /// <param name="currentShield"></param>
        /// <param name="maxShield"></param>
        private void UpdateShield(int currentShield, int maxShield)
        {
            imgShield.fillAmount = maxShield == 0 ? 0 : currentShield / (float)maxShield;
        }

        /// <summary>
        /// ����״̬�¼��ص�
        /// </summary>
        private void OnStatusAddedEvent(StatusAddedEvent statusAddedEvent)
        {
            if (statusAddedEvent.NewStatus.Owner != battleEntity)
            {
                return;
            }

            IStatus status = statusAddedEvent.NewStatus;

            // �����仯�����ô���
            switch ((E_ConflictType)status.StatusProperty.StatusInfo.f_conflictType)
            {
                case E_ConflictType.Add:
                    OnConflict_Add(status);
                    break;
                case E_ConflictType.Lonely:
                    OnConflict_Lonel(status);
                    break;
                case E_ConflictType.Cover:
                    OnConflict_Cover(status);
                    break;
            }
        }

        private async void OnConflict_Add(IStatus status)
        {
            // �ж��Ƿ��и�ID��״̬
            bool hasStatus = statusGridUIs.Any(s => s.GetStatusId() == status.StatusProperty.StatusInfo.f_id);
            if (!hasStatus)
            {
                StatusGridUI statusGridUI = await ServiceLocator.Get<IObjectBuilder>().GetHotfixUIObject<StatusGridUI>(EAssetBundleType.UI, ResKeyCollection.StatusGridUI, svBuffBox.content);
                statusGridUI.Init(status);
                statusGridUIs.Add(statusGridUI);
            }
        }

        private async void OnConflict_Lonel(IStatus newStatus)
        {
            StatusGridUI statusGridUI = await ServiceLocator.Get<IObjectBuilder>().GetHotfixUIObject<StatusGridUI>(EAssetBundleType.UI, ResKeyCollection.StatusGridUI, svBuffBox.content);
            statusGridUI.Init(newStatus);
            statusGridUIs.Add(statusGridUI);
        }

        private async void OnConflict_Cover(IStatus newStatus)
        {
            StatusGridUI statusGrid = statusGridUIs.FirstOrDefault(s => s.GetStatusId() == newStatus.StatusProperty.StatusInfo.f_id);
            // ���뻺���
            ServiceLocator.Get<IPoolManager>().PushObj(statusGrid.gameObject);
            // �����¸���
            StatusGridUI statusGridUI = await ServiceLocator.Get<IObjectBuilder>().GetHotfixUIObject<StatusGridUI>(EAssetBundleType.UI, ResKeyCollection.StatusGridUI, svBuffBox.content);
            statusGridUI.Init(newStatus);
            statusGridUIs.Add(statusGridUI);
        }

        /// <summary>
        /// ����״̬
        /// �غϿ�ʼ����
        /// </summary>
        public void UpdateStatus()
        {
            for (int i = statusGridUIs.Count - 1; i >= 0; i--)
            {
                if (!statusGridUIs[i].IsValid)
                {
                    // �Ƴ���Ч��״̬
                    ServiceLocator.Get<IPoolManager>().PushObj(statusGridUIs[i].gameObject);
                    statusGridUIs.RemoveAt(i);
                }
            }
        }

        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnSkill":
                    if (!isTriggerUltimate)
                    {
                        battleContext.GetEventBus().TriggerEvent(new PlayerTriggerUltimateSkillEvent(battleContext, battleEntity, ultimateSkillId));
                        isTriggerUltimate = true;
                    }
                    break;
            }
        }

        private void OnUpdate()
        {
            // Ѫ�������߼�
            FadeBllood();
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
