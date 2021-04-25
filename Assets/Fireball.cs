using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [MinMaxSlider(3f, 15f)]
    public Vector2 moveSpeed = new Vector2(3, 15);

    float chosenMoveSpeed;

    void Awake()
    {
        chosenMoveSpeed = Random.Range(moveSpeed.x, moveSpeed.y);
    }

    void Update()
    {
        if (GameController.Instance.paused)
            return;
        transform.position = transform.position + chosenMoveSpeed * transform.right * Time.deltaTime;
    }
}
