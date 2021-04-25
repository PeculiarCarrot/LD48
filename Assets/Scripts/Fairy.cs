using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer))]
public class Fairy : MonoBehaviour
{
    public float lerpSpeed = 7f;
    new SpriteRenderer renderer;
    public GameObject hatPos;

	private void Awake()
	{
        renderer = GetComponentInChildren<SpriteRenderer>();
	}

	void Start()
    {
        
    }

    void LateUpdate()
    {
        if (GameController.Instance.paused)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.y -= Time.deltaTime * GameController.Instance.scrollSpeed;

        var off = Vector3.down * GameController.Instance.scrollSpeed * Time.deltaTime;
        transform.position = transform.position + off;
        mousePos = GameController.Instance.ClampInsideCamera(mousePos, .25f, .25f);
        var deltaX = transform.position.x - mousePos.x;
        transform.rotation = Quaternion.Euler(0, 0, deltaX * 8);

        transform.position = 
            Vector3.Lerp(transform.position, mousePos, lerpSpeed * Time.deltaTime);
    }
}
