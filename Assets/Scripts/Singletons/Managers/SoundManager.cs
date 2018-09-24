using UnityEngine;

// Singletone manager class to handle sounds
public class SoundManager : Singleton<SoundManager>
{

    public float MusicVolume { get { return _musicVolume; } }
    public float FxVolume { get { return _fxVolume; } }

    [SerializeField, Range(0, 1)] float _musicVolume = 0.5f;
    [SerializeField, Range(0, 1)] float _fxVolume = 1f;

    [SerializeField] private AudioClip[] _musicClips;
    [SerializeField] private AudioClip[] _winClips;
    [SerializeField] private AudioClip[] _loseClips;
    [SerializeField] private AudioClip[] _bonusClips;

    [SerializeField] private int _poolSize = 10;
    [SerializeField] private float _lowPitch = 0.95f;
    [SerializeField] private float _highPitch = 1.05f;

    private SoundPlayer _currentMusic;
    private Transform _pool;

    private void Start()
    {
        _pool = ObjectPool.Instance.CreatePool<SoundPlayer>("Sound", _poolSize);
    }

    private void Update()
    {
        PlayRandomMusic();
    }

    ///<summary> Plays clip at position in world space</summary>
    public SoundPlayer PlayClipAt(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogError("No audio clip set! Can't execute PlayClipAtPoint()");
            return null;
        }

        SoundPlayer player = ObjectPool.Instance.
            GetFromPool(_pool).GetComponent<SoundPlayer>();

        if (player != null)
        {
            player.Source.clip = clip;

            // change the pitch of the sound within some variation
            float randomPitch = Random.Range(_lowPitch, _highPitch);
            player.Source.pitch = randomPitch;

            player.Source.volume = volume;
            player.gameObject.SetActive(true);
        }

        return player;
    }

    ///<summary> Plays random clip from clip array at position in world space</summary>
    public SoundPlayer PlayRandom(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogError("No audio clip set! Can't execute PlayRandom()");
            return null;
        }

        SoundPlayer player = null;
        int randomIndex = Random.Range(0, clips.Length);

        if (clips[randomIndex] != null)
        { player = PlayClipAt(clips[randomIndex], position, volume); }

        return player;
    }

    ///<summary> Plays random music from clip array</summary>
    public void PlayRandomMusic()
    {
        if (_currentMusic != null && _currentMusic.Source.isPlaying)
        {
            return;
        }

        _currentMusic = PlayRandom
            (_musicClips, Vector3.zero, _musicVolume);
    }

    ///<summary> Plays win clip</summary>
    public void PlayWinSound()
    {
        PlayRandom(_winClips, Vector3.zero, _fxVolume);
    }

    ///<summary> Plays lose clip</summary>
    public void PlayLoseSound()
    {
        PlayRandom(_loseClips, Vector3.zero, _fxVolume * 0.5f);
    }

    ///<summary> Plays bonus clip</summary>
    public void PlayBonusSound()
    {
        PlayRandom(_bonusClips, Vector3.zero, _fxVolume);
    }

}
