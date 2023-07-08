using System.Collections;
using UnityEngine;
using Cinemachine;

public class Sentence : MonoBehaviour
{
    public int id = 0;
    [HideInInspector] public string text; //set from csv file

    private HighlightableText censorableText;

    public CinemachineVirtualCamera sentenceCam;
    public CinemachineVirtualCamera imageCam;

    private void Awake()
    {
        text = GameManager.Instance.sentenceDict.GetStringFromID(id);
        censorableText = GetComponentInChildren<HighlightableText>();

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
        string result = censorableText.GetCurrentString();
        Response response = GameManager.Instance.sentenceDict.GetResponse(id, result);
        
        
        GameManager.Instance.SetGameState(GameState.BetweenSentences);
        sentenceCam.enabled = false;
        imageCam.enabled = true;
        // StartCoroutine(SubmitAnim());
    }

    private void OnStartNextSentence(int nextId)
    {
        if (nextId == id)
        {
            sentenceCam.enabled = true;
        }
        else
        {
            imageCam.enabled = false;
        }
    }

    // private IEnumerator SubmitAnim()
    // {
    // }
}
