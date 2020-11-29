using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBagUI : MonoBehaviour
{
    private Sprite s0;
    private Sprite s1;
    private Sprite s2;
    private Sprite s3;
    private Sprite s4;

    private Sprite rose_sprite;
    private Sprite bluebell_sprite;
    private Sprite doughtnut_tree_sprite;


    // Start is called before the first frame update
    void Start()
    {
        s0 = GameObject.Find("s0").GetComponent<Sprite>();
        s1 = GameObject.Find("s1").GetComponent<Sprite>();
        s2 = GameObject.Find("s2").GetComponent<Sprite>();
        s3 = GameObject.Find("s3").GetComponent<Sprite>();
        s4 = GameObject.Find("s4").GetComponent<Sprite>();

        rose_sprite = Resources.Load<Sprite>("Seeds/Rose");
        bluebell_sprite = Resources.Load<Sprite>("Seeds/Bluebell");
        doughtnut_tree_sprite = Resources.Load<Sprite>("Seeds/Doughnut");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
