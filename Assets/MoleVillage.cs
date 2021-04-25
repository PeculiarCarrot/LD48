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
        Instantiate(prefab, gameObject.transform);
        prefab.transform.localPosition = pos;
	}

    public void OnTriggerEnter2D(Collider2D col)
	{
        if(col.gameObject.CompareTag("Fairy"))
		{
            StartCoroutine(StartConversation());
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
