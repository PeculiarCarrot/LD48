using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Worm : MonoBehaviour
{
    new SpriteRenderer renderer;
    [MinMaxSlider(0, 5f)]
    public Vector2 moveSpeed = new Vector2(.3f, 2);
    float chosenSpeed;
    bool movingRight = true;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        movingRight = Random.Range(0, 1f) < .5f;
        renderer.flipX = !movingRight;
        chosenSpeed = Random.Range(moveSpeed.x, moveSpeed.y);
    }

    void Update()
    {
        if (GameController.Instance.paused)
            return;
        transform.position += transform.right * chosenSpeed * Time.deltaTime * (movingRight ? 1 : -1);
    }
}
