// using System.Collections;
// using Core.Serialize.Binary;
// using HotUpdate.Base.Settings;
// using HotUpdate.Common.Events;
// using UnityEngine;
//
// namespace HotUpdate.UI.Dialogue
// {
//     /// <summary>
//     /// 对话服务类，处理对话逻辑
//     /// </summary>
//     public class DialogueService
//     {
//         private void ShowCurrentDialogue(DialogueContext context, DialogueInfo dialogueInfo, NpcInfo npcInfo, bool enableTypewriter)
//         {
//             // 记录当前对话信息
//             context.CurrentDialogueInfo = dialogueInfo;
//             // 从配置表中获取说话者（NPC）信息
//             context.NpcInfo = npcInfo;
//             if (enableTypewriter)
//             {
//                 // 启用打字机效果：初始化状态+启动协程
//                 context.DialogueOver = false;
//                 context.TypewriterCor = _monoAdapter.StartCoroutine(ApplyTypewriter());
//                 OnSingleDialogueStart?.Invoke(_dialogueContext.CurrentDialogueInfo);
//             }
//             else
//             {
//                 // 禁用打字机：直接显示完整文本+显示分支选项
//                 context.DialogueOver = true;
//                 context.DialogueController.ShowDialogueText(_dialogueContext.NpcInfo.f_speakerName, _dialogueContext.CurrentDialogueInfo.f_dialgueText);
//                 ShowBranchOpt();
//             }
//         }
//         
//         private IEnumerator ApplyTypewriter(DialogueContext context)
//         {
//             var text = context.CurrentDialogueInfo.f_dialgueText;
//             context.TypewriterBuilder.Clear();
//             foreach (var t in text)
//             {
//                 context.TypewriterBuilder.Append(t);
//                 // 逐帧更新对话文本显示
//                 context.DialogueController.ShowDialogueText(context.NpcInfo.f_speakerName, context.TypewriterBuilder.ToString());
//                 // 等待字符间隔时间
//                 yield return new WaitForSeconds(context.TypewriterInterval);
//             }
//             // 标记单条对话播放完成
//             context.DialogueOver = true;
//             OnSingleDialogueEnd?.Invoke();
//             // 显示分支选项（如果有）
//             ShowBranchOpt();
//         }
//         
//         public void EndDialogue()
//         {
//             // 标记对话为未进行状态
//             _dialogueContext.IsDialogueActive = false;
//             // 销毁对话UI
//             _uiService.CloseAsync(_dialogueContext.DialogueController.panelId, true);
//             // 触发全局对话事件
//             _eventCenter.TriggerEvent(new DialogueEvent(_dialogueContext.NpcInfo.f_id));
//             // 触发对话结束事件
//             OnDialogueEnd?.Invoke();
//         }
//     }
// }
