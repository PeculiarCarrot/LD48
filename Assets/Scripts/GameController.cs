using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using TMPro;
using System.Linq;
using DG.Tweening;
using SpriteShatter;
using UnityEngine.UI;

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

    public GameObject moleVillagePrefab;

    [Header("Misc")]
    public float goalScrollSpeed = 4;
    public float scrollSpeed = 0;
    public bool lockFPS;
    public int fps = 30;
    public bool fairyPullsRoot;
    [HideInInspector]
    public bool paused = true;
    public float difficulty;

    [Required]
    public TextMeshProUGUI difficultyText;
    public GameObject fairyHouse;
    [Required]
    public TextMeshProUGUI meterText;

    public GameObject[] hatPrefabs;

    GameObject[] hatOrderForThisRun;
    [HideInInspector]
    public int hatsObtained = 0;
    bool rootInitialized;
    float timeSinceRootInitialized;
    public float timePerVillage = 10f;
    public bool invincible;
    public Image blackFade;

    public AudioSource menuMusic, gameplayMusic;
    [HideInInspector]
    public new AudioSource audio;

    [Header("Audio")]
    public AudioClip fairyEntrance;
    public AudioClip[] fairyEntranceQuips;
    public AudioClip[] fairyGetHat;
    public AudioClip[] getHat;
    public AudioClip[] rootHit;
    public AudioClip[] gameOverSounds;
    public AudioClip[] fairyGameOver;

    [Range(0, 1f)]
    public float musicVolume = .05f;

    new void Awake()
	{
        blackFade.color = Color.black;
        audio = GetComponent<AudioSource>();
	}

    void Start()
    {
        blackFade.DOFade(0f, 1.2f);
        menuMusic.volume = 0;
        menuMusic.DOFade(musicVolume, 1.2f);
        Cursor.visible = true;
        hatOrderForThisRun = new GameObject[hatPrefabs.Length];
		for (int i = 0; i < hatPrefabs.Length; i++)
		{
            hatOrderForThisRun[i] = hatPrefabs[i];
		}
        hatOrderForThisRun.Shuffle();

        scrollSpeed = 0;

        cam = Camera.main;
        if(lockFPS)
        {
            Application.targetFrameRate = fps;
            timeBetweenNodes = 1f / fps;
        }
        meterText.enabled = false;
        Pause();
    }

    public Hat GetNewHat()
	{
        Transform parent = fairy.hatPos.transform;
        if(hatsObtained > 0)
		{
            var h = parent.GetChild(0).GetComponent<Hat>();
            var s = "";
            while(h.topPos.childCount > 0)
			{
                s += "  ";
                h = h.topPos.GetChild(0).GetComponent<Hat>();
			}
            parent = h.topPos;
		}

        var hat = Instantiate(hatOrderForThisRun[hatsObtained], parent).GetComponent<Hat>();

        audio.PlayOneShot(Utils.Choose(getHat));

        hatsObtained++;
        return hat;
	}

    public void OnStartClicked()
	{
        if(!fairy.gameObject.activeSelf)
            StartCoroutine(DoIntro());
	}

    IEnumerator DoIntro()
	{
        fairy.gameObject.SetActive(true);

        audio.PlayOneShot(fairyEntrance);
        audio.PlayOneShot(Utils.Choose(fairyEntranceQuips));
        var oldScale = Vector3.one;
        fairyHouse.transform.localScale = fairyHouse.transform.localScale * 1.2f;
        fairyHouse.transform.DOScale(oldScale, .5f).SetEase(Ease.OutBounce);

        yield return new WaitForSeconds(1.5f);
        Unpause();
        DOTween.To(() => scrollSpeed, x => scrollSpeed = x, goalScrollSpeed * 1.6f, 1.5f).OnComplete(()=> {
            DOTween.To(() => scrollSpeed, x => scrollSpeed = x, goalScrollSpeed, 1f).SetDelay(3f);
        });
        menuMusic.DOFade(0, 2f).SetDelay(3f);
        gameplayMusic.Play();
        gameplayMusic.DOFade(musicVolume, 2f).SetDelay(3f);
        InitializeRoot();
        yield break;
    }

    void InitializeRoot()
	{
        var arr = new Vector3[maxNodes];
        var p = root.GetPosition(root.positionCount - 1);
        var bounds = cam.OrthographicBounds();
        p.x = cam.transform.position.x;
        p.y = bounds.yMax + 1;
        for (int i = 0; i < maxNodes; i++)
        {
            arr[i] = p;
        }
        root.positionCount = maxNodes;
        root.SetPositions(arr);
        rootInitialized = true;
    }

    public void OnRetryClicked()
	{
        gameplayMusic.DOFade(0, 1.2f);
        blackFade.DOFade(1f, 1.2f).onComplete += () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        };
	}

    public void OnQuitClicked()
	{

        gameplayMusic.DOFade(0, 1.2f);
        blackFade.DOFade(1f, 1.2f).onComplete += () =>
        {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        };
    }

    void Update()
    {
        difficultyText.text = "Difficulty: " + difficulty.ToString("0.00");
        meterText.text = (Camera.main.transform.position.y * -.1f).ToString("0.0") + "m";
        if (paused)
            return;

        if (!meterText.enabled && cam.transform.position.y < -.5f)
            meterText.enabled = true;
        Cursor.visible = false;

        if (rootInitialized)
            timeSinceRootInitialized += Time.deltaTime;

        cam.transform.position = cam.transform.position + Vector3.down * scrollSpeed * Time.deltaTime;
        distScrolledSinceLastRootUpdate += scrollSpeed * Time.deltaTime;
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        var camPos = Camera.main.transform.position;

        difficulty = Utils.Remap(-camPos.y, 0, 0, 480, 1, true, true);
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(2))
            Debug.Break();
