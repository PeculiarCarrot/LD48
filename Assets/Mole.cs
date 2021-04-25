using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Mole : MonoBehaviour
{
    [MinMaxSlider(0, 10f)]
    public Vector2 timeBetweenWander = new Vector2(.5f, 5f);

    public float maxWanderDistance = 2f;
    [MinMaxSlider(0f, 5f)]
    public Vector2 moveSpeed = new Vector2(.2f, 2f);
    public float maxSpawnDistFromVillageCenter = 2f;

    Vector3 goalPos;
    Vector3 startPos;
    float chosenMoveSpeed;
    float timeUntilWander;
    Animator animator;
    bool facingRight;
    public MoleState state;
    public Sprite snoozySprite;
    bool snoozy;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

	private void Start()
	{
        var newPos = transform.localPosition;
        newPos.x = Random.Range(-maxSpawnDistFromVillageCenter, maxSpawnDistFromVillageCenter);
        newPos.y = -2.87f;
        chosenMoveSpeed = Random.Range(moveSpeed.x, moveSpeed.y);

        if(Random.Range(1, 10) == 1)
		{
            snoozy = true;
            GetComponentInChildren<SpriteRenderer>().sprite = snoozySprite;
            newPos.x = Random.Range(-maxSpawnDistFromVillageCenter - maxWanderDistance, maxSpawnDistFromVillageCenter + maxWanderDistance);
        }
        transform.localPosition = newPos;
        startPos = transform.position;
    }

	public void GetExcited()
	{
        if (snoozy)
            return;
        state = MoleState.Giving;
        animator.SetTrigger("give");
	}

    public void StopBeingExcited()
    {
        if (snoozy)
            return;
        state = MoleState.Idling;
        animator.SetTrigger("idle");
        timeUntilWander = Random.Range(timeBetweenWander.x, timeBetweenWander.y);
    }

    void Update()
    {
        if (snoozy)
            return;
        if (state == MoleState.Moving)
		{
            Debug.DrawLine(transform.position, goalPos, Color.yellow);
            if(Vector3.Distance(transform.position, goalPos) > .05f)
			{
                transform.position = Vector3.MoveTowards(transform.position, goalPos, chosenMoveSpeed * Time.deltaTime);
			}
            else
			{
                state = MoleState.Idling;
                animator.SetTrigger("idle");
                timeUntilWander = Random.Range(timeBetweenWander.x, timeBetweenWander.y);
            }
		}
        else if (state == MoleState.Idling)
        {
            if (timeUntilWander <= 0)
            {
                WalkToNewPos();
            }
            else
                timeUntilWander -= Time.deltaTime;
        }
    }

    void WalkToNewPos()
    {
        if (snoozy)
            return;
        goalPos = startPos + (Vector2.right * new Vector2(Random.Range(-maxWanderDistance, maxWanderDistance), 0)).XY();
        state = MoleState.Moving;

        if (facingRight && goalPos.x < transform.position.x)
        {
            facingRight = false;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (!facingRight && goalPos.x > transform.position.x)
        {
            facingRight = true;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        animator.SetTrigger("walk");
    }

    public enum MoleState
	{
        Idling,
        Moving,
        Giving
	}
}
