using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Imp : MonoBehaviour
{
    public GameObject fireballPrefab;
    Animator animator;
    bool facingRight;
    public Vector2Int fireballCount = new Vector2Int(3, 7);
    public float spread = 20;
    [MinMaxSlider(1f, 10f)]
    public Vector2 timeBetweenCasts = new Vector2(1.5f, 5);
    public Vector2Int castsBeforeLeave = new Vector2Int(3, 10);
    new AudioSource audio;

    int castsLeft;
    bool leaving;

    public AudioClip fireballInitial;
    public AudioClip[] spawnIn;
    public AudioClip[] useFireballVoice;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        castsLeft = Random.Range(castsBeforeLeave.x, castsBeforeLeave.y);
    }

	private void Start()
	{
        StartCoroutine(DoStuff());
	}

    IEnumerator DoStuff()
	{
        yield return new WaitForSeconds(.2f);
        audio.PlayOneShot(Utils.Choose(spawnIn));
        yield return new WaitForSeconds(1.3f);
        while (castsLeft > 0)
        {
            if (!GameController.Instance.paused)
            {
                animator.SetTrigger("cast");
                castsLeft--;
            }
            yield return new WaitForSeconds(Random.Range(timeBetweenCasts.x, timeBetweenCasts.y));
        }
        yield return new WaitForSeconds(1.5f);
        leaving = true;
    }

	void LateUpdate()
    {
        if (GameController.Instance.paused)
            return;
        if (leaving)
		{
            transform.position += Vector3.up * Time.deltaTime;
            return;
		}

        var fairy = GameController.Instance.fairy;
        if (facingRight && fairy.transform.position.x < transform.position.x)
        {
            facingRight = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (!facingRight && fairy.transform.position.x > transform.position.x)
        {
            facingRight = true;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        var newPos = transform.position;
        newPos.y = Mathf.Lerp(newPos.y, fairy.transform.position.y - GameController.Instance.scrollSpeed * 13f * Time.deltaTime,
            Time.deltaTime * 2f);
        transform.position = newPos;
    }

    public void OnCast()
	{
        audio.PlayOneShot(fireballInitial);
        if(Random.Range(1, 5) == 1)
            audio.PlayOneShot(Utils.Choose(useFireballVoice));
        var fairyPos = GameController.Instance.fairy.transform.position;
        fairyPos.y -= GameController.Instance.scrollSpeed;
        for (int i = 0; i < Random.Range(fireballCount.x, fireballCount.y); i++)
        {
            var go = Instantiate(fireballPrefab);
            go.transform.position = transform.position + Vector3.right * (facingRight ? 1 : -1) * .5f;
            var diff = go.transform.position - fairyPos;
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 180 + Random.Range(-spread, spread));
        }
    }
}
