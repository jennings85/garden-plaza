using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GardenManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int level = 0;
    public int totalXP = 0;
    public int xpToLevel = 50;

    //public GameObject[] curPins;
    public int candyCount;
    public List<GameObject> curPlants;
    public float growthNeeded;
    private Text candyTxt;

    //Grown/Resident Recorder
    #region
    //Plants
    private bool grownRose;
    private bool grownBluebell;

    //Pinatas
    private bool residentPinataX;
    #endregion

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

    public GameObject CheckInGarden(string item, GameObject Pinata)
    {
        GameObject returner = null;
        float closest = 999f;
        foreach(GameObject g in curPlants)
        {
            if(g != null && g.GetComponent<Plant>().pType == item && g.GetComponent<Plant>().getGrowth() <= growthNeeded) //Plant exists and is mature
            {
                if(Vector3.Distance(Pinata.transform.position, g.transform.position) < closest)
                {
                    closest = Vector3.Distance(Pinata.transform.position, g.transform.position);
                    returner = g;
                }
            }
        }
        return returner;
    }

    public int HowManyInGarden(string item)
    {
        int count = 0;
        foreach (GameObject g in curPlants)
        {
            if (g != null && g.GetComponent<Plant>().pType == item && g.GetComponent<Plant>().getGrowth() <= growthNeeded) //Plant exists and is mature
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

    public void AwardCheck(string s, int xp)
    {
        if(s == "Rose" && !grownRose)
        {
            grownRose = true;
            totalXP += xp;
            LevelUpCheck();
        }
    }

    private void LevelUpCheck()
    {
        if(totalXP >= xpToLevel)
        {
            level++;
            xpToLevel = (int)(xpToLevel * 1.2);
        }
    }

    private void LevelingText()
    {
        //levelText.text = level;
        //xpText.text = totalXP + " / " + xpToLevel;
    }
}
