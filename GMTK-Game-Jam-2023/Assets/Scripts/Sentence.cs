using UnityEngine;
using Cinemachine;

public class Sentence : MonoBehaviour
{
    public string text; //doing public cuz game jam wooo

    private HighlightableText censorableText;

    public CinemachineVirtualCamera sentenceCam;
    public CinemachineVirtualCamera imageCam;

    private void Awake()
    {
        censorableText = GetComponentInChildren<HighlightableText>();

        sentenceCam.enabled = false;
        imageCam.enabled = false;
    }

    private void OnEnable()
    {
        sentenceCam.enabled = true;
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
        Response response = GameManager.Instance.sentenceDict.GetResponse(text, result);

        sentenceCam.enabled = false;
        imageCam.enabled = true;
    }
}
