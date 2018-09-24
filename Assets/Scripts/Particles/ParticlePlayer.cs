using UnityEngine;

public class ParticlePlayer : MonoBehaviour 
{

    [SerializeField] private float _lifetime = 1f;

    private ParticleSystem[] _allParticles;

	void Awake () 
	{
		_allParticles = GetComponentsInChildren<ParticleSystem>();
	}

    void OnEnable()
	{
		foreach (ParticleSystem ps in _allParticles)
		{
			ps.Stop();
			ps.Play();
		}

        Invoke("Deactivate", _lifetime);
	}

    ///<summary>Deactivates GameObject</summary>
    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
