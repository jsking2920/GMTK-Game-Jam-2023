using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSentence : Sentence
{
    protected override void OnStartNextSentence(int nextId)
    {
        if (nextId == 4 || nextId == 5 || nextId == 6)
        {
            id = nextId;
            text = GameManager.Instance.sentenceDict.GetStringFromID(id);
        }
        base.OnStartNextSentence(nextId);
    }
}
