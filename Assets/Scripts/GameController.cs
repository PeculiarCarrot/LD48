using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    [Header("Root")]
    public LineRenderer root;
    float timeBetweenNodes = .0333333f;
    float timeSinceLastRootNode;
    public int maxNodes = 10;
    public float rootAccel = 3;
    public float rootFric = .95f;
    Vector2 rootVelocity;
    Vector3 lastPos;
    Camera cam;
    float distScrolledSinceLastRootUpdate;

    public Fairy fairy;
    public GameObject rootHitbox;

    [Header("Misc")]
    public float scrollSpeed = 4;
    public bool lockFPS;
    public int fps = 30;

    void Start()
    {
        cam = Camera.main;
        if(lockFPS)
        {
            Application.targetFrameRate = fps;
            timeBetweenNodes = 1f / fps;
        }
        var arr = new Vector3[maxNodes];
        var p = root.GetPosition(root.positionCount - 1);
		for (int i = 0; i < maxNodes; i++)
		{
            arr[i] = p;
		}
        root.SetPositions(arr);
        Cursor.visible = false;
    }

    void Update()
    {
        cam.transform.position = cam.transform.position + Vector3.down * scrollSpeed * Time.deltaTime;
        distScrolledSinceLastRootUpdate += scrollSpeed * Time.deltaTime;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        var camPos = Camera.main.transform.position;

        HandleMoveInput();
        UpdateRoot();
    }

    void UpdateRoot()
    {
        //if (timeSinceLastRootNode > timeBetweenNodes)
        {
            lastPos = root.GetPosition(root.positionCount - 1);
            var newPos = lastPos + Vector3.down * distScrolledSinceLastRootUpdate;
            newPos += rootVelocity.XY() * Time.deltaTime;

            newPos.y = Mathf.Min(lastPos.y - distScrolledSinceLastRootUpdate * .25f, newPos.y);

            if (root.positionCount < maxNodes)
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
            rootHitbox.transform.position = root.GetPosition(Mathf.Max(0, root.positionCount - 10));

            root.SetPosition(root.positionCount - 1, newPos);
            timeSinceLastRootNode = 0;
            distScrolledSinceLastRootUpdate = 0;
        }
       // else
        //    timeSinceLastRootNode += Time.deltaTime;
    }

    void HandleMoveInput()
	{
        rootVelocity += new Vector2(Input.GetAxis("RootX"), Input.GetAxis("RootY"))
            * Time.deltaTime * rootAccel;
        rootVelocity.x = Utils.Multiply(rootVelocity.x, rootFric);
        rootVelocity.y = Utils.Multiply(rootVelocity.y, rootFric);
    }
}
