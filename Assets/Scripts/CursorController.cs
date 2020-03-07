using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    //GardenManagment Specific Values
    private GardenManager GM;
    private bool InGarden = true;

    //Cursor Specific Values
    public Color REDCOLOR;
    public Color GREYCOLOR;
    public float speed;

    //Tool Specific Values
    private string[] toolList = { "Shovel", "Watering Can", "Select", };
    private int curTool = 0;
    private Image modeSprite;
    private int plantCounter = 0;
    private GameObject shovelObj;
    private GameObject waterObj;

    public Animation shovelDig;
    public Animation canWater;
    public int waterStrength;
    public Sprite shovImg;
    public Sprite magImg;
    public Sprite waterImg;

    //UI Specific Values
    private GameObject profOBJ;
    private GameObject lifeArrow;
    private GameObject waterArrow;
    private Text sellText;
    private Text itemNick;
    private Text candyText;
    private Text itemType;
    private Text ageText;
    private GameObject inputF;
    private bool UIVisible = false;

    public GameObject plantToSpawn;
    private GameObject selectedObj;
    private Collider currentCol;



    void Start()
    {
        shovelObj = GameObject.Find("Shovel");
        waterObj = GameObject.Find("Can");
        profOBJ = GameObject.Find("profObjBack");
        modeSprite = GameObject.Find("modeSelect").GetComponent<Image>();
        GM = GameObject.Find("GardenObject").GetComponent<GardenManager>();
        profOBJ = GameObject.Find("profObjBack");
        inputF = GameObject.Find("renameInput");
        lifeArrow = GameObject.Find("lifeArrow");
        waterArrow = GameObject.Find("waterArrow");

        //Text and UI
        sellText = GameObject.Find("sellText").GetComponent<Text>();
        itemNick = GameObject.Find("itemName").GetComponent<Text>();
        itemType = GameObject.Find("itemType").GetComponent<Text>();
        ageText = GameObject.Find("ageText").GetComponent<Text>();
        candyText = GameObject.Find("Candy Text").GetComponent<Text>();

        //Disable UI
        sellText.gameObject.SetActive(false);
        profOBJ.SetActive(false);
        inputF.SetActive(false);
        waterObj.SetActive(false);
    }

    void Update()
    {


        //Change Tool
        if (Input.GetButtonDown("X") && !shovelDig.IsPlaying("Shovel_Dig") && !canWater.IsPlaying("Can_Pour"))
        {
            if(curTool == 0) //SET TO WATER
            {
                curTool++;
                modeSprite.sprite = waterImg;
                shovelObj.SetActive(false);
                waterObj.SetActive(true);

            }
            else if(curTool == 1)//SET TO SELECT
            {
                curTool++;
                modeSprite.sprite = magImg;
                profOBJ.SetActive(false);
                waterObj.SetActive(false);
            }
            else //SET TO SHOVEL
            {
                curTool = 0;
                modeSprite.sprite = shovImg;
                profOBJ.SetActive(false);
                waterObj.SetActive(false);
                shovelObj.SetActive(true);
            }
        }
        //Input w/Tool if PLANT
            if (Input.GetButtonDown("A") && toolList[curTool] == "Shovel" && InGarden)
            {
                if (GM.candyCount >= 250)
                {
                    shovelDig["Shovel_Dig"].wrapMode = WrapMode.Once;
                    shovelDig.Play("Shovel_Dig");
                    GameObject justIn = Instantiate(plantToSpawn, new Vector3(transform.GetChild(0).position.x, 0, transform.GetChild(0).position.z), Quaternion.identity);
                    justIn.GetComponent<Plant>().pNick = "Rose (" + plantCounter + ")";
                    plantCounter++;
                    GM.addPlant(justIn);
                    StartCoroutine(UpdateCandy(GM.candyCount, 250));
                    GM.candyCount -= 250;
                    candyText.text = GM.candyCount.ToString("N0");
                }
            }
            else if (Input.GetButton("A") && toolList[curTool] == "Select" && currentCol != null && currentCol.tag == "Plant")
            {
                profOBJ.SetActive(true);
                UIVisible = true;
                selectedObj = currentCol.gameObject;
                ageText.text = selectedObj.GetComponent<Plant>().getAge();
                itemNick.text = selectedObj.GetComponent<Plant>().pNick;
                itemType.text = selectedObj.GetComponent<Plant>().pName;
            }
            else if (Input.GetButton("A") && toolList[curTool] == "Watering Can" && currentCol != null && currentCol.tag == "Plant")
            {
                canWater["Can_Pour"].wrapMode = WrapMode.Once;
                canWater.Play("Can_Pour");
                profOBJ.SetActive(true);
                UIVisible = true;
                selectedObj = currentCol.gameObject;
                ageText.text = selectedObj.GetComponent<Plant>().getAge();
                itemNick.text = selectedObj.GetComponent<Plant>().pNick;
                itemType.text = selectedObj.GetComponent<Plant>().pName;
                //WATER CODE
                selectedObj.GetComponent<Plant>().waterLevel += Time.deltaTime * waterStrength;
            }
            else if(Input.GetButton("A") && toolList[curTool] == "Select" && currentCol != null && currentCol.tag == "Pickup")
            {
                Pickup PS = currentCol.GetComponent<Pickup>();
                Destroy(currentCol.gameObject);
                StartCoroutine(UpdateCandy(GM.candyCount, -PS.candyValue));
                GM.candyCount += PS.candyValue;
                candyText.text = GM.candyCount.ToString("N0");
        }
        

        //Update UI
        if (UIVisible)
        {
            ageText.text = selectedObj.GetComponent<Plant>().getAge();
            lifeArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2((97 + (selectedObj.GetComponent<Plant>().getLifeLerp() * -200f)), -43);
            waterArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(((selectedObj.GetComponent<Plant>().waterLevel - 100f)), -159);
        }

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Plant")
        {
            //Debug.Log("Entered: " +collision.gameObject.name);
            currentCol = collision;
        }
        else if (collision.gameObject.tag == "Pickup")
        {
            currentCol = collision;
            Pickup PS = currentCol.GetComponent<Pickup>();
            sellText.gameObject.SetActive(true);
            sellText.text = "Sell for " + PS.candyValue + " candy (Press A)";
            currentCol = collision;
        }
        else if (collision.gameObject.name == "GardenObject")
        {
            InGarden = true;
            StartCoroutine(EnterGarden());
        }
    }
    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Plant")
        {
            //Debug.Log("Exited: " +collision.gameObject.name);
            currentCol = null;
        }
        else if (collision.gameObject.tag == "Pickup")
        {
            sellText.gameObject.SetActive(false);
            currentCol = null;
        }
        else if (collision.gameObject.name == "GardenObject")
        {
            InGarden = false;
            StartCoroutine(LeftGarden());
        }
    }

    void FixedUpdate()
    {

        //reading the input:
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        //assuming we only using the single camera:
        var camera = Camera.main;

        //camera forward and right vectors:
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        //this is the direction in the world space we want to move:
        var desiredMoveDirection = forward * verticalAxis + right * horizontalAxis;

        //now we can apply the movement:
        transform.GetChild(0).Rotate(new Vector3(0, 100*Time.deltaTime, 0));
        if (!shovelDig.IsPlaying("Shovel_Dig"))
        {
            transform.Translate(desiredMoveDirection * speed * Time.deltaTime);
        }
    }
    public IEnumerator LeftGarden()
    {
        float ElapsedTime = 0.0f;
        float TotalTime = 0.25f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(REDCOLOR, GREYCOLOR, (ElapsedTime / TotalTime));
            yield return null;
        }
    }
    public IEnumerator EnterGarden()
    {
        float ElapsedTime = 0.0f;
        float TotalTime = 0.25f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(GREYCOLOR, REDCOLOR, (ElapsedTime / TotalTime));
            yield return null;
        }
    }

    public IEnumerator UpdateCandy(int startCount, int sub)
    {
        float lerp = 0.0f;
        float duration = 0.5f;
        int curCount = startCount;
        int endCandy = (startCount - sub);
        while (lerp < duration)
        {
            lerp += Time.deltaTime;
            curCount = (int)Mathf.Lerp(startCount, endCandy, lerp);
            candyText.text = curCount.ToString("N0");
            yield return null;
        }
        candyText.text = endCandy.ToString("N0");
    }

    public void InformedDeath(GameObject died)
    {
        if(selectedObj == died)
        {
            UIVisible = false;
            profOBJ.SetActive(false);
            selectedObj = null;
        }
    }
}
