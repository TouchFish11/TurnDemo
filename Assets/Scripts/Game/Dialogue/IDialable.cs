using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialable
{
    /// <summary>
    /// 对话开始
    /// </summary>
    void OnDialogueStart();

    /// <summary>
    /// 对话结束
    /// </summary>
    void OnDialogueEnd();
}
