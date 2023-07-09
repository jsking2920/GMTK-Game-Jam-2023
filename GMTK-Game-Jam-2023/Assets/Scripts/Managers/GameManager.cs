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

    public int CurrentSentence => currentSentence;
    private int currentSentence = 0;

    public static event Action<int> StartNextSentence;
    public static event Action FinishSentence;
    public static event Action ResetGame;

    public Cinemachine.CinemachineVirtualCamera titleScreenVCam;
    public Cinemachine.CinemachineVirtualCamera tutorialVCam;

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        gameState = GameState.MainMenu;
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
            }
            else if (gameState == GameState.Tutorial)
            {
                gameState = GameState.Playing;
                tutorialVCam.enabled = false;
                StartNextSentence?.Invoke(currentSentence);
            }
            else if (gameState == GameState.BetweenSentences)
            {
                if (currentSentence >= sentenceDict.sentenceDict.Count)
                {
                    gameState = GameState.MainMenu;
                    currentSentence = 0;
                    titleScreenVCam.enabled = true;
                    ResetGame?.Invoke();
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
    }
}
