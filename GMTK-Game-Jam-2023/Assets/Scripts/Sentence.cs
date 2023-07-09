using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Sentence : MonoBehaviour
{
    public int id = 0;
    [HideInInspector] public string text; //set from csv file
    public bool requiresMaxBarsToBeUsed = false;

    protected HighlightableText censorableText;

    public CinemachineVirtualCamera sentenceCam;
    public CinemachineVirtualCamera imageCam;

    public Image responseImage;
    public BarsRemainingUI barsRemainingUI;
    public Button submitButton;
    private Image submitImage;

    public Subtitle subtitle;

    private void Awake()
    {
        text = GameManager.Instance.sentenceDict.GetStringFromID(id);
        censorableText = GetComponentInChildren<HighlightableText>();
        censorableText.editable = false;

        sentenceCam.enabled = false;
        imageCam.enabled = false;

        submitImage = submitButton.GetComponent<Image>();
        submitButton.onClick.RemoveAllListeners();
        submitButton.onClick.AddListener(Submit);
        SetButtonInteractable(!requiresMaxBarsToBeUsed);
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
        if (censorableText.editable && requiresMaxBarsToBeUsed)
        {
            if (!submitButton.interactable && censorableText.curBars == censorableText.maxBars)
            {
                SetButtonInteractable(true);
            }
            else if (submitButton.interactable && censorableText.curBars != censorableText.maxBars)
            {
                SetButtonInteractable(false);
            }
        }
    }

    public virtual void Submit()
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
        if (subtitle && subtitle.enabled && subtitle.gameObject.activeInHierarchy) subtitle.WriteSubtitle(response.sideHeadline);
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Submit");
        
        submitButton.gameObject.SetActive(false);

        GameManager.Instance.SentenceSubmitted();
    }

    protected virtual void OnStartNextSentence(int nextId)
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
        //subtitle.Clear();
    }

    public virtual void UpdateSubmittable()
    {
        string result = censorableText.GetCurrentString();
        Response response = GameManager.Instance.sentenceDict.GetResponse(id, result);
        if (response.image == null) //jank check for default response
        {
            SetButtonInteractable(false);
        }
        else
        {
            SetButtonInteractable(true);
        }
    }

    private void SetButtonInteractable(bool interactable)
    {
        if (interactable)
        {
            submitImage.color = Color.white;
        }
        else
        {
            submitImage.color = new Color(0.5764706f, 0.5764706f, 0.5764706f);
        }

        submitButton.interactable = interactable;
    }
}
