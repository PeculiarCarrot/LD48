using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Vector3 dir;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = transform.position + dir * Time.deltaTime;
    }
}
