using Framework;
using Game;
using Game.Battle;
using Game.Battle.Core;
using static UnityEditor.Timeline.TimelinePlaybackControls;

/// <summary>
/// ս�������¼�������
/// </summary>
public class BattleEventProcessor
{
    private BattleController _battleController;
    private BattleUIManager _uiManager;
    private BattleUIInitializer _uiInitializer;

    public BattleEventProcessor(BattleController battleController, BattleUIManager uiManager, BattleUIInitializer uiInitializer)
    {
        _battleController = battleController;
        _uiManager = uiManager;
        _uiInitializer = uiInitializer;
    }

    /// <summary>
    /// ͳһע������ս���¼�
    /// </summary>
    public void RegisterBattleEvents(IBattleEventBus eventBus)
    {
        eventBus.AddListener<TurnStartEvent>(OnTurnStart);
        eventBus.AddListener<TurnEndEvent>(OnTurnEnd);
        eventBus.AddListener<OnBattlePointCountChangedEvent>(OnBattlePointCountChanged);
        eventBus.AddListener<SelectTargetEvent>(OnTargetSelectionChanged);
        eventBus.AddListener<HpChangedEvent>(OnHpChanged);
        eventBus.AddListener<ShieldChangedEvent>(OnShieldChanged);
        eventBus.AddListener<ApplyDamageEvent>(OnTakeDamage);
        eventBus.AddListener<PlayerReleaseSkillEvent>(OnPlayerReleaseSkillEvent);
        eventBus.AddListener<ActionBarSortPostEvent>(OnActionBarSortPostEvent);
        eventBus.AddListener<TurnStartStatusChangedEvent>(OnTurnStartStatusChangedEvent);
        eventBus.AddListener<StatusAddedEvent>(OnStatusAddedEvent);
        eventBus.AddListener<BattleOverEvent>(OnBattleOverEvent);
        eventBus.AddListener<MonsterDeadEvent>(OnMonsterDeadEvent);
    }

    /// <summary>
    /// �غϿ�ʼ�¼�����
    /// �������/����UI
    /// TODO�����Ż�Ϊͨ���ⲿ�����߼�����ʵ���߼�
    /// </summary>
    /// <param name="turnStartEvent"></param>
    private void OnTurnStart(TurnStartEvent turnStartEvent)
    {
        //if (turnStartEvent.CurrentBattleEntity is MonsterObject)
        //{
        //    // TODO����ʱд��������ع���UI�������Ż������߼�

        //    // ���﹥��ʱ��ȥ���ع���UI

        //}
        //else
        //{
        //    // ������λ�ò�ͬ����Ҫÿ�غϿ�ʼʱ���¹���UIλ��
        //    await _uiInitializer.InitMonsterUI(turnStartEvent.Context.GetMonsterObjects());
        //}

        // ��ʾ����UI
        _uiInitializer.InitMonsterUI(turnStartEvent.Context.GetMonsterObjects());

        if (turnStartEvent.CurrentBattleEntity is PlayerObject)
        {
            // ����Ŀ��ѡ��
            TargetSelectManager.Instance.ActiveSelectTarget();
            // �����ж���ʾ
            _uiManager.SetActTipActive(BattleUIManager.E_ActTipType.Hide);
            // ���µ�ǰ����UI
            _uiManager.UpdateOperator(turnStartEvent.CurrentBattleEntity, IFactory.GetTypeInstance<SkillKeyUIDataProviderFactory, BaseSkillKeyUIDataProvider>());
        }
        else if (turnStartEvent.CurrentBattleEntity is MonsterObject)
        {
            // ����Ŀ��ѡ��
            ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            // ���UI
            _uiManager.ClearSelectMarker();
            // �������UI
            _uiManager.SetOperator(null);
            // ��������ж���ʾ
            _uiManager.SetActTipActive(BattleUIManager.E_ActTipType.Monster);
        }
    }

    /// <summary>
    /// �غϽ����¼�����
    /// </summary>
    /// <param name="turnEndEvent"></param>
    private void OnTurnEnd(TurnEndEvent turnEndEvent)
    {

    }

    /// <summary>
    /// �غϿ�ʼ״̬�仯�¼��ص�
    /// </summary>
    /// <param name="turnStartStatusChangedEvent"></param>
    private void OnTurnStartStatusChangedEvent(TurnStartStatusChangedEvent turnStartStatusChangedEvent)
    {
        // ����ָ�����״̬��
        _uiManager.UpdatePlayerStatuebar(turnStartStatusChangedEvent.CurrentBattleEntity);
    }

