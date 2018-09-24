using UnityEngine;
using System.Collections;

///<summary>Allows a UI component to move from a start position to onscreen position to an end position</summary>
[RequireComponent(typeof(RectTransform))]
public class SliderUI : MonoBehaviour 
{

	[SerializeField] private Vector3 _startPosition;
	[SerializeField] private Vector3 _onScreenPosition;
	[SerializeField] private Vector3 _endPosition;
	[SerializeField] private float _timeToMove = 1f;
    [SerializeField] private InterpType _interpolation = InterpType.SmootherStep;

    private RectTransform _rectTransform;

	private bool _isMoving = false;

	void Awake() 
	{
		_rectTransform = GetComponent<RectTransform>();
	}

    ///<summary>Moves component from start to end position</summary>
    void Slide(Vector3 startPos, Vector3 endPos, float timeToMove)
	{
		if (!_isMoving) 
        { StartCoroutine (SlideRoutine (startPos, endPos, timeToMove)); }
	}

	IEnumerator SlideRoutine(Vector3 startPos, Vector3 endPos, float timeToMove)
	{

		if (_rectTransform != null) 
		{ _rectTransform.anchoredPosition = startPos; }

		bool reachedDestination = false;
		float elapsedTime = 0f;

		_isMoving = true;

		while (!reachedDestination) 
		{
			if (Vector3.Distance (_rectTransform.anchoredPosition, endPos) < 0.01f)
			{
				reachedDestination = true;
				break;
			}

			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp (elapsedTime / timeToMove, 0f, 1f);

            t = Mathematics.Interpolate(t, _interpolation);

			if (_rectTransform != null)
			{ _rectTransform.anchoredPosition = Vector3.Lerp (startPos, endPos, t); }

			yield return null;

		}

		_isMoving = false;
	}

    ///<summary>UI slides in from start position to on screen position</summary>
    public void SlideIn()
	{
		Slide (_startPosition, _onScreenPosition, _timeToMove);
	}

    ///<summary>UI slides in from on screen position to end position</summary>
    public void SlideOut()
	{
		Slide (_onScreenPosition, _endPosition, _timeToMove);
	}


}
