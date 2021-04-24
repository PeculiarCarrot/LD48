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
		for (int i = 0; i < Random.Range(moleCount.x, moleCount.y); i++)
		{
            var mole = Instantiate(molePrefab, transform);
		}
    }

    void Update()
    {
        
    }
}
