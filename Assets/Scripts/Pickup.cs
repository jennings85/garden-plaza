using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string pickupName;
    public int candyValue;
    public int size;

    public float startTime;
    public float endTime = 180; //dies after this time
    private float currentTime;
    void Start()
    {
        startTime = Time.time;
        endTime = Time.time + 180;
    }
}
