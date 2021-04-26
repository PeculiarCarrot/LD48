using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkMole : MonoBehaviour
{
    Animator animator;
    AudioSource audio;
    public AudioClip[] jumpWarning;
    public AudioClip[] jump;
    bool animStarted;
    Camera cam;
    float off;
    float camSpd;
    bool left;
    SpriteRenderer sprite;

	private void Awake()
	{
        animator = GetComponent<Animator>();
        cam = Camera.main;
        off = Random.Range(-3f, 1);
        camSpd = Random.Range(0f, 3f);
        audio = GetComponent<AudioSource>();
        sprite = GetComponentInChildren<SpriteRenderer>();
	}

	void Start()
    {
        left = Random.Range(0f, 1f) < .5f;
        transform.position = new Vector3(9.4f * (left ? -1 : 1), transform.position.y, transform.position.z);
        if (left)
            transform.Rotate(0, 180, 0);
    }

    void Update()
    {
        if (!animStarted)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = cam.aspect * halfHeight;
            if(transform.position.y > cam.transform.position.y - halfHeight + off)
			{
                animStarted = true;
                animator.SetTrigger("jump");
			}
        }
        else
		{
            animator.SetFloat("animSpeed", GameController.Instance.paused ? 0 : 1);
		}

        if(!GameController.Instance.paused)
        {
            var newPos = transform.position;
            newPos += Vector3.down * camSpd * Time.deltaTime;
            transform.position = newPos;
        }

        audio.panStereo = Utils.Remap(sprite.transform.position.x, -9.4f, -.9f, 9.4f, .9f, true, true);
    }

    public void Jump()
    {
        audio.Stop();
        audio.clip = Utils.Choose(jump);
        audio.Play();
    }

    public void JumpWarning()
    {
        audio.Stop();
        audio.clip = Utils.Choose(jumpWarning);
        audio.Play();
    }

    public void EndJump()
	{
        Destroy(gameObject, 2f);
	}
}
