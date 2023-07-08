using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CSVToSTIDict
{
    public static string[] paths =
    {
        "0",

    };

    [MenuItem("Utilities/Generate Dictionaries")]
    public static void GenerateDicts()
    {
        SentenceDict sentenceDict = 
            AssetDatabase.LoadAssetAtPath<SentenceDict>("Assets/ScriptableObjects/SentenceDict.asset");
        sentenceDict.sentenceDict.Clear();
        
        foreach (string name in paths)
        {
            //split data
            string[] allLines = File.ReadAllLines(Application.dataPath + "/CSVs/" + name + ".csv");
            List<string[]> splitData = new List<string[]>();

            foreach (var s in allLines)
            {
                string[] temp = s.Split(',');
                splitData.Add(temp);
            }

            //add sentence to overall dictionary
            string sentence = splitData[1][0];
            int sentenceID;
            if (!int.TryParse(splitData[3][0], out sentenceID))
            {
                Debug.LogWarning("Unparseable sentence ID: " + splitData[3][0]);
            }
            
            ResponseDict newDict = AssetDatabase.LoadAssetAtPath<ResponseDict>($"Assets/ScriptableObjects/ResponseDicts/{name}.asset");
            if (newDict == null)
            {
                Debug.Log("Making new responseDict");
                newDict = ScriptableObject.CreateInstance<ResponseDict>();
                AssetDatabase.CreateAsset(newDict, $"Assets/ScriptableObjects/ResponseDicts/{name}.asset");
            }
            else
            {
                newDict.sentenceToIntDict.Clear();
            }
            
            sentenceDict.sentenceDict.Add(sentenceID, newDict);
            sentenceDict.idToSentenceDict.Add(sentenceID, sentence);
            
            for (int i = 1; i < allLines.Length; i++)
            {
                //add sentence/response ID pair
                string[] splitLine = splitData[i];

                int responseID;
                if (!int.TryParse(splitLine[4], out responseID))
                {
                    Debug.LogWarning("Unparseable response ID: " + splitLine[4] + ". one line " + i);
                }
                if (!newDict.sentenceToIntDict.TryAdd(splitLine[1], responseID))
                {
                    Debug.LogWarning("Duplicate key detected in " + name + ".csv. Sentence" + splitLine[1] + ": " + splitData[1]);
                }
                
                
                //add new responses to already existing dictionary (we don't want to overwrite old references)
                if(int.TryParse(splitLine[6], out responseID))
                {
                    Response response;
                    if (!newDict.intToResponseDict.TryGetValue(responseID, out response))
                    {
                        response = new Response();
                        newDict.intToResponseDict.Add(responseID, response);
                    }

                    response.sideHeadline = splitLine[8];
                    Debug.Log(splitLine[8]);
                }
            }

        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("Dictionaries generated. ");
    }
}
