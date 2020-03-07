using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Requirement
{
    public string task;
    public string item;
    public string visRes;
    public int count;
    public bool complete;
    public Requirement(string vr, string t, string i, int c)
    {
        visRes = vr;
        task = t;
        item = i;
        count = c;
        complete = false;
    }
}
public class Pinata : MonoBehaviour
{
    public string breed;
    public bool resident;
    public bool inGarden;
    public List<string> rawReq;
    public List<Requirement> vRequirements = new List<Requirement>();
    public List<Requirement> rRequirements = new List<Requirement>();

    public Color GREYCOLOR;
    public Color REDCOLOR;

    private GardenManager GM;

    void Start()
    {
        GM = GameObject.Find("GardenObject").GetComponent<GardenManager>();
        foreach(string s in rawReq)
        {
            string[] i = s.Split(',');
            string _vr = i[0];
            string _t = i[1];
            int _c = int.Parse(i[2]);
            string _i = i[3];
            if(_vr == "V")
            {
                vRequirements.Add(new Requirement(_vr, _t, _i, _c));

            }
            else if(_vr == "R")
            {
                rRequirements.Add(new Requirement(_vr, _t, _i, _c));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!inGarden)
        {
            if(!resident)
            {
                GameObject plantMoveTo = GM.CheckInGarden(vRequirements[0].item);
                if (plantMoveTo != null && !rRequirements[0].complete)
                {
                    if (Vector3.Distance(transform.position, plantMoveTo.transform.position) > 2.0f)
                    {
                        float step = 5 * Time.deltaTime; // calculate distance to move
                        transform.position = Vector3.MoveTowards(transform.position, plantMoveTo.transform.position, step);
                    }
                    else
                    {
                        plantMoveTo.GetComponent<Plant>().Killed();
                        rRequirements[0].complete = true;
                        resident = true;
                        StartCoroutine(BecomeResident());
                    }
                }
            }
        }
    }

    public IEnumerator BecomeResident()
    {
        float ElapsedTime = 0.0f;
        float TotalTime = 5.00f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            transform.GetComponent<Renderer>().material.color = Color.Lerp(GREYCOLOR, REDCOLOR, (ElapsedTime / TotalTime));
            yield return null;
        }
    }
}
