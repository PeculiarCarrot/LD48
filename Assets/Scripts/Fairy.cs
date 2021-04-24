using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Fairy : MonoBehaviour
{
    public float lerpSpeed = 5f;
    new SpriteRenderer renderer;

	private void Awake()
	{
        renderer = GetComponent<SpriteRenderer>();
	}

	void Start()
    {
        
    }

    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Debug.DrawLine(mousePos, Vector3.zero, Color.magenta);

        transform.position = Vector3.Lerp(transform.position, mousePos, Time.deltaTime * lerpSpeed);
    }
}
