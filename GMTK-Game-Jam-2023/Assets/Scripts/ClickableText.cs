using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

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


    [Header("Simple or complex word delineation")]
    public bool simpleDelineation = false;
    public char seperator = '^'; // '^' for complex; ' ' for simple
    public string joiner = ""; // "" for complex; " " for simple
    private int[] wordLengths;


    private void Start()
    {
        mainCam = Camera.main;

        text = GetComponent<TextMeshProUGUI>();

        allWordsUntagged = sentence.text.Split(seperator);
        allWordsTagged = new string[allWordsUntagged.Length];
        allWordsUntagged.CopyTo(allWordsTagged, 0);

        text.text = string.Join(joiner, allWordsTagged);

        blackoutRichTextTagOpener = "<mark=#" + blackoutColor.ToHexString() + ">";
        hoverRichTextTagOpener = "<mark=#" + hoverOnBlackColor.ToHexString() + ">";
        hoverOnNormalRichTextTagOpener = "<mark=#" + hoverOnNormalColor.ToHexString() + ">";

        if (!simpleDelineation)
        {
            wordLengths = new int[allWordsTagged.Length];
            for (int i = 0; i < allWordsUntagged.Length; i++)
            {
                wordLengths[i] = allWordsUntagged[i].Length;
            }
        }
    }

    private void Update()
    {
        if (hovering)
        {
            highlightedWordIndex = GetWordIndex(Input.mousePosition, mainCam);

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
                    text.text = string.Join(joiner, allWordsTagged);
                }
                prevHighlightedWordIndex = highlightedWordIndex;
            }
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        int index = GetWordIndex(pointerEventData.position, mainCam);

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
            text.text = string.Join(joiner, allWordsTagged);
        }
        else
        {
            allWordsTagged[index] = allWordsUntagged[index];
            text.text = string.Join(joiner, allWordsTagged);
        }
    }

    private void ToggleWordState(int index)
    {
        if (!blackedOutWordIndices.Contains(index))
        {
            allWordsTagged[index] = blackoutRichTextTagOpener + allWordsUntagged[index] + richTextTagCloser;
            text.text = string.Join(joiner, allWordsTagged);

            blackedOutWordIndices.Add(index);
        }
        else
        {
            allWordsTagged[index] = allWordsUntagged[index];
            text.text = string.Join(joiner, allWordsTagged);

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
                agg += allWordsUntagged[i] + joiner;
            }
        }

        if (joiner.Length > 0)
        {
            return agg.Substring(0, agg.Length - 1);
        }
        else
        {
            return agg;
        }
    }

    private int GetWordIndex(Vector3 pos, Camera cam)
    {
        if (simpleDelineation)
        {
            return TMP_TextUtilities.FindIntersectingWord(text, pos, mainCam);
        }
        else
        {
            int charIndex = TMP_TextUtilities.FindIntersectingCharacter(text, pos, cam, false);

            if (charIndex == -1) return -1;

            int accumulator = 0;

            for (int i = 0; i < wordLengths.Length; i++)
            {
                accumulator += wordLengths[i];
                if (charIndex < accumulator) return i;
            }
            return -1;
        }
    }
}
