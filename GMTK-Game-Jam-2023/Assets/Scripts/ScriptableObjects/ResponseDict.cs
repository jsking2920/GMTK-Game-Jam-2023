using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StringResponseDictionary : SerializableDictionary<string, Response> { }

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResponseDict", order = 1)]
public class ResponseDict : ScriptableObject
{
    [SerializeField] public StringResponseDictionary responseList;
}
