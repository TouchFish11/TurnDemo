using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEvent : IEvent
{
    public int NpcId { get; }

    public DialogueEvent(int npcId)
    {
        NpcId = npcId;
    }

    void IEvent.ResetEvent()
    {

    }
}
