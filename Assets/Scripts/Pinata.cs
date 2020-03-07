using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Requirement
{
    public string task;
    public string item;
    public int count;
    public Requirement(string t, string i, int c)
    {
        task = t;
        item = i;
        count = c;
    }
}
public class Pinata : MonoBehaviour
{
    public string breed;
    public bool resident;
    public bool inGarden;
    public List<string> rawReq;
    public List<Requirement> vRequirements;
    public List<Requirement> rRequirements;

    void Start()
    {
        Debug.Log("Visit Requirements:");
        foreach(Requirement v in vRequirements)
        {
        }
        vRequirements.Add(new Requirement(vRequirements[0].)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
