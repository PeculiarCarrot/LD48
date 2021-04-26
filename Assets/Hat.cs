using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Hat : MonoBehaviour
{
    [HideInInspector]
    public Transform hatPos;
    Camera cam;
    bool presenting = true;
    public float spinSpeed = 30f;
    [Required]
    public Transform topPos;
    [Required]
    public GameObject showoff;

	private void Awake()
	{
        cam = Camera.main;
        hatPos = transform.parent;
        transform.parent = null;
	}

	void Start()
    {
        var newPos = cam.transform.position;
        newPos.z = 0;
        transform.position = newPos;
        transform.localScale = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.DOScale(.7f, .5f).SetEase(Ease.OutBack);
        //showoff.transform.localScale = Vector3.zero;
        //showoff.transform.DOScale(1f, 1f).SetEase(Ease.OutCubic);
    }

    public void StopPresenting()
    {
        if(Random.Range(1, 3) == 1)
            GameController.Instance.audio.PlayOneShot(Utils.Choose(GameController.Instance.fairyGetHat));
        presenting = false;
        transform.parent = hatPos;
        DOTween.Complete(transform);
        transform.DOLocalMove(Vector3.zero, .7f).SetEase(Ease.InOutQuad);
        transform.DORotate(new Vector3(0, 0, 0), .7f).SetEase(Ease.InOutQuad);
        transform.DOScale(1f, .7f).SetEase(Ease.InOutQuad);
        showoff.transform.DOScale(0, .5f).onComplete += ()=> {
            showoff.SetActive(false);
            GetComponent<SpriteRenderer>().sortingOrder = 160 + GameController.Instance.hatsObtained;
        };
    }

    void Update()
    {
        if (presenting)
            transform.Rotate(0, 0, spinSpeed * Time.deltaTime);
    }
}
