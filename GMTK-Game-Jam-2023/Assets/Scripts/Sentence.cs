using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Sentence : MonoBehaviour
{
    public int id = 0;
    [HideInInspector] public string text; //set from csv file

    private HighlightableText censorableText;

    public CinemachineVirtualCamera sentenceCam;
    public CinemachineVirtualCamera imageCam;

    public Image responseImage;
    public BarsRemainingUI barsRemainingUI;
    public Button submitButton;

    private void Awake()
    {
        text = GameManager.Instance.sentenceDict.GetStringFromID(id);
        censorableText = GetComponentInChildren<HighlightableText>();
        censorableText.editable = false;

        sentenceCam.enabled = false;
        imageCam.enabled = false;

        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(Submit);
        submitButton.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameManager.StartNextSentence += OnStartNextSentence;
        GameManager.ResetGame += ResetSentence;
    }

    private void OnDisable()
    {
        GameManager.StartNextSentence -= OnStartNextSentence;
        GameManager.ResetGame -= ResetSentence;
    }

    void Update()
    {
        if (censorableText.editable)
        {
            if (!submitButton.interactable && censorableText.curBars == censorableText.maxBars)
            {
                submitButton.interactable = true;
            }
            else if (submitButton.interactable && censorableText.curBars != censorableText.maxBars)
            {
                submitButton.interactable = false;
            }
        }
    }

    public void Submit()
    {
        if (!censorableText.editable) return;

        string result = censorableText.GetCurrentString();
        Response response = GameManager.Instance.sentenceDict.GetResponse(id, result);
  
        sentenceCam.enabled = false;
        imageCam.enabled = true;
        censorableText.editable = false;
        responseImage.sprite = response.image;
        barsRemainingUI.gameObject.SetActive(false);
        GameManager.Instance.score += response.fulfillsPrompt;

        submitButton.gameObject.SetActive(false);

        GameManager.Instance.SentenceSubmitted();
    }

    private void OnStartNextSentence(int nextId)
    {
        if (nextId == id)
        {
            sentenceCam.enabled = true;
            censorableText.editable = true;
            StartCoroutine(DelayActivateButton());
        }
        else
        {
            censorableText.editable = false;
            imageCam.enabled = false;
            submitButton.gameObject.SetActive(false);
        }
    }

    private IEnumerator DelayActivateButton()
    {
        yield return new WaitForSeconds(2.0f);

        submitButton.gameObject.SetActive(true);
    }

    private void ResetSentence()
    {
        censorableText.editable = false;

        sentenceCam.enabled = false;
        imageCam.enabled = false;

        submitButton.gameObject.SetActive(false);

        //responseImage.sprite = null;
        //censorableText.ResetText();
    }
}
