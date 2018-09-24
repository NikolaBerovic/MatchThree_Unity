using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : Singleton<ScoreManager> 
{
    public int CurrentScore { get; private set; }

    [SerializeField] private Text _scoreText;

    private int _increment = 1;

	void Start () 
	{
		UpdateScoreText (CurrentScore);
	}

    ///<summary> Updates score text</summary>
    void UpdateScoreText(int scoreValue)
	{
		if (_scoreText != null) 
		{
			_scoreText.text = scoreValue.ToString ();
		}
	}

    ///<summary> Returns score text as int</summary>
    int GetScoreText()
    {
        int i = 0;
        int.TryParse(_scoreText.text, out i);

        return i;
    }


    ///<summary> Adds value to score</summary>
    public void AddScore(int value)
	{
		CurrentScore += value;
		StartCoroutine (CountScoreRoutine ());
	}

    ///<summary> Increments score gradually</summary>
    IEnumerator CountScoreRoutine()
	{
		int iterations = 0;
        int counterValue = GetScoreText();

        while (counterValue < CurrentScore && iterations < 100000) 
		{
			counterValue += _increment;
			UpdateScoreText (counterValue);
			iterations++;
			yield return null;
		}

		counterValue = CurrentScore;
		UpdateScoreText (CurrentScore);
	}
}
