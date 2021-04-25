using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomImage : MonoBehaviour
{
    public Sprite[] choices;

    void Awake()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sprite = Utils.Choose(choices);
        else if (TryGetComponent(out SpriteMask spriteMask))
            spriteMask.sprite = Utils.Choose(choices);
    }

    void Update()
    {
        
    }
}
