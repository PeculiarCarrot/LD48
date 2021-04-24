using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer))]
public class Fairy : MonoBehaviour
{
    public float lerpSpeed = 7f;
    new SpriteRenderer renderer;

	private void Awake()
	{
        renderer = GetComponentInChildren<SpriteRenderer>();
	}

	void Start()
    {
        
    }

    void LateUpdate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.y -= Time.deltaTime * GameController.Instance.scrollSpeed;

        var off = Vector3.down * GameController.Instance.scrollSpeed * Time.deltaTime;
        transform.position = transform.position + off;

        transform.position = Vector3.Lerp(transform.position, mousePos, lerpSpeed * Time.deltaTime);
    }
}
