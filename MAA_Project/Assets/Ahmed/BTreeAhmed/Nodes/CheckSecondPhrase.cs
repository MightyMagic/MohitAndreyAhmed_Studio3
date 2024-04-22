using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckSecondPhrase : BTNode
{
    private NpcAI _npcAI;
    public CheckSecondPhrase(NpcAI npc)
    {
        this._npcAI = npc;
    }
    public override BTNodeState Evaluate()  // Need to update this later with the capture script in the tutturial level
    {
        if (_npcAI._puzzleCatcher.puzzleWords[1].captured)
        {
           
            return BTNodeState.SUCCESS;
        }
           
        return BTNodeState.FAILURE;
    }
}
