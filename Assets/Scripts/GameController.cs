using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public LineRenderer root;
    public float timeBetweenNodes = .2f;
    float timeSinceLastRootNode;

    void Start()
    {
        
    }

    void Update()
    {
        if (timeSinceLastRootNode > timeBetweenNodes)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            root.positionCount++; root.SetPosition(root.positionCount - 1, mousePos);
            timeSinceLastRootNode = 0;
        }
        else
            timeSinceLastRootNode += Time.deltaTime;
    }
}
