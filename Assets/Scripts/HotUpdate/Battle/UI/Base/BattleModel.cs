using System.Collections.Generic;
using System.Linq;
using Core.Pool;
using Core.Service;
using Core.UI.MVC;
using HotUpdate.Battle.UI.ActionLine;
using HotUpdate.Battle.UI.BattlePoint;
using HotUpdate.Battle.UI.Role;
using HotUpdate.Battle.UI.SkillKey;

namespace HotUpdate.Battle.UI.Base
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
        // ս����UI�б�
        private readonly List<BattlePointUI> battlePointUIs = new();
        // ѡ����UI�б�
        private readonly List<SelectMarkerUI> selectMarkerUIs = new();
        // �ȴ��ж������б�
        private readonly List<WaitingActUI> waitingActUIs = new();
        // ��ǰ�ۼ��˺�
        private long currentCalcDamage;

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
            waitingActUIs.Add(waitingActUI);
        }

        public void ClearWaitingActUI()
        {
            foreach (var waitingActUI in waitingActUIs)
            {
                ServiceLocator.Get<IPoolManager>().PushObj(waitingActUI.gameObject);
            }
            waitingActUIs.Clear();
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
        ///  缓存目标标记
        /// </summary>
        /// <param name="selectMarkerUI"></param>
        public void AddSelectMarker(SelectMarkerUI selectMarkerUI)
        {
            selectMarkerUIs.Add(selectMarkerUI);
        }
        
        /// <summary>
        /// 清理所有标记
        /// </summary>
        public void ClearSelectMarkers()
        {
            foreach (var selectMarkerUI in selectMarkerUIs)
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
            roleStateUIs.Add(roleStateUI);
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
