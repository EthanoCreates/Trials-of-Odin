using System.Collections.Generic;
using UnityEngine;

public class EnemyAppearanceRandomizer : MonoBehaviour
{
    public List<GameObject> Skin;
    public List<GameObject> Armour;

    private void Start()
    {
        foreach (GameObject obj in Armour)
        {
            if(Random.Range(0,2) == 0)
            {
                obj.SetActive(false);
            }
        }
    }
}
