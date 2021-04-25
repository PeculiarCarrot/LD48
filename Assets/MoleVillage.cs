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

    static string[] quips;
    [MinMaxSlider(1, 20)]
    public Vector2Int moleCount = new Vector2Int(1, 4);
    Mole[] moles;
    bool givingHat;
    bool gaveHat;
    string goalDialogueText = "";

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
    }

    public void OnTriggerEnter2D(Collider2D col)
	{
        if(col.gameObject.CompareTag("Fairy"))
		{
            StartCoroutine(GiveHat());
		}
	}

    IEnumerator GiveHat()
	{
        if (gaveHat)
            yield break;
        gaveHat = true;
        GameController.Instance.Pause();
        StartCoroutine(TypeText(Utils.Choose(quips)));
        foreach (var m in moles)
        {
            m.Give();
        }
        givingHat = true;
    }

    IEnumerator TypeText(string s)
	{
        foreach(var letter in s)
		{
            yield return new WaitForSeconds(.05f);
            dialogueText.text += letter;
		}
	}

    void EndGiveHat()
    {
        givingHat = false;
        GameController.Instance.GetNewHat();
        dialogueText.text = "";
        foreach (var m in moles)
        {
            m.StopGiving();
        }
        GameController.Instance.Unpause();
    }

    void Update()
    {
        if (givingHat && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            EndGiveHat();
    }
}