    /// <summary>
    /// ���������¼��ص�
    /// </summary>
    /// <param name="monsterDeadEvent"></param>
    private void OnMonsterDeadEvent(MonsterDeadEvent monsterDeadEvent)
    {
        _uiManager.HideNormalMonsterStateUI(monsterDeadEvent.DeadMonster);
    }

    /// <summary>
    /// ״̬�����¼��ص�
    /// ��ʾ״̬�����ı�Ч��
    /// </summary>
    /// <param name="statusAddedEvent"></param>
    private void OnStatusAddedEvent(StatusAddedEvent statusAddedEvent)
    {
        _uiManager.ShowStatusText(statusAddedEvent.NewStatus);
    }

    /// <summary>
    /// �ж���������¼��ص�
    /// </summary>
    /// <param name="actionBarSortPostEvent"></param>
    private void OnActionBarSortPostEvent(ActionBarSortPostEvent actionBarSortPostEvent)
    {
        _uiManager.UpdateActionBar(actionBarSortPostEvent.battleEntities);
    }

    /// <summary>
    /// �ܵ��˺��¼��ص�
    /// ��ʾ�˺��ı�
    /// </summary>
    /// <param name="applyDamageEvent"></param>
    private void OnTakeDamage(ApplyDamageEvent applyDamageEvent)
    {
        _uiManager.ShowDamageText(applyDamageEvent.DamageResult);
    }

    /// <summary>
    /// ����ͷż����¼��ص�
    /// </summary>
    /// <param name="playerReleaseSkillEvent"></param>
    private void OnPlayerReleaseSkillEvent(PlayerReleaseSkillEvent playerReleaseSkillEvent)
    {
        // ����Ŀ��ѡ��
        ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
        // ������UI
        _uiManager.ClearSelectMarker();
        // �������UI
        _uiManager.SetOperator(null);
        // ��������ж���ʾ
        _uiManager.SetActTipActive(BattleUIManager.E_ActTipType.Player);
    }

    /// <summary>
    /// Ѫ���仯�ص��¼�
    /// ��ʾ�ָ��ı�
    /// </summary>
    /// <param name="hpChangedEvent"></param>
    private void OnHpChanged(HpChangedEvent hpChangedEvent)
    {
        if (hpChangedEvent.DeltaHp < 0)
        {
            _uiManager.ShowHealText(hpChangedEvent.Target, hpChangedEvent.DeltaHp);
        }
    }

    /// <summary>
    /// ���ܱ仯�¼��ص�
    /// ��ʾ���ܱ仯�ı�
    /// </summary>
    /// <param name="onShieldChangedEvent"></param>
    private void OnShieldChanged(ShieldChangedEvent onShieldChangedEvent)
    {
        if (onShieldChangedEvent.DeltaShield < 0)
        {
            // ����������ʾ

        }
        else
        {
            // ���ܼ�����ʾ

        }
    }

    /// <summary>
    /// Ŀ��ѡ��仯�¼��ص�
    /// </summary>
    /// <param name="selectTargetEvent"></param>
    private void OnTargetSelectionChanged(SelectTargetEvent selectTargetEvent)
    {
        if (selectTargetEvent.MainTarget is PlayerObject)
        {
            return;
        }

        // ����Ŀ����UI��ʾ
        _uiManager.UpdateTargetMarker(selectTargetEvent.SelectedTargets);
        // �����ж���Ŀ�������ʾ
        _uiManager.UpdateActionGridHighlight(selectTargetEvent.SelectedTargets);
    }

    /// <summary>
    /// ս����仯�¼��ص�
    /// </summary>
    /// <param name="battlePointCountChanged"></param>
    private async void OnBattlePointCountChanged(OnBattlePointCountChangedEvent battlePointCountChanged)
    {
        await _uiManager.UpdateBattlePointCount(battlePointCountChanged.CurentBattlePointCount, battlePointCountChanged.MaxBattlePointCount);
    }

    /// <summary>
    /// ս�������¼��ص�
    /// ��ʾս������UI
    /// </summary>
    private void OnBattleOverEvent(BattleOverEvent battleOverEvent)
    {
        _uiManager.ShowBattleOver(battleOverEvent.Context);
    }
}
