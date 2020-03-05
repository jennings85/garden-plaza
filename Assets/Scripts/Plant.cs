﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    //Controller
    private CursorController CC;

    public string pName;
    public string pNick;
    public string pType;
    public int pHealth = 100;
    //0 -dead AF, 25 - almost dead, 50 low, 75 fine, 100 perfect, 125 fine, 150, too much, 175 almost dead, 200 dead
    public float waterLevel = 100;
    public float thirstStrength;

    //Plant Life Info
    public float startTime;
    public float endTime = 30;
    private float currentTime;
    private List<GameObject> Children = new List<GameObject>();

    private void Start()
    {
        CC = GameObject.Find("Cursor Object").GetComponent<CursorController>();
        startTime = Time.time;
        endTime = Time.time + 30;
        foreach (Transform child in transform)
        {
            Children.Add(child.gameObject);
        }
    }

    void Update()
    {
        waterLevel -= Time.deltaTime * thirstStrength;
        if(!(Time.time > endTime))
        {
            float growth = Mathf.Lerp(0, 1, (endTime - Time.time) / (endTime - startTime));
            int stage = (int)Mathf.Floor(growth * 3);
            Children[stage].SetActive(true);
            if(waterLevel < 0.0f || waterLevel > 200.0f)
            {
                CC.InformedDeath(gameObject);
                Destroy(gameObject);
            }
        }
        else
        {
            //spawn flower
            CC.InformedDeath(gameObject);
            Destroy(gameObject);
        }
    }

    public string getAge()
    {
        return ((Time.time - startTime)/60).ToString("F1") + "  mins";
    }

    public float getLifeLerp()
    {
        return Mathf.Lerp(0, 1, (endTime - Time.time) / (endTime - startTime));
    }
}