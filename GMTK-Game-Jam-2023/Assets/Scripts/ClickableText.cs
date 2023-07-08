using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

public class ClickableText : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Camera mainCam;
    private TextMeshProUGUI text;

    private bool hovering = false;
    private int highlightedWordIndex = -1;
    private int prevHighlightedWordIndex = -1;

    private string[] allWordsUntagged; // Array of the original words in the text, no rich text tags included
    private string[] allWordsTagged; // Actual words being displayed, including rich text modifiers

    private HashSet<int> blackedOutWordIndices = new HashSet<int>();

    private string blackoutRichTextTagOpener;
    private string hoverRichTextTagOpener;
    private string hoverOnNormalRichTextTagOpener;
    private string richTextTagCloser = "</mark>";


    public Color blackoutColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
    public Color hoverOnBlackColor = new Color(0.3f, 0.3f, 0.3f, 0.35f);
    public Color hoverOnNormalColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public Sentence sentence;


    private void Awake()
    {
        mainCam = Camera.main;

        text = GetComponent<TextMeshProUGUI>();
        text.text = sentence.text;

        allWordsUntagged = sentence.text.Split(' '); // will consider punctuation to be part of the words
        allWordsTagged = new string[allWordsUntagged.Length];
        allWordsUntagged.CopyTo(allWordsTagged, 0);

        blackoutRichTextTagOpener = "<mark=#" + blackoutColor.ToHexString() + ">";
        hoverRichTextTagOpener = "<mark=#" + hoverOnBlackColor.ToHexString() + ">";
        hoverOnNormalRichTextTagOpener = "<mark=#" + hoverOnNormalColor.ToHexString() + ">";
    }

    private void Update()
    {
        if (hovering)
        {
            highlightedWordIndex = TMP_TextUtilities.FindIntersectingWord(text, Input.mousePosition, mainCam);

            if (highlightedWordIndex != prevHighlightedWordIndex)
            {
                if (prevHighlightedWordIndex != -1)
                {
                    ResetWordState(prevHighlightedWordIndex);
                }
                if (highlightedWordIndex != -1)
                {
                    if (blackedOutWordIndices.Contains(highlightedWordIndex))
                    {
                        allWordsTagged[highlightedWordIndex] = hoverRichTextTagOpener + allWordsUntagged[highlightedWordIndex] + richTextTagCloser;
                    }
                    else
                    {
                        allWordsTagged[highlightedWordIndex] = hoverOnNormalRichTextTagOpener + allWordsUntagged[highlightedWordIndex] + richTextTagCloser;
                    }
                    text.text = string.Join(" ", allWordsTagged);
                }
                prevHighlightedWordIndex = highlightedWordIndex;
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // See: https://docs.unity3d.com/Packages/com.unity.textmeshpro@1.3/api/TMPro.TMP_TextUtilities.html
        int index = TMP_TextUtilities.FindIntersectingWord(text, pointerEventData.position, mainCam);

        if (index != -1)
        {
            ToggleWordState(index);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hovering = false;
    }

    // Used to un-highlight a word after hovering
    private void ResetWordState(int index)
    {
        if (blackedOutWordIndices.Contains(index))
        {
            allWordsTagged[index] = blackoutRichTextTagOpener + allWordsUntagged[index] + richTextTagCloser;
            text.text = string.Join(" ", allWordsTagged);
        }
        else
        {
            allWordsTagged[index] = allWordsUntagged[index];
            text.text = string.Join(" ", allWordsTagged);
        }
    }

    private void ToggleWordState(int index)
    {
        if (!blackedOutWordIndices.Contains(index))
        {
            allWordsTagged[index] = blackoutRichTextTagOpener + allWordsUntagged[index] + richTextTagCloser;
            text.text = string.Join(" ", allWordsTagged);

            blackedOutWordIndices.Add(index);
        }
        else
        {
            allWordsTagged[index] = allWordsUntagged[index];
            text.text = string.Join(" ", allWordsTagged);

            blackedOutWordIndices.Remove(index);
        }
    }

    public string GetCurrentString()
    {
        if (blackedOutWordIndices.Count == allWordsUntagged.Length)
        {
            return "";
        }
        
        string agg = "";
        for (int i = 0; i < allWordsUntagged.Length; i++)
        {
            if (!blackedOutWordIndices.Contains(i))
            {
                agg += allWordsUntagged[i] + " ";
            }
        }
        
        return agg.Substring(0, agg.Length - 1);;
    }
}
