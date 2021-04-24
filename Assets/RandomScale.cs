using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RandomScale : MonoBehaviour
{
    public Vector2 range = new Vector2(.75f, 1.25f);
    void Awake()
    {
        transform.localScale = transform.localScale * Random.Range(range.x, range.y);
    }
}
