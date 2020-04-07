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
    public bool inMotionAlready = false;
    public List<string> rawReq;
    public List<Requirement> vRequirements = new List<Requirement>();
    public List<Requirement> rRequirements = new List<Requirement>();
    private GardenManager GM;
    private GameObject plantmoveto;
    private Requirement curReq = null;
    public Material startMat;
    public Material endMat;

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
    void FixedUpdate()
    {
        if (!resident)
        {
            if (!ableToVisit)
            {
                bool _makeComplete = true;
                foreach (Requirement vReq in vRequirements) //Go through all Visit Requirements to check if they're in garden
                {
                    if (vReq.task == "GROW")
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
                if (_makeComplete) //Nothing WASNT in garden, make pinata visit allowed
                {
                    ableToVisit = true;
                }
            }
            else //Can visit, not resident yet
            {
                if (curReq == null) //no requirement yet
                {
                    foreach (Requirement r in rRequirements) //go through all
                    {
                        if (r.task == "EAT" && GM.CheckInGarden(r.item, gameObject) && r.count > 0) //able to complete
                        {
                            curReq = r;
                            inMotionAlready = false;
                        }
                    }
                }
                if (curReq != null && !inMotionAlready && plantmoveto == null)
                {
                    if (GM.CheckInGarden(curReq.item, gameObject))
                    {
                        plantmoveto = GM.CheckInGarden(curReq.item, gameObject);
                        inMotionAlready = true;
                    }
                }
                if (plantmoveto != null && Vector3.Distance(transform.position, plantmoveto.transform.position) > 0.7f)
                {
                    float step = 2.5f * Time.deltaTime; // calculate distance to move
                    transform.position = Vector3.MoveTowards(transform.position, plantmoveto.transform.position, step);
                }
                else if(plantmoveto != null && Vector3.Distance(transform.position, plantmoveto.transform.position) < 0.7f)
                {
                    Debug.Log("ATE:" + plantmoveto.name);
                    Debug.Log(curReq.count);
                    plantmoveto.GetComponent<Plant>().Killed();
                    plantmoveto = null;
                    inMotionAlready = false;
                    curReq.count -= 1;
                    if (curReq.count == 0)
                    {
                        curReq.complete = true;
                        inMotionAlready = false;
                        curReq = null;
                    }
                }
                var allGood = true;
                foreach(Requirement r in rRequirements)
                {
                    if(!r.complete)
                    {
                        allGood = false;
                    }
                }
                if(allGood)
                {
                    StartCoroutine(BecomeResident());
                    resident = true;
                }
            }
        }
    }

    public IEnumerator BecomeResident()
    {
        yield return null;
    }
}
