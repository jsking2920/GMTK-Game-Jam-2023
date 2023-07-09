using System;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Tutorial,
    Playing,
    BetweenSentences,
    End
}

public class GameManager : Singleton<GameManager>
{
    public SentenceDict sentenceDict;

    public GameState gameState;

    public float score;
    public int CurrentSentence => currentSentence;
    private int currentSentence = 0;

    public static event Action<int> StartNextSentence;
    public static event Action FinishSentence;
    public static event Action ResetGame;

    public Cinemachine.CinemachineVirtualCamera titleScreenVCam;
    public Cinemachine.CinemachineVirtualCamera tutorialVCam;

    public Texture2D penTex;
    public CursorMode cursorMode;

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        gameState = GameState.MainMenu;

#if UNITY_WEBGL
        cursorMode = CursorMode.ForceSoftware;
#else
        cursorMode = CursorMode.Auto;
#endif
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (gameState == GameState.MainMenu)
            {
                gameState = GameState.Tutorial;
                currentSentence = 0;
                titleScreenVCam.enabled = false;
                tutorialVCam.enabled = true;
                AudioManager.Instance.StartMusic();
                score = 0;
            }
            else if (gameState == GameState.Tutorial)
            {
                gameState = GameState.Playing;
                tutorialVCam.enabled = false;
                StartNextSentence?.Invoke(currentSentence);
            }
            else if (gameState == GameState.BetweenSentences)
            {
                if (currentSentence >= sentenceDict.sentenceDict.Count + 1)
                {
                    gameState = GameState.MainMenu;
                    currentSentence = 0;
                    titleScreenVCam.enabled = true;
                    ResetGame?.Invoke();
                }
                else if (currentSentence > 3)
                {
                    //Set up ending, this is a jank way yes but sue me
                    if (score < -0.4f)
                    {
                        currentSentence = 4;
                    } 
                    else if (score < 0.4f)
                    {
                        currentSentence = 5;
                    }
                    else
                    {
                        currentSentence = 6;
                    }
                    Debug.Log("Going to ending " + currentSentence);
                    StartNextSentence?.Invoke(currentSentence);
                    
                    //hopefully this does not break anything but likely culprit
                    currentSentence = 6;
                }
                else
                {
                    gameState = GameState.Playing;
                    StartNextSentence?.Invoke(currentSentence);
                }
            }
        }
    }

    public void SentenceSubmitted()
    {
        gameState = GameState.BetweenSentences;
        currentSentence++;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
