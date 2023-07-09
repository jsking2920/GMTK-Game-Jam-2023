using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class Subtitle : MonoBehaviour
{
    private TextMeshProUGUI tmpro;
    public float TimeBetweenLetters = 0.05f;

    private void Start()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
        Clear();
    }

    public void Clear()
    {
        StopAllCoroutines();
        if(tmpro) tmpro.text = "";
    }

    public void WriteSubtitle(string subtitle)
    {
        Clear();
        StartCoroutine(TypeOutText(subtitle));
    }

    private IEnumerator TypeOutText(string final)
    {
        StringBuilder currentString = new StringBuilder();
        for (int i = 0; i < final.Length; i++)
        {
            if (final[i] != ' ')
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Mouseover");
            }
            if (final[i] == '*')
            {
                tmpro.text += ',';
                continue;
            }
            tmpro.text += final[i];
            yield return new WaitForSeconds(TimeBetweenLetters);
        }
    }
}