#endif

        HandleMoveInput();
        UpdateRoot();
        if (timeSinceRootInitialized > rootDelay)
        {
            SpawnObjects();

            if(timeSinceRootInitialized - rootDelay > (1 + villagesSpawned) * timePerVillage)
			{
                SpawnMoleVillage();
			}
        }
        root.SetPosition(0, cam.transform.position + Vector3.up * 25);
    }
    int villagesSpawned;

    void SpawnObjects()
	{
        var diff = Mathf.Max(difficulty, .1f);
        if (Random.Range(0, 1f) < diff * .13f)
            SpawnObstacle();
	}

    void SpawnMoleVillage()
	{
        villagesSpawned++;
        var o = Instantiate(moleVillagePrefab);
        var pos = cam.transform.position;
        float halfHeight = cam.orthographicSize;
        float halfWidth = cam.aspect * halfHeight;
        pos.z = 0;
        pos.y -= 10 + Random.Range(0, 3f);
        pos.x += Random.Range(-halfWidth / 2, halfWidth / 2);
        o.transform.position = pos;
    }

    void SpawnObstacle()
	{
        var obstacle = ChooseWeightedObstacle();
        var o = Instantiate(obstacle.obj);
        var pos = cam.transform.position;
        float halfHeight = cam.orthographicSize;
        float halfWidth = cam.aspect * halfHeight;
        pos.z = 0;
        pos.y -= 10 + Random.Range(0, 3f);
        pos.x += Random.Range(-halfWidth, halfWidth);
        o.transform.position = pos;
        o.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-obstacle.rotate, obstacle.rotate));
	}

    void GameOver()
    {
        //audio.Stop();
        audio.PlayOneShot(Utils.Choose(rootHit));
        audio.PlayOneShot(Utils.Choose(fairyGameOver));
        //audio.clip = Utils.Choose(gameOverSounds);
        //audio.Play();
        gameOverCanvas.enabled = true;
        paused = true;
        Cursor.visible = true;
	}

    public void Pause()
	{
        paused = true;
	}

    public void Unpause()
	{
        paused = false;
	}

    public Vector3 ClampInsideCamera(Vector3 pos, float xPad, float yPad)
	{
        return ClampInsideCamera(pos, xPad, xPad, yPad, yPad);
	}

    public Vector3 ClampInsideCamera(Vector3 pos, float leftPad, float rightPad, float topPad, float bottomPad)
	{
        var cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float halfWidth = cam.aspect * halfHeight;
        var left = cam.transform.position.x - halfWidth + leftPad; //bounds.center.x - bounds.width / 2 + leftPad;
        var right = cam.transform.position.x + halfWidth - rightPad;//bounds.center.x + bounds.width / 2 - rightPad;
        var top = cam.transform.position.y + halfHeight - topPad;//bounds.center.y + bounds.height / 2 - topPad;
        var bottom = cam.transform.position.y - halfHeight + bottomPad;//bounds.center.y - bounds.height / 2 + bottomPad;
        //Utils.DrawRect(bounds, Color.yellow);
        var newPos = pos;
        newPos.x = Mathf.Clamp(newPos.x, left, right);
        newPos.y = Mathf.Clamp(newPos.y, bottom, top);
        return newPos;
    }
    public void OnFairyHit(Collider2D col)
    {
        if (col.gameObject.CompareTag("FairyHazard") && !invincible)
        {
            //TimeControl.Hitstop(.4f, .2f);
            GameOver();
        }
    }

    public void OnRootHit(Collider2D col)
	{
        if(col.gameObject.CompareTag("Harmful") && !invincible)
		{
            TimeControl.Hitstop(.05f);
            CameraShake.Shake(.1f, .1f);
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

            newPos.y = Mathf.Min(lastPos.y - distScrolledSinceLastRootUpdate * .1f, newPos.y);
            if(timeSinceRootInitialized > rootDelay)
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
            rootHitbox.transform.position = root.GetPosition(Mathf.Max(0, root.positionCount - 4));

            root.SetPosition(root.positionCount - 1, newPos);
            timeSinceLastRootNode = 0;
            distScrolledSinceLastRootUpdate = 0;
        }
       // else
        //    timeSinceLastRootNode += Time.deltaTime;
    }
    public float rootDelay = 4.5f;

    void HandleMoveInput()
	{
        var input = new Vector2(Input.GetAxis("RootX"), Input.GetAxis("RootY"));
        if (timeSinceRootInitialized < rootDelay)
            input = Vector2.zero;

        var t = 1f;
        if (timeSinceRootInitialized < t + rootDelay && timeSinceRootInitialized > rootDelay)
            input.y -= ((t + rootDelay) - timeSinceRootInitialized) * 3f;

        if (input.y > 0)
        {
            var yy = input.y;
            input.y = 0;
            input.Normalize();
            input.y = yy;
        }
        else
        {
            input.Normalize();
        }

        rootVelocity += input * Time.deltaTime * rootAccel;
        rootVelocity.x = Utils.Multiply(rootVelocity.x, rootFric);
        rootVelocity.y = Utils.Multiply(rootVelocity.y, rootFric);
    }

    WeightedObstacle ChooseWeightedObstacle()
	{
        float total = 0;
        foreach(var o in obstacles)
		{
            if(difficulty >= o.onlySpawnAfter)
                total += Utils.Remap(difficulty, 0, o.weight.x, 1, o.weight.y);
		}
        float x = Random.Range(0, total);

        foreach (var o in obstacles)
        {
            if (difficulty < o.onlySpawnAfter)
                continue;
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