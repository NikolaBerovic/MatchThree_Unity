using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    public bool IsGameOver
    { get { return _isGameOver; } set { _isGameOver = value; } }

    [SerializeField] private int _movesLeft = 30;
    [SerializeField] private int _scoreGoal = 10000;
    [SerializeField] private FaderUI _screenFader;
    [SerializeField] private Text _movesLeftText;
    [SerializeField] private Sprite _loseIcon;
    [SerializeField] private Sprite _winIcon;
    [SerializeField] private Sprite _goalIcon;
    [SerializeField] private MessageWindowUI _messageWindow;

    private Board _board;

    private bool _isReadyToBegin = false;
    private bool _isGameOver = false;
    private bool _isWinner = false;
    private bool _isReadyToReload = false;

    void Start()
    {
        _board = FindObjectOfType<Board>().GetComponent<Board>();

        UpdateMoves();
        StartCoroutine("ExecuteGameLoop");
    }

    /// <summary> Decrements left moves and updates text </summary>
    public void UpdateMoves()
    {
        _movesLeft--;

        if (_movesLeftText != null)
        { _movesLeftText.text = _movesLeft.ToString(); }
    }

    ///<summary> Executes the main coroutine for the Game (Start, Play, Wait, End) </summary>
    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");

        // wait for board to refill
        yield return StartCoroutine("WaitForBoardRoutine", 0.5f);
        yield return StartCoroutine("EndGameRoutine");
    }

    ///<summary> Begins the game by switching bool for coroutine. Button usable</summary>
    public void BeginGame()
    {
        _isReadyToBegin = true;
    }

    ///<summary> Coroutine responsible for executing all actions for start game (fade, sound, board init)</summary>
    IEnumerator StartGameRoutine()
    {
        if (_messageWindow != null)
        {
            _messageWindow.GetComponent<SliderUI>().SlideIn();
            _messageWindow.ShowMessage(_goalIcon, "SCORE GOAL\n" + _scoreGoal.ToString(), "START");
        }

        //Waits until player presses start button
        while (!_isReadyToBegin)
        { yield return null; }

        if (_screenFader != null)
        { _screenFader.FadeOut(); }

        yield return new WaitForSeconds(0.5f);

        if (_board != null)
        { _board.InitBoard(); }
    }

    ///<summary> Coroutine responsible for ingame condition checks</summary>
    IEnumerator PlayGameRoutine()
    {
        while (!_isGameOver)
        {
            if (ScoreManager.Instance != null)
            {
                if (ScoreManager.Instance.CurrentScore >= _scoreGoal)
                {
                    _isGameOver = true;
                    _isWinner = true;
                }
            }

            if (_movesLeft == 0)
            {
                _isGameOver = true;
                _isWinner = false;
            }

            yield return null;
        }
    }

    ///<summary> Coroutine responsible for pausing player action while refilling</summary>
    IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
        if (_board != null)
        {
            yield return new WaitForSeconds(_board.SwapTime);

            while (_board.IsRefilling)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(delay);
    }

    ///<summary> Coroutine responsible for executing all actions for end game (fade, sound, reload)</summary>
    IEnumerator EndGameRoutine()
    {
        _isReadyToReload = false;

        if (_isWinner)
        {
            if (_messageWindow != null)
            {
                _messageWindow.GetComponent<SliderUI>().SlideIn();
                _messageWindow.ShowMessage(_winIcon, "YOU WIN!", "OK");
            }

            if (SoundManager.Instance != null)
            { SoundManager.Instance.PlayWinSound(); }
        } 
		else
        {
            if (_messageWindow != null)
            {
                _messageWindow.GetComponent<SliderUI>().SlideIn();
                _messageWindow.ShowMessage(_loseIcon, "YOU LOSE!", "OK");
            }

            if (SoundManager.Instance != null)
            { SoundManager.Instance.PlayLoseSound(); }
        }

        yield return new WaitForSeconds(1f);

        if (_screenFader != null)
        { _screenFader.FadeIn(); }

        while (!_isReadyToReload)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);		
    }

    ///<summary> Reloads the scene by switching bool for coroutine. Button usable</summary>
    public void ReloadScene()
    {
        _isReadyToReload = true;
    }
}
