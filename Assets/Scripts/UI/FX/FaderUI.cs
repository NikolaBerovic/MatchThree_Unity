using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// this is a graphic that can be used to fade on or off to help the screen transition
[RequireComponent(typeof(MaskableGraphic))]
public class FaderUI : MonoBehaviour 
{

	[SerializeField, Range(0,1)] private float _fadeInToAlpha = 1f;
	[SerializeField, Range(0,1)] private float _fadeOutToAlpha = 0f;

	[SerializeField] private float _delay = 0f;
	[SerializeField] private float _timeToFade = 1f;

	private MaskableGraphic _graphic;

	void Awake() 
	{
		_graphic = GetComponent<MaskableGraphic> ();
	}

    ///<summary>Fades MaskableFraphic to alpha</summary>
    IEnumerator FadeRoutine(float alpha)
    {
        yield return new WaitForSeconds(_delay);
        _graphic.CrossFadeAlpha(alpha, _timeToFade, true);
    }

    ///<summary>Fades in MaskableFraphic</summary>
    public void FadeIn()
	{
		StartCoroutine (FadeRoutine (_fadeInToAlpha));
	}

    ///<summary>Fades out MaskableFraphic</summary>
    public void FadeOut()
	{
		StartCoroutine (FadeRoutine (_fadeOutToAlpha));
	}
}
