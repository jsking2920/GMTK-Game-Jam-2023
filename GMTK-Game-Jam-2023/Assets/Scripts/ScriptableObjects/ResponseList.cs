using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StringResponseDictionary : SerializableDictionary<string, Response> { }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResponseList", order = 1)]
public class ResponseList : ScriptableObject
{
    [SerializeField] public StringResponseDictionary responseList;
}

[System.Serializable]
public class Response
{
    // public string sentence; //final sentence made by player

    public Sprite image;
    
    //add stats etc in here
}
