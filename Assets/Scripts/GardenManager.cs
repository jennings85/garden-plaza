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
    public float growthNeeded;

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

    public GameObject CheckInGarden(string item)
    {
        foreach(GameObject g in curPlants)
        {
            if(g.GetComponent<Plant>().pType == item && g.GetComponent<Plant>().getGrowth() <= growthNeeded) //Plant exists and is mature
            {
                return g;
            }
        }
        return null;
    }

    public int HowManyInGarden(string item)
    {
        int count = 0;
        foreach (GameObject g in curPlants)
        {
            if (g.GetComponent<Plant>().pType == item && g.GetComponent<Plant>().getGrowth() <= growthNeeded) //Plant exists and is mature
            {
                count++;
            }
        }
        return count;
    }

    public void KillPlant(GameObject died)
    {
        curPlants.Remove(died);
    }
}
