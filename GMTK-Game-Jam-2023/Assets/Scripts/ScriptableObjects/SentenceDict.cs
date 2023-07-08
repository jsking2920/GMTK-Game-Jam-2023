using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StringResponseDictDictionary : SerializableDictionary<string, ResponseDict> { }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SentenceDict", order = 1)]
public class SentenceDict : ScriptableObject
{
    public StringResponseDictDictionary sentenceDict;

    public Response GetResponse(string sentence, string finalText)
    {
        return sentenceDict[sentence].responseList[finalText];
    }
}