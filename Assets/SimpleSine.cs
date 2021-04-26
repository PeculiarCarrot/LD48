using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSine : MonoBehaviour
{
    public float amplitude = .5f;
    public float frequency = 1f;
    Vector3 startPos;

    void Awake()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position = startPos + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
    }
}
