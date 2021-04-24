using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Worm : MonoBehaviour
{
    new SpriteRenderer renderer;
    //public float timeBetweenDirectionSwitches = 1f;
    [MinMaxSlider(0, 5f)]
    public Vector2 moveSpeed = new Vector2(.3f, 2);
    float chosenSpeed;
    //float timeUntilDirectionSwitch;
    bool movingRight = true;

    void Start()
    {
       // timeUntilDirectionSwitch = timeBetweenDirectionSwitches;
        renderer = GetComponent<SpriteRenderer>();
        movingRight = Random.Range(0, 1f) < .5f;
        renderer.flipX = !movingRight;
        chosenSpeed = Random.Range(moveSpeed.x, moveSpeed.y);
    }

    void Update()
    {
       /* if (timeUntilDirectionSwitch > 0)
            timeUntilDirectionSwitch -= Time.deltaTime;
        else
        {
            timeUntilDirectionSwitch = timeBetweenDirectionSwitches;
            movingRight = !movingRight;
            renderer.flipX = !movingRight;
        }*/

        transform.position += transform.right * chosenSpeed * Time.deltaTime * (movingRight ? 1 : -1);
    }
}
