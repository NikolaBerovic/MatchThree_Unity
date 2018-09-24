using UnityEngine;
using System.Collections;

// this manager class handles particle effects
public class ParticleManager : Singleton<ParticleManager>
{
    [SerializeField] private int _poolSize = 20;

    [SerializeField] private GameObject _clearParticle;
	[SerializeField] private GameObject _breakParticle;
	[SerializeField] private GameObject _breakableParticle;
    [SerializeField] private GameObject _bombParticle;

    private Transform _clearParticlePool;
    private Transform _breakParticlePool;
    private Transform _breakableParticlePool;
    private Transform _bombParticlePool;

    void Start()
    {
        _clearParticlePool = ObjectPool.Instance.CreatePool(_clearParticle, _poolSize);
        _breakParticlePool = ObjectPool.Instance.CreatePool(_breakParticle, _poolSize);
        _breakableParticlePool = ObjectPool.Instance.CreatePool(_breakableParticle, _poolSize);
        _bombParticlePool = ObjectPool.Instance.CreatePool(_bombParticle, _poolSize);
    }

    ///<summary> Plays Clear particle effect at location</summary>
    public void PlayClearFXAt(int x, int y, int z = 0)
	{
		if (_clearParticle == null)
		{
            Debug.LogError("Clear particle prefab not set!");
            return;
		}

        GameObject particle = ObjectPool.Instance.GetFromPool(_clearParticlePool);
        particle.transform.position = new Vector3(x, y, z);
        particle.SetActive(true);
	}

    ///<summary> Plays Break particle effect at location</summary>
    public void PlayBreakFXAt(int breakableValue, int x, int y, int z = 0)
	{
        if (_breakParticle == null || _breakableParticle == null)
        {
            Debug.LogError("Break or breakable prefab not set!");
            return;
        }

        Transform pool = (breakableValue > 0) 
            ? _breakableParticlePool : _breakParticlePool;

        GameObject particle = ObjectPool.Instance.GetFromPool(pool);
        particle.transform.position = new Vector3(x, y, z);
        particle.SetActive(true);
    }

    ///<summary> Plays Bomb particle effect at location</summary>
    public void PlayBombFXAt(int x, int y, int z = 0)
	{
        if (_bombParticle == null)
        {
            Debug.LogError("Clear particle prefab not set!");
            return;
        }

        GameObject particle = ObjectPool.Instance.GetFromPool(_bombParticlePool);
        particle.transform.position = new Vector3(x, y, z);
        particle.SetActive(true);
    }

}
