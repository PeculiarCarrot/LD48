using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomImage : MonoBehaviour
{
    public Sprite[] choices;

    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = Utils.Choose(choices);
    }

    void Update()
    {
        
    }
}
