using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;

public class HighlightableText : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCam;
    private TextMeshProUGUI text;

    private string[] allWordsUntagged; // Array of the original words in the text, no rich text tags included
    private string[] allWordsTagged; // Actual words being displayed, rich text modifiers added to the strings
    
    private List<Vector2Int> highlightedRanges = new List<Vector2Int>(); // Ranges of words that are currently censored, vectors are endpoint inclusive

    private string blackoutRichTextTagOpener;
    private string richTextTagCloser = "</mark>";

    public Color highlightColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    public Sentence sentence;

    public int maxBars = 2;
    [HideInInspector] public int curBars = 0;
    public BarsRemainingUI barsRemaingUI;

    [Header("Simple or complex word delineation")]
    public bool simpleDelineation = false;
    public char seperator = '^'; // '^' for complex; ' ' for simple
    public string joiner = ""; // "" for complex; " " for simple
    private int[] wordLengths;

    private bool dragging = false;

    public bool editable = false;

    private bool penCursor = false;

    private void Start()
    {
        mainCam = Camera.main;

        text = GetComponent<TextMeshProUGUI>();

        // commas are swapped with asterisks in the csv files so replace them
        string sentenceText = sentence.text.Replace('*', ',');

        allWordsUntagged = sentenceText.Split(seperator);
        allWordsTagged = new string[allWordsUntagged.Length];
        allWordsUntagged.CopyTo(allWordsTagged, 0);

        text.text = string.Join(joiner, allWordsTagged);

        blackoutRichTextTagOpener = "<mark=#" + highlightColor.ToHexString() + ">";

        if (!simpleDelineation)
        {
            wordLengths = new int[allWordsTagged.Length];
            for (int i = 0; i < allWordsUntagged.Length; i++)
            {
                wordLengths[i] = allWordsUntagged[i].Length;
            }
        }

        SetCurBars(0);
    }

    private void Update()
    {
        if (!editable) return;

        int index = GetWordIndex(Input.mousePosition, mainCam);

        if (index == -1 && penCursor)
        {
            penCursor = false;
            Cursor.SetCursor(null, Vector2.zero, GameManager.Instance.cursorMode);
        }
        else if (index != -1 && !penCursor)
        {
            penCursor = true;
            Cursor.SetCursor(GameManager.Instance.penTex, Vector2.zero, GameManager.Instance.cursorMode);
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (dragging || !editable) return;
        
        int index = GetWordIndex(pointerEventData.position, mainCam);

        if (index != -1)
        {
            int highlightRange = GetHighlightRangeForWord(index);

            if (highlightRange != -1)
            {
                DeleteRange(highlightRange);
            }
            else
            {
                int addedToExistingRange = AddToExistingAdjacentRangeIfPossible(index);
                if (addedToExistingRange == -1 && curBars < maxBars)
                {
                    InsertNewRange(index);
                }
            }
        }
    }
    
    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if (!editable) return;

        dragging = true;

        int index = GetWordIndex(pointerEventData.position, mainCam);

        if (index != -1)
        {
            int highlightRange = GetHighlightRangeForWord(index);

            if (highlightRange == -1)
            {
                int addedToExistingRange = AddToExistingAdjacentRangeIfPossible(index);
                if (addedToExistingRange == -1 && curBars < maxBars)
                {
                    InsertNewRange(index);
                }
            }
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (!editable) return;

        int index = GetWordIndex(data.position, mainCam);

        if (index != -1)
        {
            int highlightRange = GetHighlightRangeForWord(index);

            if (highlightRange == -1)
            {
                AddToExistingAdjacentRangeIfPossible(index);
            }
        }
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        dragging = false;
    }

    public string GetCurrentString()
    {
        string agg = "";
        for (int i = 0; i < allWordsUntagged.Length; i++)
        {
            if (GetHighlightRangeForWord(i) == -1)
            {
                agg += allWordsUntagged[i] + joiner;
            }
        }

        if (joiner.Length > 0)
        {
            agg = agg.Substring(0, agg.Length - 1);
        }

        agg = agg.Replace(',', '*'); // commas replaces with asterisks in the csv files
        return agg;
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

    private int GetHighlightRangeForWord(int wordIndex)
    {
        for (int i = 0; i < highlightedRanges.Count; i++)
        {
            if (wordIndex >= highlightedRanges[i].x && wordIndex <= highlightedRanges[i].y)
            {
                return i;
            }
        }

        return -1;
    }

    // Should only be called on wordIndices that are within bounds and not already highlighted
    private int AddToExistingAdjacentRangeIfPossible(int wordIndex)
    {
        for (int i = 0; i < highlightedRanges.Count; i++)
        {
            if (wordIndex == highlightedRanges[i].x - 1)
            {
                highlightedRanges[i] = new Vector2Int(wordIndex, highlightedRanges[i].y);
                SetText();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Marker");
                return i;
            }
            else if (wordIndex == highlightedRanges[i].y + 1)
            {
                highlightedRanges[i] = new Vector2Int(highlightedRanges[i].x, wordIndex);
                SetText();
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Marker");
                return i;
            }
        }
        return -1;
    }

    private int InsertNewRange(int wordIndex)
    {
        Vector2Int r = new Vector2Int(wordIndex, wordIndex);
        highlightedRanges.Add(r);
        SortRanges();
        SetText();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Marker");

        SetCurBars(curBars + 1);

        return highlightedRanges.IndexOf(r);
    }

    private void DeleteRange(int index)
    {
        highlightedRanges.RemoveAt(index);
        SetText();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Erase");

        SetCurBars(curBars - 1);
    }

    private void SortRanges()
    {
        highlightedRanges.Sort((a, b) => {
            return a.x.CompareTo(b.x);
        });
    }

    private void SetText()
    {
        allWordsUntagged.CopyTo(allWordsTagged, 0);

        foreach (Vector2Int r in highlightedRanges)
        {
            if (r.x == r.y)
            {
                allWordsTagged[r.x] = blackoutRichTextTagOpener + allWordsUntagged[r.x] + richTextTagCloser;
            }
            else
            {
                allWordsTagged[r.x] = blackoutRichTextTagOpener + allWordsUntagged[r.x];
                allWordsTagged[r.y] = allWordsUntagged[r.y] + richTextTagCloser;
            }
        }
        text.text = string.Join(joiner, allWordsTagged);
    }

    private void SetCurBars(int bars)
    {
        curBars = bars;

        if (barsRemaingUI != null)
        {
            barsRemaingUI.UpdateUI(maxBars - bars);
        }
    }

    /*
    public void ResetText()
    {
        // commas are swapped with asterisks in the csv files so replace them
        string sentenceText = sentence.text.Replace('*', ',');

        allWordsUntagged = sentenceText.Split(seperator);
        allWordsTagged = new string[allWordsUntagged.Length];
        allWordsUntagged.CopyTo(allWordsTagged, 0);

        text.text = string.Join(joiner, allWordsTagged);

        highlightedRanges.Clear();
        dragging = false;

        if (!simpleDelineation)
        {
            wordLengths = new int[allWordsTagged.Length];
            for (int i = 0; i < allWordsUntagged.Length; i++)
            {
                wordLengths[i] = allWordsUntagged[i].Length;
            }
        }

        SetCurBars(0);
        SetText();
    }
    */
}
