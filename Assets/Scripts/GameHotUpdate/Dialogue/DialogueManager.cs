using System;
using System.Collections;
using System.Text;
using Core.DataPersistence.Binary;
using Core.Global;
using Core.GlobalEvent;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Singleton;
using Core.UI;
using Core.Utility;
using Game.Dialogue;
using GameHotUpdate.Config;
using GameHotUpdate.Dialogue.UI;
using GameHotUpdate.Tasks;
using UnityEngine;

namespace GameHotUpdate.Dialogue
{
    /// <summary>
    /// �Ի�������
    /// </summary>
    public class DialogueManager : SingletonBase<DialogueManager>, IDialogueManager
    {
        // �Ƿ����ô��ֻ�Ч����ȫ�����ã��������ѡ��
        private bool enableTypewriter;
        // ��ǰ����Ի��Ƿ����
        private bool dialogueOver;
        // ���ֻ�Ч��Э��
        private Coroutine typewriterCor;
        // ��ǰ�Ի���Ϣ
        private DialogueInfo currentDialogueInfo;
        // �Ի����������
        private DialogueController dialogueController;
        // ��ǰ�Ի���NpcId
        private NpcInfo npcInfo;

        /// <summary>
        /// ���ֻ����ּ��
        /// </summary>
        private const float TypewriterInterval = 0.05f;

        public event Action OnDialogueStart;

        public event Action OnDialogueEnd;

        public event Action OnBranchSelected;

        public event Action<DialogueInfo> OnSingleDialogueStart;

        public event Action OnSingleDialogueEnd;

        public bool IsDialogueActive { get; private set; } 

        private DialogueManager()
        {
            enableTypewriter = true;
            GameSettingManager.Instance.OnEnableTypewriterChanged += OnEnableTypewriterChanged;
        }

        private void OnEnableTypewriterChanged(bool value)
        {
            enableTypewriter = value;
        }

        public async void StartDialogue(int startDialogueId)
        {
            if (IsDialogueActive)
            {
                return;
            }

            // ��ȡ�Ի����������
            dialogueController = await ServiceLocator.Get<IUIManager>().CreateViewAsync<DialogueView, DialogueModel, DialogueController>(AbKeyCollection.Ui, E_UILayer.Mid, ResKeyCollection.DialogueView);
            // �Ի���
            IsDialogueActive = true;
            // �������Ի���ʼ���¼�
            OnDialogueStart?.Invoke();
            // ��ʾ��ǰ�Ի�
            ShowCurrentDialogue(startDialogueId);
        }

        /// <summary>
        /// ��ʾ��ǰ�Ի�
        /// </summary>
        private void ShowCurrentDialogue(int startDialogueId)
        {
            if (startDialogueId == -1)
            {
                EndDialogue();
                return;
            }

            // ��ȡ��ID�ĶԻ���Ϣ
            var dialogueInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<DialogueInfoContainer>(EConfigLoadType.Excel).dataDic[startDialogueId];
            // ��¼��ǰ�Ի���Ϣ
            currentDialogueInfo = dialogueInfo;
            // ��¼��ǰ�Ի���Npc��Ϣ
            npcInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[dialogueInfo.f_speakerId];

            if (enableTypewriter)
            {
                dialogueOver = false;
                // ������ʾ
                typewriterCor = ServiceLocator.Get<IMonoAdapter>().StartCoroutine(ApplyTypewriter());
                OnSingleDialogueStart?.Invoke(currentDialogueInfo);
            }
            else
            {
                dialogueOver = true;
                // ֱ����ʾ�Ի��ı�
                dialogueController.ShowDialogueText(npcInfo.f_speakerName, currentDialogueInfo.f_dialgueText);
                // ��ʾ�Ի���֧�����У�
                ShowBranchOpt();
            }
        }

        /// <summary>
        /// Ӧ�ô��ֻ�Ч��
        /// </summary>
        /// <returns></returns>
        private IEnumerator ApplyTypewriter()
        {
            var text = currentDialogueInfo.f_dialgueText;
            var sb = new StringBuilder(text.Length);
            foreach (var t in text)
            {
                sb.Append(t);
                dialogueController.ShowDialogueText(npcInfo.f_speakerName, sb.ToString());
                yield return new WaitForSeconds(TypewriterInterval);
            }
            dialogueOver = true;
            OnSingleDialogueEnd?.Invoke();
            ShowBranchOpt();
        }

        public void NextDialogue()
        {
            if (!IsDialogueActive)
            {
                return;
            }

            // �����ô��ֻ�Ч������δ���ʱ����ֹͣЧ��ֱ����ʾ�����ı�
            if (!dialogueOver && typewriterCor != null)
            {
                ServiceLocator.Get<IMonoAdapter>().StopCoroutine(typewriterCor);
                dialogueController.ShowDialogueText(npcInfo.f_speakerName, currentDialogueInfo.f_dialgueText);
                dialogueOver = true;
                OnSingleDialogueEnd?.Invoke();
                ShowBranchOpt();
            }
            // �ƽ��Ի�
            else
            {
                if (!currentDialogueInfo.f_hasBranch)
                {
                    // ��ʾ��һID�ĶԻ�
                    ShowCurrentDialogue(currentDialogueInfo.f_nextId);
                }
            }
        }

        /// <summary>
        /// ��ʾ�Ի���֧ѡ��
        /// </summary>
        private void ShowBranchOpt()
        {
            if (currentDialogueInfo.f_hasBranch)
            {
                var branchIds = TextUtility.SplitToIntArr(currentDialogueInfo.f_branchIds, 2);
                var branchInfos = new BranchInfo[branchIds.Length];

                for (var i = 0; i < branchIds.Length; i++)
                {
                    branchInfos[i] = ServiceLocator.Get<IBinaryDataManager>().GetConfig<BranchInfoContainer>(EConfigLoadType.Excel).dataDic[branchIds[i]];
                }
                dialogueController.SetBranchOpt(branchInfos);
            }
        }

        public void OnSelectOpt(int dialogueId)
        {
            ShowCurrentDialogue(dialogueId);
            OnBranchSelected?.Invoke();
        }

        public void EndDialogue()
        {
            // ���ñ�־
            IsDialogueActive = false;
            // ���ضԻ�UI
            ServiceLocator.Get<IUIManager>().DestroyView(AbKeyCollection.Ui, dialogueController);
            // �ַ��Ի������¼�
            ServiceLocator.Get<IEventCenter>().TriggerEvent(new DialogueEvent() { NpcId = npcInfo.f_id });
            // �������Ի��������¼�
            OnDialogueEnd?.Invoke();
            // �����Ի�ѡ��UI����
            ServiceLocator.Get<IPoolManager>().ClearTypes(typeof(DialogueOptUI));
            // �����Ի��ع�UI����
            ServiceLocator.Get<IPoolManager>().ClearTypes(typeof(DialogueReviewUI));
        }
    }
}
