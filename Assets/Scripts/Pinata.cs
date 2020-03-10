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
    public bool ableToVisit;
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
        if(!resident)
        {
            if(!ableToVisit)
            {
                bool _makeComplete = true;
                foreach (Requirement vReq in vRequirements) //Go through all Visit Requirements to check if they're in garden
                {
                    if(vReq.task == "GROW")
                    {
                        if (GM.HowManyInGarden(vReq.item) < vReq.count) //Not enough of required item in garden
                        {
                            _makeComplete = false;
                        }
                        else
                        {
                            vReq.complete = true;
                        }
                    }
                }
                if(_makeComplete) //Nothing WASNT in garden, make pinata visit allowed
                {
                    ableToVisit = true;
                }
            }
            else //Can visit, not resident yet
            {
                bool _makeComplete = true;
                foreach (Requirement rReq in rRequirements) 
                {
                    if (!rReq.complete) //rReq not filled
                    {
                        _makeComplete = false;
                        if(rReq.task == "EAT" && rReq.count != 0)
                        {
                            GameObject plantmoveto = GM.CheckInGarden(rReq.item);
                            if (plantmoveto != null && !rReq.complete)
                            {
                                if (Vector3.Distance(transform.position, plantmoveto.transform.position) > 2.0f)
                                {
                                    float step = 5 * Time.deltaTime; // calculate distance to move
                                    transform.position = Vector3.MoveTowards(transform.position, plantmoveto.transform.position, step);
                                }
                                else
                                {
                                    plantmoveto.GetComponent<Plant>().Killed();
                                    plantmoveto = null;
                                    rReq.count -= 1;
                                }
                            }
                        }
                        else if(rReq.task == "EAT" && rReq.count == 0)
                        {
                            rReq.complete = true;
                        }

                    }
                }
                if (_makeComplete) //All resident reqs done, become res!
                {
                    StartCoroutine(BecomeResident());
                    resident = true;
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
