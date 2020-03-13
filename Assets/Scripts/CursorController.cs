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
    private string[] toolList = { "Shovel", "Watering Can", "Select", "Surface Packet"};
    private int curTool = 0;
    private Image modeSprite;
    private int plantCounter = 0;
    private GameObject shovelObj;
    private GameObject waterObj;
    private bool testVar = true;

    public Animation shovelDig;
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
    public Text mat;

    //Plants
    public GameObject rose;
    public GameObject bluebell;

    private GameObject plantToSpawn;
    private GameObject selectedObj;
    private Collider currentCol;

    //Terrain
    public Texture2D curTexture;
    public Terrain tur;
    public int tPosX;
    public int tPosZ;
    public float[] textureValues;
    private float[,,] originalTerrain;

    void Start()
    {
        plantToSpawn = rose;
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

        //Save Terrain
        originalTerrain = tur.terrainData.GetAlphamaps(0, 0, tur.terrainData.alphamapWidth, tur.terrainData.alphamapHeight);

    }

    void Update()
    {
        //Change Plant Testing
        if (Input.GetButtonDown("Y"))
        {
            tur.terrainData.SetAlphamaps(0, 0, originalTerrain);
            if (testVar)
            {
                plantToSpawn = bluebell;
                testVar = false;
            }
            else
            {
                plantToSpawn = rose;
                testVar = true;
            }
        }

        //Change Tool
        if (Input.GetButtonDown("X") && !shovelDig.IsPlaying("Shovel_Dig"))//!canWater.IsPlaying("Can_Pour")
        {
            if (curTool == 0) //SET TO WATER
            {
                curTool++;
                modeSprite.sprite = waterImg;
                shovelObj.SetActive(false);
                waterObj.SetActive(true);

            }
            else if (curTool == 1)//SET TO SELECT
            {
                curTool++;
                modeSprite.sprite = magImg;
                profOBJ.SetActive(false);
                waterObj.SetActive(false);
            }
            else if (curTool == 2)//SET TO SURFACE PACKET
            {
                curTool++;
            }
            else
            {
                curTool = 0;
                modeSprite.sprite = shovImg;
                profOBJ.SetActive(false);
                waterObj.SetActive(false);
                shovelObj.SetActive(true);
            }
        }
        //Input w/Surface Packet
        if (Input.GetButton("A") && toolList[curTool] == "Surface Packet" && InGarden)
        {
            DrawTexture(curTexture);
        }
        else
        {
            mat.text = TextureOnTopOf();
        }

        //Input w/Tool if PLANT
        if (Input.GetButtonDown("A") && toolList[curTool] == "Shovel" && InGarden && currentCol == null) // && !shovelDig.IsPlaying("Shovel_Dig") 
        {
            if (GM.candyCount >= 250)
            {
                shovelDig["Shovel_Dig"].wrapMode = WrapMode.Once;
                shovelDig.Play("Shovel_Dig");

                var euler = transform.eulerAngles;
                euler.y = Random.Range(0.0f, 360.0f);
                GameObject justIn = Instantiate(plantToSpawn, new Vector3(transform.GetChild(0).position.x, 0, transform.GetChild(0).position.z), Quaternion.identity);
                justIn.transform.Rotate(euler);
                justIn.GetComponent<Plant>().pNick += " (" + plantCounter + ")";
                int price = justIn.GetComponent<Plant>().price;
                plantCounter++;
                GM.addPlant(justIn);
                StartCoroutine(UpdateCandy(GM.candyCount, price));
                GM.candyCount -= price;
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
            profOBJ.SetActive(true);
            UIVisible = true;
            selectedObj = currentCol.gameObject;
            ageText.text = selectedObj.GetComponent<Plant>().getAge();
            itemNick.text = selectedObj.GetComponent<Plant>().pNick;
            itemType.text = selectedObj.GetComponent<Plant>().pName;
            //WATER CODE
            selectedObj.GetComponent<Plant>().waterLevel += Time.deltaTime * waterStrength;
        }
        else if (Input.GetButton("A") && toolList[curTool] == "Select" && currentCol != null && currentCol.tag == "Pickup")
        {
            Pickup PS = currentCol.GetComponent<Pickup>();
            Destroy(currentCol.gameObject);
            StartCoroutine(UpdateCandy(GM.candyCount, -PS.candyValue));
            GM.candyCount += PS.candyValue;
            candyText.text = GM.candyCount.ToString("N0");
            sellText.gameObject.SetActive(false);
        }


        //Update UI
        if (UIVisible)
        {
            ageText.text = selectedObj.GetComponent<Plant>().getAge();
            lifeArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2((97 + (selectedObj.GetComponent<Plant>().getLifeLerp() * -200f)), -43);
            waterArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(((selectedObj.GetComponent<Plant>().waterLevel - 100f)), -159);
        }

    }

    void DrawTexture(Texture2D tx)
    {
        Vector3 terrainPosition = transform.position - tur.transform.position;
        Vector3 mapPosition = new Vector3(terrainPosition.x / tur.terrainData.size.x, 0, terrainPosition.z / tur.terrainData.size.z);
        tPosX = (int)(mapPosition.x * tur.terrainData.alphamapWidth);
        tPosZ = (int)(mapPosition.z * tur.terrainData.alphamapHeight);
        float[,,] map = tur.terrainData.GetAlphamaps(0, 0, tur.terrainData.alphamapWidth, tur.terrainData.alphamapHeight);
        map[tPosZ, tPosX, 0] = 0;
        map[tPosZ, tPosX, 1] = 1;
        map[tPosZ, tPosX, 2] = 0;
        tur.terrainData.SetAlphamaps(0, 0, map);
    }

    string TextureOnTopOf()
    {
        Vector3 terrainPosition = transform.position - tur.transform.position;
        Vector3 mapPosition = new Vector3(terrainPosition.x / tur.terrainData.size.x, 0, terrainPosition.z / tur.terrainData.size.z);
        tPosX = (int)(mapPosition.x * tur.terrainData.alphamapWidth);
        tPosZ = (int)(mapPosition.z * tur.terrainData.alphamapHeight);
        float[,,] splatMap = tur.terrainData.GetAlphamaps(tPosX, tPosZ, 1, 1);
        textureValues[0] = splatMap[0, 0, 0];
        textureValues[1] = splatMap[0, 0, 1];
        textureValues[2] = splatMap[0, 0, 2];
        float most = 0;
        int mostIndex = 0;
        for (int i = 0; i < textureValues.Length; i++)
        {
            if (textureValues[i] > most)
            {
                most = textureValues[i];
                mostIndex = i;
            }
        }
        if (mostIndex == 0)
        {
            return "Grass";
        }
        else if (mostIndex == 1)
        {
            return "Sand";
        }
        else
        {
            return "Dirt";
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Plant")
        {
            //Debug.Log("Entered: " +collision.gameObject.name);
            currentCol = collision;
        }
        else if (collision.gameObject.tag == "Pickup" && toolList[curTool] == "Select")
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
        else if (collision.gameObject.tag == "Pickup" && toolList[curTool] == "Select")
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

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.tag == "Plant")
        {
            currentCol = collision;
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
        transform.GetChild(0).Rotate(new Vector3(0, 100 * Time.deltaTime, 0));
        //if (!shovelDig.IsPlaying("Shovel_Dig"))
        //{
        transform.Translate(desiredMoveDirection * speed * Time.deltaTime);
        //}
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
        if (selectedObj == died)
        {
            UIVisible = false;
            profOBJ.SetActive(false);
            selectedObj = null;
        }
    }

    void OnApplicationQuit()
    {
        tur.terrainData.SetAlphamaps(0, 0, originalTerrain);
    }
}
