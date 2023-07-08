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

    private void Start()
    {
        text = GameManager.Instance.sentenceDict.GetStringFromID(id);
        censorableText = GetComponentInChildren<HighlightableText>();
        censorableText.editable = false;

        sentenceCam.enabled = false;
        imageCam.enabled = false;
    }

    private void OnEnable()
    {
        GameManager.StartNextSentence += OnStartNextSentence;
    }

    private void OnDisable()
    {
        GameManager.StartNextSentence -= OnStartNextSentence;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && censorableText.editable)
        {
            Submit();
        }
    }

    public void Submit()
    {
        string result = censorableText.GetCurrentString();
        Response response = GameManager.Instance.sentenceDict.GetResponse(id, result);
        
        GameManager.Instance.SetGameState(GameState.BetweenSentences);
        sentenceCam.enabled = false;
        imageCam.enabled = true;
        censorableText.editable = false;
        responseImage.sprite = response.image;
        barsRemainingUI.gameObject.SetActive(false);
    }

    private void OnStartNextSentence(int nextId)
    {
        if (nextId == id)
        {
            sentenceCam.enabled = true;
            censorableText.editable = true;
        }
        else
        {
            censorableText.editable = false;
            imageCam.enabled = false;
        }
    }
}
