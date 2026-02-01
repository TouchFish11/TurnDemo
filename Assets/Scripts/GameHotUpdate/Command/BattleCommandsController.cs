using System.Collections;
using System.Collections.Generic;
using Core.Service;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.UI.Battle;
using GameHotUpdate.UI.Battle.Base;

namespace GameHotUpdate.Command
{
    /// <summary>
    /// ս��ָ�������
    /// </summary>
    public class BattleCommandsController
    {
        // ս�������б�
        private readonly List<ICommand> _battleCommands = new List<ICommand>();
        // ս��������
        private readonly IBattleContext _context;
        // ��ǰִ�е�����
        private ICommand _command;
        // �Ƿ��˳�
        private bool _isQuit;

        public int Count => _battleCommands.Count;

        public BattleCommandsController(IBattleContext context)
        {
            _context = context;
        }

        public IEnumerator ExcuteCommand()
        {
            // ����������û�н�������ִ��
            while ((_command != null || _battleCommands.Count > 0) && !_isQuit)
            {
                GetFirst();
                // ִ�м�������
                yield return _command.Execute(_context);
                _command = null;
                // ��������ִ�к��߼�
                yield return OnPostCommandExcute();
            }

            // ��ǰ��������ִ�к����߼�
            // ,,,
        }

        /// <summary>
        /// ����ִ�к�
        /// </summary>
        private IEnumerator OnPostCommandExcute()
        {
            yield return RemoveDeadMonster();
            // ���ս���Ƿ����
            _isQuit = _context.GetTurnManager().CheckBattleOver();
            // δ������ɸ������Ч����
            FilterInvalidCommand();
        }

        /// <summary>
        /// ִ����������Ƴ������Ĺ���ʵ��
        /// </summary>
        /// <returns></returns>
        private IEnumerator RemoveDeadMonster()
        {
            return _context.GetTurnManager().RemoveDeadMonster();
        }

        /// <summary>
        /// ������Ч����
        /// </summary>
        private void FilterInvalidCommand()
        {
            for (int i = _battleCommands.Count - 1; i >= 0; i--)
            {
                if (!_battleCommands[i].IsValid)
                {
                    _battleCommands.RemoveAt(i);
                }
            }
            // ָ���Ŷӣ�����UI��ʾ
            ((BattleController)ServiceLocator.Get<IBattleUIScheduler>().BattleController).BattleUiManager.UpdateWaitingCommmand(GetCommandSenders());
        }

        /// <summary>
        /// ��ȡ�׸�����
        /// </summary>
        /// <returns></returns>
        public void GetFirst()
        {
            if (_battleCommands.Count > 0)
            {
                _command = _battleCommands[0];
                RemoveFirst();
            }
        }

        /// <summary>
        /// ����ָ��
        /// </summary>
        /// <param name="command"></param>
        public void InsertCommand(ICommand command)
        {
            if (_command == null)
            {
                _command = command;
                return;
            }
            else
            {
                _battleCommands.Add(command);
                // �����ȼ���������
                SortCommand();
                // ָ���Ŷӣ�����UI��ʾ
                ((BattleController)ServiceLocator.Get<IBattleUIScheduler>().BattleController).BattleUiManager.UpdateWaitingCommmand(GetCommandSenders());
            }
        }

        /// <summary>
        /// �Ƴ��׸�����
        /// </summary>
        public void RemoveFirst()
        {
            _battleCommands.RemoveAt(0);
            // ָ���Ŷӣ�����UI��ʾ
            ((BattleController)ServiceLocator.Get<IBattleUIScheduler>().BattleController).BattleUiManager.UpdateWaitingCommmand(GetCommandSenders());
        }

        /// <summary>
        /// ��������
        /// �����ȼ�����
        /// </summary>
        private void SortCommand()
        {
            _battleCommands.Sort((c1, c2) =>
            {
                if (c1.Priority > c2.Priority)
                {
                    return -1;
                }
                else if (c1.Priority < c2.Priority)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });
        }

        /// <summary>
        /// ��ȡ��������б�
        /// </summary>
        /// <returns></returns>
        public List<IBattleEntityObject> GetCommandSenders()
        {
            List<IBattleEntityObject> battleEntities = new List<IBattleEntityObject>(_battleCommands.Count);
            foreach (ICommand command in _battleCommands)
            {
                battleEntities.Add(command.Sender);
            }
            return battleEntities;
        }
    }
}
