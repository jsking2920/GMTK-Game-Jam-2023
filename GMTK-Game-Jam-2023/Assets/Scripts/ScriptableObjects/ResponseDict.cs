using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> { }

[System.Serializable]
public class IntResponseDictionary : SerializableDictionary<int, Response> { }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResponseDict", order = 1)]
public class ResponseDict : ScriptableObject
{
    [SerializeField] public StringIntDictionary sentenceToIntDict;
    [SerializeField] public IntResponseDictionary intToResponseDict;

    public Response StringToResponse(string input)
    {
        int index = sentenceToIntDict[input];
        return intToResponseDict[index];
    }
}
