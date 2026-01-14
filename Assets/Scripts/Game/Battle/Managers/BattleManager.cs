using Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Battle.Core;

namespace Game.Battle
{
    /// <summary>
    /// ս��������
    /// </summary>
    public class BattleManager : SingletonBase<BattleManager>, IBattleManager
    {
        // ��������
        private int _monsterNum;
        // ս��������
        private IBattleContext context;
        // ʵ��ս����
        private BattlePoint battlePoint; 

        private BattleManager()
        {

        }

        public async Task StartBattle(/* ս����ɫѡ�񣬹���ѡ��ս������ѡ�񣨿�ѡ���� */)
        {
            // ��ʼ��ս��������
            context = new BattleContext();
            // ��ʼ��ս����ع�����
            InitBattle();
            // ��ʼ��ս��
            await context.InitBattle();
            // ��ȡ�����ϵ�ս������󣬳�ʼ��ս�������
            battlePoint = BattlePoint.Instance.InitBattlePoint();
            // �����غ�
            MonoManager.Instance.StartCoroutine(context.GetTurnManager().BattleLoop());
        }

        public IBattleContext GetContext()
        {
            return context;
        }

        private void InitBattle()
        {
            // ����ս��������
            ServiceLocator.Register<ITargetSelectManager>(TargetSelectManager.Instance);
            // ����Ҵ���ʱ����������Ҫ������Ҵ���
            ServiceLocator.Register<IDamageCalcManager>(DamageCalcManager.Instance);
            ServiceLocator.Register<ISkillManager>(SkillManager.Instance);

            // �����˳�ս���¼�
            context.GetEventBus().AddListener<QuitBattleEvent>(OnQuitBattleEvent);
        }

        /// <summary>
        /// �˳�ս���¼��ص�
        /// ����ս�����桢����ս��������ݡ���ʾ�ڱ�������
        /// </summary>
        /// <param name="quitBattleEvent"></param>
        private void OnQuitBattleEvent(QuitBattleEvent quitBattleEvent)
        {
            // ����ս������
            ServiceLocator.Get<IUIManager>().DestroyView();
            // ����ս��
            context.CleanupBattle();
            // ��ʾ�ڱ���
            ShowBackView();
        }

        private async void ShowBackView()
        {
            BackController backController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<BackView, BackModel, BackController>(E_UILayer.Mid);
            //backController.CompletedHide(() =>
            //{
            //    //�л�����
            //    SceneManager.Instance.LoadSceneAsync(ResKeyCollection.MainScene, UnityEngine.SceneManagement.LoadSceneMode.Single, (progress) =>
            //    {
            //        LogManager.Log($"���ؽ��ȣ�{progress}");
            //    }, async () =>
            //    {
            //        // ���ر���
            //        ServiceLocator.Get<IUIManager>().DestroyView();
            //        // ��ʾ������
            //        await ServiceLocator.Get<IUIManager>().CreateViewAsync<MainView, MainModel, MainController>(E_UILayer.Top);
            //    });
            //});
        }
    }
}
