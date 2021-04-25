using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AutoVolume : MonoBehaviour
{
    public Vector2 distances = new Vector2(5, 8f);
    public Vector2 volumes = new Vector2(.5f, 0f);

    AudioSource audio;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        var d = Mathf.Abs(Camera.main.transform.position.y - transform.position.y);
        audio.volume = Utils.Remap(d, distances.x, volumes.x, distances.y, volumes.y, true, true);
    }
}
