using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GardenManager : MonoBehaviour
{
    // Start is called before the first frame update
    //private int level = 0;
    //public GameObject[] curPins;
    public int candyCount;
    public List<GameObject> curPlants;

    private Text candyTxt;

    public void addPlant(GameObject plantToAdd)
    {
        curPlants.Add(plantToAdd);
    }

    public string plantsList()
    {
        string pList = "";
        foreach(GameObject p in curPlants)
        {
            pList += (p.GetComponent<Plant>().pName + ", "); 
        }
        return pList;
    }

    public void candyChange()
    {
        candyTxt.text = candyCount.ToString("N0");
    }
    void Start()
    {
        candyTxt = GameObject.Find("Candy Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
