using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideWhenCamIsLessThan : MonoBehaviour
{
    Camera cam;
    public float threshold = 20;

    void Awake()
	{
        cam = Camera.main;
	}

    void Update()
    {
        if(cam.transform.position.y < threshold)
		{
            gameObject.SetActive(false);
		}
    }
}
