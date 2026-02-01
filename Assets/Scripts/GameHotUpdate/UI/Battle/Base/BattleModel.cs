using System.Collections.Generic;
using System.Linq;
using Core.Pool;
using Core.Service;
using Game.Battle.Objects;
using GameHotUpdate.UI.Battle.ActionLine;
using GameHotUpdate.UI.Battle.BattlePoint;
using GameHotUpdate.UI.Battle.Role;
using GameHotUpdate.UI.Battle.SkillKey;
using GameHotUpdate.UI.MVC;

namespace GameHotUpdate.UI.Battle.Base
{
    /// <summary>
    /// ս����������
    /// </summary>
    public class BattleModel : UIModel
    {
        // �ж�������UI�б�
        private readonly List<ActionGridUI> actions = new();
        // ���ܰ���UI�б�
        private readonly List<SkillKeyUI> skillKeyUIs = new();
        // ��ɫ״̬UI�б�
        private readonly List<RoleStateUI> roleStateUIs = new();
        // ��ͨ����״̬UI�б�
        private readonly List<NormalMonsterStateUI> normalMonsterStateUIs = new();
        // ս����UI�б�
        private readonly List<BattlePointUI> battlePointUIs = new();
        // ѡ����UI�б�
        private readonly List<SelectMarkerUI> selectMarkerUIs = new();
        // �ȴ��ж������б�
        private readonly List<WaitingActUI> waitingActUIs = new();
        // ��ǰ�ۼ��˺�
        private long currentCalcDamage;

        /// <summary>
        /// ������ͨ����״̬UI
        /// </summary>
        /// <param name="deadMonster"></param>
        public void HideNormalMonsterStateUI(IBattleEntityObject deadMonster)
        {
            var normalMonsterStateUI = normalMonsterStateUIs.Find((m) => m.BattleEntity == deadMonster);
            normalMonsterStateUIs.Remove(normalMonsterStateUI);
            ServiceLocator.Get<IPoolManager>().PushObj(normalMonsterStateUI.gameObject);
        }

        /// <summary>
        /// ͨ��ID��ȡ��ɫ״̬UI
        /// ʹ��Linq��ѯ
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>δ�ҵ�����null</returns>
        public RoleStateUI GetRoleStateUIById(int roleId)
        {
            return roleStateUIs.FirstOrDefault(r => r.RoleId == roleId);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitingActUI"></param>
        public void CacheWaitingCommmand(WaitingActUI waitingActUI)
        {
            this.waitingActUIs.Add(waitingActUI);
        }

        public void ClearWaitingActUI()
        {
            foreach (var waitingActUI in this.waitingActUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(waitingActUI.gameObject);
            }
            this.waitingActUIs.Clear();
        }

        /// <summary>
        /// ������ͨ����״̬UI
        /// </summary>
        /// <param name="normalMonsterStateUIs"></param>
        public void UpdateNormalMonsterState(IEnumerable<NormalMonsterStateUI> normalMonsterStateUIs)
        {
            foreach (var monsterStateUI in this.normalMonsterStateUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(monsterStateUI.gameObject);
            }
            this.normalMonsterStateUIs.Clear();
            this.normalMonsterStateUIs.AddRange(normalMonsterStateUIs);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearActionBar()
        {
            foreach (var actionGridUI in actions)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(actionGridUI.gameObject);
            }
            actions.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionGridUI"></param>
        public void UpdateAcitonbar(ActionGridUI actionGridUI)
        {
            actions.Add(actionGridUI);
        }

        /// <summary>
        /// ��ȡ���е��ж�����
        /// </summary>
        /// <returns></returns>
        public List<ActionGridUI> GetActionGridUIs()
        {
            return actions;
        }

        /// <summary>
        /// ���ò���UI
        /// </summary>
        /// <param name="skillKeyUIs"></param>
        public void SetOperator(List<SkillKeyUI> skillKeyUIs)
        {
            foreach (SkillKeyUI skillKeyUI in this.skillKeyUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(skillKeyUI.gameObject);
            }

            this.skillKeyUIs.Clear();
            this.skillKeyUIs.AddRange(skillKeyUIs);
        }

        /// <summary>
        /// �������UI
        /// </summary>
        public void ClearOperator()
        {
            foreach (SkillKeyUI skillKeyUI in skillKeyUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(skillKeyUI.gameObject);
            }
            skillKeyUIs.Clear();
        }

        /// <summary>
        /// ����ս������
        /// </summary>
        /// <param name="current"></param>
        /// <param name="battlePointUIs"></param>
        public void UpdateBattlePointCount(int current, IEnumerable<BattlePointUI> battlePointUIs)
        {
            foreach (BattlePointUI battlePointUI in this.battlePointUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(battlePointUI.gameObject);
            }
            this.battlePointUIs.Clear();
            this.battlePointUIs.AddRange(battlePointUIs);
        }

        /// <summary>
        /// ����ѡ����
        /// </summary>
        /// <param name="selectMarkerUIs"></param>
        public void UpdateSelectMarker(List<SelectMarkerUI> selectMarkerUIs)
        {
            ClearSelectMarker();
            this.selectMarkerUIs.AddRange(selectMarkerUIs);
        }

        /// <summary>
        /// ������
        /// </summary>
        public void ClearSelectMarker()
        {
            foreach (SelectMarkerUI selectMarkerUI in selectMarkerUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(selectMarkerUI.gameObject);
            }
            selectMarkerUIs.Clear();
        }

        /// <summary>
        /// ��ʼ����ɫ״̬UI
        /// </summary>
        /// <param name="roleStateUI"></param>
        public void InitRoleStateUI(RoleStateUI roleStateUI)
        {
            this.roleStateUIs.Add(roleStateUI);
        }

        /// <summary>
        /// �����ۼ��˺��ı�
        /// </summary>
        /// <param name="dmg"></param>
        /// <param name="isClear"></param>
        /// <returns></returns>
        public long SetCumulativeDamage(int dmg, bool isClear)
        {
            if (!isClear)
            {
                currentCalcDamage += dmg;
            }
            else
            {
                currentCalcDamage = 0;
            }

            return currentCalcDamage;
        }
    }
}
