using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentence : MonoBehaviour
{
    public string text; //doing public cuz game jam wooo
    private string currentText;
    private ClickableText clickableText;

    private void Awake()
    {
        clickableText = GetComponentInChildren<ClickableText>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentText = text;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Submit();
        }
    }

    public void Submit()
    {
        string result = clickableText.GetCurrentString();
        Response response = GameManager.Instance.sentenceDict.GetResponse(text, result);
        
    }
}
