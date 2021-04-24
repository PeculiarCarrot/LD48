using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public LineRenderer root;
    public float timeBetweenNodes = .2f;
    float timeSinceLastRootNode;
    public int maxNodes = 10;

    void Start()
    {
        
    }

    void Update()
    {
        if (timeSinceLastRootNode > timeBetweenNodes)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if(root.positionCount < maxNodes)
            {
                root.positionCount++;
            }
            else
			{
                var arr = new Vector3[maxNodes];
                root.GetPositions(arr);
				for (int i = 0; i < maxNodes - 1; i++)
				{
                    arr[i] = arr[i + 1];
				}
                root.SetPositions(arr);
			}

            root.SetPosition(root.positionCount - 1, mousePos);
            timeSinceLastRootNode = 0;
        }
        else
            timeSinceLastRootNode += Time.deltaTime;
    }
}
