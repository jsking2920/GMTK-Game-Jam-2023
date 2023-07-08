using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class IntResponseDictDictionary : SerializableDictionary<int, ResponseDict> { }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SentenceDict", order = 1)]
public class SentenceDict : ScriptableObject
{
    public IntResponseDictDictionary sentenceDict = new IntResponseDictDictionary();
    public Dictionary<int, string> idToSentenceDict = new Dictionary<int, string>();

    public Response defaultResponse;

    public Response GetResponse(int id, string finalText)
    {
        Response ret = sentenceDict[id].StringToResponse(finalText.ToLower());
        if (ret == null)
        {
            return defaultResponse;
        }
        else
        {
            return ret;
        }
    }

    public string GetStringFromID(int id)
    {
        return idToSentenceDict[id];
    }
}