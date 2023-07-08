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
    [Header("Data")] 
    public SentenceDict sentenceDict;
    private GameState _gamestate;
    public GameState GameState //GameState cannot be set without calling SetGameState
    {
        set { SetGameState(value); }
        get { return _gamestate; }
    }
    
    //Set in Awake() functions of player & enemyManager
    public int CurrentSentence => currentSentence;
    private int currentSentence = 0;

    //events - these can be recieved and trigger things all throughout the game
    public static event Action GameStart; //
    public static event Action GameOver;
    public static event Action<int> StartNextSentence;
    public static event Action FinishSentence;
    public static event Action ResetGame; //reset all relevant variables

    public Cinemachine.CinemachineVirtualCamera titleScreenVCam;
    public Cinemachine.CinemachineVirtualCamera tutorialVCam;

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetGameState(GameState.MainMenu);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButtonDown(0) && _gamestate == GameState.MainMenu)
            {
                SetGameState(GameState.Tutorial);
                titleScreenVCam.enabled = false;
                tutorialVCam.enabled = true;
            }
            else if (_gamestate == GameState.Tutorial)
            {
                tutorialVCam.enabled = false;
                SetGameState(GameState.Playing);
            }
            else if (Input.GetMouseButtonDown(0) && _gamestate == GameState.BetweenSentences)
            {
                tutorialVCam.enabled = false;
                SetGameState(GameState.Playing);
            }
        }
    }

    public void SetGameState(GameState newGameState)
    {
        switch (newGameState)
        {
            case (GameState.MainMenu):
            {
                currentSentence = 0;
                ResetGame?.Invoke();
                break;
            }
            case (GameState.Tutorial):
            {
                currentSentence = 0;
                GameStart?.Invoke();
                break;
            }
            case (GameState.BetweenSentences):
            {
                if (GameState == GameState.End)
                {
                    currentSentence = 0;
                    ResetGame?.Invoke();
                }
                else
                {
                    FinishSentence?.Invoke();
                    currentSentence++;
                }
                break;
            }
            case (GameState.Playing):
            {
                StartNextSentence?.Invoke(currentSentence);
                break;
            }
            case (GameState.End):
            {
                GameOver?.Invoke();
                break;
            }
        }

        _gamestate = newGameState;
    }
}
