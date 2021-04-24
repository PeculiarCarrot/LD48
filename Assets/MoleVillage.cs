using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleVillage : MonoBehaviour
{
    public GameObject molePrefab;
    [MinMaxSlider(1, 20)]
    public Vector2Int moleCount = new Vector2Int(1, 4);
    Mole[] moles;

    void Awake()
    {
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
        foreach (var m in moles)
        {
            m.Give();
        }
        yield return new WaitForSeconds(.5f);
        GameController.Instance.GetNewHat();
        yield return new WaitForSeconds(2f);
        foreach (var m in moles)
        {
            m.StopGiving();
        }

    }

    void Update()
    {
        
    }
}
