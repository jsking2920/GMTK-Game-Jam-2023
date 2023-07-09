using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class IntResponseDictionary : SerializableDictionary<int, Response> { }

[System.Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> { }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResponseDict", order = 1)]
public class ResponseDict : ScriptableObject
{
    [SerializeField] public StringIntDictionary sentenceToIntDict = new StringIntDictionary();
    [SerializeField] public IntResponseDictionary intToResponseDict = new IntResponseDictionary();

    public Response defaultResponse = new Response();
    
    public Response StringToResponse(string input)
    {
        int index;
        if (!sentenceToIntDict.TryGetValue(input, out index))
        {
            return defaultResponse;
        }
        return intToResponseDict[index];
    }
}
