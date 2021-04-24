using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Awake()
    {
        chosenAmplitude = Random.Range(amplitude.x, amplitude.y);
        chosenFrequency = Random.Range(frequency.x, frequency.y);
        chosenMoveSpeed = Random.Range(moveSpeed.x, moveSpeed.y);
    }

    void Update()
    {
        var newPos = transform.position;
        newPos += transform.up * chosenAmplitude * Mathf.Sin(chosenFrequency * Time.time) * Time.deltaTime;
        newPos += transform.right * chosenMoveSpeed * Time.deltaTime;
        transform.position = newPos;
    }
}
