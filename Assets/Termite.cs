using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Termite : MonoBehaviour
{
    [MinMaxSlider(0, 5)]
    public Vector2 frequency = new Vector2(0, 5);
    [MinMaxSlider(0, 10)]
    public Vector2 amplitude = new Vector2(0, 10);
    [MinMaxSlider(0, 5)]
    public Vector2 moveSpeed = new Vector2(0, 5);

    float chosenAmplitude = 3;
    float chosenFrequency = 3;
    float chosenMoveSpeed = 2;
    bool movingRight = true;
    new SpriteRenderer renderer;
    void Awake()
    {
        chosenAmplitude = Random.Range(amplitude.x, amplitude.y);
        chosenFrequency = Random.Range(frequency.x, frequency.y);
        chosenMoveSpeed = Random.Range(moveSpeed.x, moveSpeed.y);
        renderer = GetComponent<SpriteRenderer>();
        movingRight = Random.Range(0, 1f) < .5f;
        renderer.flipX = !movingRight;
    }

    void Update()
    {
        if (GameController.Instance.paused)
            return;
        var newPos = transform.position;
        newPos += transform.up * chosenAmplitude * Mathf.Sin(chosenFrequency * Time.time) * Time.deltaTime;
        newPos += transform.right * chosenMoveSpeed * Time.deltaTime * (movingRight ? 1 : -1);
        transform.position = newPos;
    }
}
