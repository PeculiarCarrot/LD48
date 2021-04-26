using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoleVillage : MonoBehaviour
{
    public GameObject molePrefab;
    public TextAsset quipsFile;
    public TextMeshProUGUI dialogueText;
    public GameObject[] housePrefabs;
    AudioSource audio;
    public AudioClip[] greetingClips;

    static string[] quips;
    [MinMaxSlider(1, 20)]
    public Vector2Int moleCount = new Vector2Int(1, 4);
    Mole[] moles;
    bool gaveHat;
    string goalDialogueText = "";
    Coroutine typeCoroutine;
    MoleVillageState state = MoleVillageState.Idle;
    Hat presentingHat;
    bool interactionEnded;

    enum MoleVillageState
	{
        Idle,
        Talking,
        Giving
	}

    void Awake()
    {
        if(quips == null)
		{
            quips = quipsFile.text.Split('\n');
		}

        audio = GetComponent<AudioSource>();
        var moleNum = Random.Range(moleCount.x, moleCount.y);
        moles = new Mole[moleNum];

        for (int i = 0; i < moleNum; i++)
		{
            moles[i] = Instantiate(molePrefab, transform).GetComponent<Mole>();
		}

        CreateHouse(Utils.Choose(housePrefabs), new Vector3(-2.66f, -2.75f));
        CreateHouse(Utils.Choose(housePrefabs), new Vector3(-.3f, -2.75f));
        CreateHouse(Utils.Choose(housePrefabs), new Vector3(2.2f, -2.75f));
    }

    void CreateHouse(GameObject prefab, Vector3 pos)
	{
        var go = Instantiate(prefab, gameObject.transform);
        go.transform.localPosition = pos;
	}

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Fairy"))
        {
            StartCoroutine(StartConversation());
        }
        else if (col.gameObject.CompareTag("Harmful"))
        {
            if (col.GetComponent<Worm>() != null
                || col.GetComponent<Termite>() != null)
                return;
            Destroy(col.gameObject);
        }
    }

    IEnumerator StartConversation()
	{
        if (gaveHat)
            yield break;
        gaveHat = true;
        state = MoleVillageState.Talking;
        GameController.Instance.Pause();
        goalDialogueText = Utils.Choose(quips);
        typeCoroutine = StartCoroutine(TypeText(goalDialogueText));

        audio.clip = Utils.Choose(greetingClips);
        audio.pitch = Random.Range(.7f, 1.3f);
        audio.Play();

        foreach (var m in moles)
        {
            m.GetExcited();
        }
    }

    IEnumerator TypeText(string s)
	{
        foreach(var letter in s)
		{
            yield return new WaitForSeconds(.025f);
            dialogueText.text += letter;
		}
	}

    void EndConversation()
    {
        dialogueText.text = "";
        StartHatAnimation();
    }

    void StartHatAnimation()
    {
        state = MoleVillageState.Giving;
        presentingHat = GameController.Instance.GetNewHat();
    }

    void EndHatAnimation()
    {
        presentingHat.StopPresenting();
        interactionEnded = true;
        foreach (var m in moles)
        {
            m.StopBeingExcited();
        }
        GameController.Instance.Unpause();
        audio.DOFade(0f, 2f);
    }

    void Update()
    {
        if (interactionEnded)
            return;

        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
		{
            if(state == MoleVillageState.Talking)
            {
                if (dialogueText.text == goalDialogueText)
                {
                    EndConversation();
                }
                else
                {
                    StopCoroutine(typeCoroutine);
                    dialogueText.text = goalDialogueText;
                }
            }
            else if(state == MoleVillageState.Giving)
			{
                EndHatAnimation();
			}
		}
    }
}
