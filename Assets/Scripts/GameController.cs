using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using TMPro;

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

    public Canvas mainMenuCanvas;
    public Canvas gameOverCanvas;

    public Fairy fairy;
    public GameObject rootHitbox;

    [Header("Obstacles")]
    [SerializeField]
    public WeightedObstacle[] obstacles;

    [Header("Misc")]
    public float scrollSpeed = 4;
    public bool lockFPS;
    public int fps = 30;
    public bool fairyPullsRoot;
    [HideInInspector]
    public bool gameOver;
    public float difficulty;

    [Required]
    public TextMeshProUGUI difficultyText;
    [Required]
    public TextMeshProUGUI meterText;

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
    }

    public void StartGame()
	{

	}

    public void OnRetryClicked()
	{
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

    public void OnQuitClicked()
	{
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update()
    {
        difficultyText.text = "Difficulty: " + difficulty.ToString("0.00");
        meterText.text = (Camera.main.transform.position.y * -.1f).ToString("0.0") + "m";
        if (gameOver)
            return;
        Cursor.visible = false;

        cam.transform.position = cam.transform.position + Vector3.down * scrollSpeed * Time.deltaTime;
        distScrolledSinceLastRootUpdate += scrollSpeed * Time.deltaTime;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        var camPos = Camera.main.transform.position;

        difficulty = Utils.Remap(-camPos.y, 0, 0, 1000, 1, true, true);
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(2))
            Debug.Break();
#endif

        HandleMoveInput();
        UpdateRoot();
        SpawnObjects();
    }

    void SpawnObjects()
	{
        var diff = Mathf.Max(difficulty, .05f);
        if (Random.Range(0, 1f) < diff * .2f)
            SpawnObstacle();
	}

    void SpawnObstacle()
	{
        var obstacle = ChooseWeightedObstacle();
        var o = Instantiate(obstacle.obj);
        var pos = cam.transform.position;
        pos.z = 0;
        pos.y -= 10 + Random.Range(0, 3f);
        pos.x += Random.Range(-7, 7);
        o.transform.position = pos;
        o.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-obstacle.rotate, -obstacle.rotate));
	}

    void GameOver()
	{
        gameOverCanvas.enabled = true;
        gameOver = true;
        Cursor.visible = true;
	}

    public Vector3 ClampInsideCamera(Vector3 pos, float xPad, float yPad)
	{
        return ClampInsideCamera(pos, xPad, xPad, yPad, yPad);
	}

    public Vector3 ClampInsideCamera(Vector3 pos, float leftPad, float rightPad, float topPad, float bottomPad)
	{
        var bounds = Camera.main.OrthographicBounds();
        var left = bounds.center.x - bounds.width / 2 + leftPad;
        var right = bounds.center.x + bounds.width / 2 - rightPad;
        var top = bounds.center.y + bounds.height / 2 - topPad;
        var bottom = bounds.center.y - bounds.height / 2 + bottomPad;
        //Utils.DrawRect(bounds, Color.yellow);
        var newPos = pos;
        newPos.x = Mathf.Clamp(newPos.x, left, right);
        newPos.y = Mathf.Clamp(newPos.y, bottom, top);
        return newPos;
    }
    public void OnFairyHit(Collider2D col)
    {
        if (col.gameObject.CompareTag("FairyHazard"))
        {
            TimeControl.Hitstop(.4f, .2f);
            GameOver();
        }
    }

    public void OnRootHit(Collider2D col)
	{
        if(col.gameObject.CompareTag("Harmful"))
		{
            TimeControl.Hitstop(.4f, .2f);
            GameOver();
		}
        //Debug.Log("ROOT HIT " + col);
	}

    void UpdateRoot()
    {
        //if (timeSinceLastRootNode > timeBetweenNodes)
        {
            lastPos = root.GetPosition(root.positionCount - 1);
            var newPos = lastPos + Vector3.down * distScrolledSinceLastRootUpdate;
            newPos += rootVelocity.XY() * Time.deltaTime;

            if(fairyPullsRoot)
            {
                var rootFairyDelta = fairy.transform.position - lastPos;
                rootFairyDelta.y *= .3f;
                newPos += rootFairyDelta * Time.deltaTime * .33f;
            }

            newPos.y = Mathf.Min(lastPos.y - distScrolledSinceLastRootUpdate * .25f, newPos.y);
            newPos = ClampInsideCamera(newPos, .5f, .5f, 1f, .5f);

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
            rootHitbox.transform.position = root.GetPosition(Mathf.Max(0, root.positionCount - 3));

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

    WeightedObstacle ChooseWeightedObstacle()
	{
        float total = 0;
        foreach(var o in obstacles)
		{
            total += Utils.Remap(difficulty, 0, o.weight.x, 1, o.weight.y);
		}
        float x = Random.Range(0, total);

        foreach (var o in obstacles)
        {
            float w = Utils.Remap(difficulty, 0, o.weight.x, 1, o.weight.y);
            x -= w;

            if (!(x <= 0))
                continue;

            return o;
        }
        return obstacles[0];
    }
}

[System.Serializable]
public class WeightedObstacle
{
    public GameObject obj;
    [Range(0, 180)]
    public float rotate = 0;
    [Range(0, 1f)]
    public float onlySpawnAfter = 0;
    [MinMaxSlider(0, 20)]
    public Vector2 weight = new Vector2(1, 1);
}