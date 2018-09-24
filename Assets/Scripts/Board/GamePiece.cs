using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

    public int IndexX { get; private set; }
    public int IndexY { get; private set; }
    public MatchValue MatchValue
    { get { return _matchValue; } set { _matchValue = value; } }

    [SerializeField] MatchValue _matchValue;
    [SerializeField] private int _scoreValue = 20;
    [SerializeField] private InterpType _interpolation = InterpType.SmootherStep;
    [SerializeField] private AudioClip _clearSound;

    private Board _board;
    private bool _isMoving = false;

    ///<summary>Initializes GamePiece</summary>
    public void Init(Board board)
	{
		_board = board;
	}

    ///<summary>Sets coordinates of a GamePiece</summary>
    public void SetCoord(int x, int y)
	{
		IndexX = x;
		IndexY = y;
	}

    ///<summary>Movees GamePiece to destination over time</summary>
    public void Move (int destX, int destY, float timeToMove)
	{
		if (!_isMoving && _board != null)
		{
			StartCoroutine(MoveRoutine(new Vector3(destX, destY,0), timeToMove));	
		}
	}

	IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
	{
        _isMoving = true;

        Vector3 startPosition = transform.position;
		float elapsedTime = 0f;

        bool reachedDestination = false;

        while (!reachedDestination)
		{
			if (Vector3.Distance(transform.position, destination) < 0.01f)
			{
				reachedDestination = true;
                _board.PlaceGamePiece(this, (int) destination.x, (int) destination.y);
				break;
			}

			elapsedTime += Time.deltaTime;

			float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            t = Mathematics.Interpolate(t, _interpolation);
			transform.position = Vector3.Lerp(startPosition, destination, t);

			yield return null;
		}

		_isMoving = false;
	}

    ///<summary>Changes color of current GamePiece to matching GamePiece</summary>
    public void ChangeColor(GamePiece pieceToMatch)
	{
		SpriteRenderer rendererToChange = GetComponent<SpriteRenderer>();

		if (pieceToMatch !=null)
		{
			SpriteRenderer rendererToMatch = pieceToMatch.GetComponent<SpriteRenderer>();

			if (rendererToMatch !=null && rendererToChange !=null)
			{ rendererToChange.color = rendererToMatch.color; }

            _matchValue = pieceToMatch.MatchValue;
		}

	}

    ///<summary>Scores points, plays clip for scoring</summary>
    public void ScorePoints(int multiplier = 1, int bonus = 0)
	{
		if (ScoreManager.Instance != null)
		{
			ScoreManager.Instance.AddScore (_scoreValue * multiplier + bonus);
		}

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayClipAt(_clearSound, Vector3.zero, SoundManager.Instance.FxVolume);
        }
	}

}
