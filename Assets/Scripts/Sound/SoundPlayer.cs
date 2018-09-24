using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour {

    public AudioSource Source { get; private set; }

    void Awake()
    {
        Source = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        Source.Stop();
        Source.Play();

        if (Source.clip != null)
        {
            float lifetime = Source.clip.length;
            Invoke("Deactivate", lifetime);
        }
    }

    ///<summary>Deactivates GameObject</summary>
    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
