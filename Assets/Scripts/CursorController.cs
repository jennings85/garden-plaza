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
    private GameObject cam;

    //Tool Specific Values
    private string[] toolList = {"Shovel", "Watering Can", "Surface Packet", "Seed Bag", };
    private int curTool = 0;
    private bool onTool = false;
    private int plantCounter = 0;
    private GameObject shovelObj;
    private GameObject waterObj;
    private GameObject surfaceObj;
    private bool testVar = true;

    public int waterStrength;

    public Material grassPack;
    public Material sandPack;
    public Material dirtPack;

    private GameObject waterFX;
    private ParticleSystem toolFX;

    //UI Specific Values
    private GameObject profOBJ;
    private GameObject lifeArrow;
    private GameObject waterArrow;
    private Text sellText;
    private Text itemNick;
    private Text candyText;
    private Text itemType;
    private Text ageText;
    private bool UIVisible = false;
    public Text debugText;
    public Animation UIFlip;
    private GameObject pauseUI;


    private Image topRightImg;
    public Sprite fullTR;
    public Sprite noYTR;
    public Sprite noATR;
    private Text aText;
    private Text xText;
    private Text yText;
    private Text bText;

    //Plants
    public GameObject rose;
    public GameObject bluebell;

    private GameObject plantToSpawn;
    private GameObject selectedObj;
    private Collider currentCol;

    //Terrain
    private string curTexture;
    public Terrain tur;
    public int tPosX;
    public int tPosZ;
    public float[] textureValues;
    private float[,,] originalTerrain;

    //Animation
    private Animator shovelAnim;
    private Animator canAnim;
    private Animator packetAnim;
    private Animator toolUIAnim;
    private GameObject arrowUI;

    public bool paused = false;
    private bool justPaused = false;
    private bool toolPickerUp = false;
    private int selectedTool = 0;

    void Start()
    {
        waterFX = GameObject.Find("WaterFX");
        waterFX.SetActive(false);
        toolFX = GameObject.Find("ToolFX").GetComponent<ParticleSystem>();
        cam = GameObject.Find("cameraHelper");
        curTexture = "Grass";
        pauseUI = GameObject.Find("Pause UI");
        topRightImg = GameObject.Find("Selector Image").GetComponent<Image>();
        aText = GameObject.Find("A_TEXT").GetComponent<Text>();
        bText = GameObject.Find("B_TEXT").GetComponent<Text>();
        xText = GameObject.Find("X_TEXT").GetComponent<Text>();
        yText = GameObject.Find("Y_TEXT").GetComponent<Text>();

        plantToSpawn = rose;

        shovelObj = GameObject.Find("Shovel");
        waterObj = GameObject.Find("Can");
        surfaceObj = GameObject.Find("Surface Packet");
        shovelAnim = shovelObj.GetComponent<Animator>();
        canAnim = waterObj.GetComponent<Animator>();
        packetAnim = surfaceObj.GetComponent<Animator>();
        toolUIAnim = GameObject.Find("Tool Picker UI").GetComponent<Animator>();
        arrowUI = GameObject.Find("Tool Picker Arrow");

        profOBJ = GameObject.Find("profObjBack");
        GM = GameObject.Find("GardenObject").GetComponent<GardenManager>();
        profOBJ = GameObject.Find("profObjBack");
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
        pauseUI.SetActive(false);

        //Save Terrain
        originalTerrain = tur.terrainData.GetAlphamaps(0, 0, tur.terrainData.alphamapWidth, tur.terrainData.alphamapHeight);

    }

    void Update()
    {
        //Tool UI is Up
        if(toolUIAnim.GetCurrentAnimatorStateInfo(0).IsName("Tool_UI_Switch_In"))
        {

            float aim_angle = -1000;
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                float x = -Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                aim_angle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            }
            if(aim_angle != -1000)
            {
                if (aim_angle > -40 && aim_angle < 40)
                {
                    aim_angle = 0;
                    selectedTool = 0;
                }
                else if (aim_angle < -40 && aim_angle > -140)
                {
                    aim_angle = -90;
                    selectedTool = 1;
                }
                else if (aim_angle < -140 || aim_angle > 140)
                {
                    aim_angle = 180;
                    selectedTool = 2;
                }
                else if (aim_angle < 140 && aim_angle > 40)
                {
                    aim_angle = 90;
                    selectedTool = 3;
                }
                arrowUI.transform.rotation = Quaternion.AngleAxis(aim_angle, Vector3.forward);
            }

            if (Input.GetButtonDown("A"))
            {
                ChangeTool(curTool,selectedTool);
                toolUIAnim.SetBool("IsSwitchingOut", true);
            }
            else if (Input.GetButtonDown("B"))
            {
                ChangeTool(curTool,-1);
                toolUIAnim.SetBool("IsSwitchingOut", true);
            }
        }


        if (paused)
        {
            if (Input.GetButtonDown("Pause"))
            {
                Time.timeScale = 1f;
                paused = false;
                pauseUI.SetActive(false);
                justPaused = true;
            }
            if (Input.GetButtonDown("X"))
            {
                Application.Quit();
            }
        }
        if (Input.GetButtonDown("Pause") && !paused && !justPaused)
        {
            Time.timeScale = 0f;
            paused = true;
            pauseUI.SetActive(true);
        }


        if (Input.GetButtonDown("X") && !shovelAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE") && !canAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE") && !packetAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE") && !toolUIAnim.GetCurrentAnimatorStateInfo(0).IsName("Tool_UI_Switch_In"))
        {
            toolPickerUp = true;
            toolUIAnim.SetBool("IsSwitchingIn", true);
        }

        if (!paused && !toolPickerUp)
        {
            if (Input.GetButtonDown("B"))
            {
                profOBJ.SetActive(false);
            }
            if(canAnim.GetCurrentAnimatorStateInfo(0).IsName("Can_Pour"))
            {
                if (waterObj.transform.eulerAngles.x == 291)
                {
                    waterFX.SetActive(true);
                }
                else
                {
                    waterFX.SetActive(false);
                }
                if (currentCol != null && currentCol.tag == "Plant") //water can stuff
                {
                    profOBJ.SetActive(true);
                    UIVisible = true;
                    selectedObj = currentCol.gameObject;
                    ageText.text = selectedObj.GetComponent<Plant>().getAge();
                    itemNick.text = selectedObj.GetComponent<Plant>().pNick;
                    itemType.text = selectedObj.GetComponent<Plant>().pName;
                    selectedObj.GetComponent<Plant>().waterLevel += Time.deltaTime * waterStrength;

                }
            }
            

            if (Input.GetButtonDown("Y") && toolList[curTool] == "Shovel")
            {
                if (testVar)
                {
                    plantToSpawn = bluebell;
                    testVar = false;
                    debugText.text = "Bluebell";
                }
                else
                {
                    plantToSpawn = rose;
                    testVar = true;
                    debugText.text = "Rose";
                }
            }

            justPaused = false;
            //Change Tool

            //Input w/Surface Packet
            if (Input.GetButton("A") && toolList[curTool] == "Surface Packet" && InGarden)
            {
                DrawTexture(curTexture);
            }
            if (Input.GetButtonDown("Y") && toolList[curTool] == "Surface Packet")
            {
                if (curTexture == "Grass")
                {
                    curTexture = "Sand";
                    surfaceObj.GetComponent<Renderer>().material = sandPack;
                }
                else if (curTexture == "Sand")
                {
                    curTexture = "Dirt";
                    surfaceObj.GetComponent<Renderer>().material = dirtPack;

                }
                else
                {
                    curTexture = "Grass";
                    surfaceObj.GetComponent<Renderer>().material = grassPack;
                }
            }

            //Input w/Tool if PLANT
            if (Input.GetButtonDown("A") && toolList[curTool] == "Shovel" && InGarden && currentCol == null)
            {
                if (GM.candyCount >= 250)
                {
                    if (TextureOnTopOf() != "Sand")
                    {
                        shovelAnim.SetBool("IsDigging", true);
                        var euler = transform.eulerAngles;
                        euler.y = Random.Range(0.0f, 360.0f);
                        GameObject justIn = Instantiate(plantToSpawn, new Vector3(transform.GetChild(0).position.x, -.02f, transform.GetChild(0).position.z), Quaternion.identity);
                        justIn.transform.Rotate(euler);
                        justIn.GetComponent<Plant>().pNick += " (" + plantCounter + ")";
                        int price = justIn.GetComponent<Plant>().price;
                        plantCounter++;
                        GM.addPlant(justIn);
                        StartCoroutine(UpdateCandy(GM.candyCount, price));
                        GM.candyCount -= price;
                        candyText.text = GM.candyCount.ToString("N0");
                    }
                    else
                    {
                        debugText.text = "Can't plant on sand!";
                    }

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
            else if (Input.GetButtonDown("A") && toolList[curTool] == "Watering Can" && !canAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE"))
            {
                canAnim.SetBool("IsPouring", true);
            }
            else if (Input.GetButton("A") && toolList[curTool] == "Select" && currentCol != null && currentCol.tag == "Pickup")
            {
                Pickup PS = currentCol.GetComponent<Pickup>();
                Destroy(currentCol.gameObject);
                StartCoroutine(UpdateCandy(GM.candyCount, -PS.candyValue));
                GM.candyCount += PS.candyValue;
                candyText.text = GM.candyCount.ToString("N0");
                sellText.gameObject.SetActive(false);
                topRightImg.sprite = noATR;
                aText.text = "";
            }


            //Update UI
            if (UIVisible)
            {
                ageText.text = selectedObj.GetComponent<Plant>().getAge();
                lifeArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2((97 + (selectedObj.GetComponent<Plant>().getLifeLerp() * -200f)), -43);
                waterArrow.GetComponent<RectTransform>().anchoredPosition = new Vector2(((selectedObj.GetComponent<Plant>().waterLevel - 100f)), -159);
            }
        }
    }

    void ChangeTool(int cT, int nT) //Ran if X is pressed outside pause and no moving animation playing
    {
        //Fade out of tool if not -1 (select)
        if(cT == 1 && nT != 1)
        {
            toolFX.Play();
            shovelAnim.SetBool("IsSwitchingOut", true);
        }
        else if (cT == 2 && nT != 2)
        {
            toolFX.Play();
            canAnim.SetBool("IsSwitchingOut", true);
        }
        else if (cT == 3 && nT != 3)
        {
            toolFX.Play();
            packetAnim.SetBool("IsSwitchingOut", true);
        }
        else if (cT == 2 && nT != 2)
        {
            toolFX.Play();
            //seedAnim.SetBool("IsSwitchingOut", true);
        }
        //Fade in tool if not -1
        if (nT == 1 && cT != 1)
        {
            curTool = 1;
            topRightImg.sprite = noYTR;
            aText.text = "Dig Hole";
            UIFlip["UI_FLIP"].wrapMode = WrapMode.Once;
            UIFlip.Play("UI_FLIP");
            shovelAnim.SetBool("IsSwitchingIn", true);
        }
        else if (nT == 2 && cT != 2)
        {
            curTool = 2;
            topRightImg.sprite = noYTR;
            aText.text = "Pour Water";
            UIFlip["UI_FLIP"].wrapMode = WrapMode.Once;
            UIFlip.Play("UI_FLIP");
            canAnim.SetBool("IsSwitchingIn", true);
        }
        else if (nT == 3 && cT != 3)
        {
            curTool = 3;
            topRightImg.sprite = fullTR;
            aText.text = "Pour Surface";
            yText.text = "Alt. Surface";
            UIFlip["UI_FLIP"].wrapMode = WrapMode.Once;
            UIFlip.Play("UI_FLIP");
            packetAnim.SetBool("IsSwitchingIn", true);
        }
        else if (nT == 4 && cT != 4)
        {
            curTool = 4;
            topRightImg.sprite = fullTR;
            aText.text = "Plant Seed";
            yText.text = "Change Seed";
            UIFlip["UI_FLIP"].wrapMode = WrapMode.Once;
            UIFlip.Play("UI_FLIP");
            //seedAnim.SetBool("IsSwitchingIn", true);
        }
        toolPickerUp = false;
        toolUIAnim.SetBool("IsSwitchingOut", true);
    }

    void DrawTexture(string tx)
    {
        float sN;
        float dI;
        float gR;

        if(tx == "Sand")
        {
            sN = 1;
            dI = 0;
            gR = 0;
        }
        else if(tx == "Dirt")
        {
            sN = 0;
            dI = 1;
            gR = 0;
        }
        else
        {
            sN = 0;
            dI = 0;
            gR = 1;
        }
        Vector3 terrainPosition = transform.position - tur.transform.position;
        Vector3 mapPosition = new Vector3(terrainPosition.x / tur.terrainData.size.x, 0, terrainPosition.z / tur.terrainData.size.z);
        tPosX = (int)(mapPosition.x * tur.terrainData.alphamapWidth);
        tPosZ = (int)(mapPosition.z * tur.terrainData.alphamapHeight);
        float[,,] map = tur.terrainData.GetAlphamaps(235, 235, 42, 42);
        map[tPosZ - 235-1, tPosX - 235-1, 2] = dI;
        map[tPosZ - 235-1, tPosX - 235,   2] = dI;
        map[tPosZ - 235-1, tPosX - 235+1, 2] = dI;
        map[tPosZ - 235, tPosX - 235-1,   2] = dI;
        map[tPosZ - 235, tPosX - 235,     2] = dI;
        map[tPosZ - 235, tPosX - 235+1,   2] = dI;
        map[tPosZ - 235+1, tPosX - 235-1, 2] = dI;
        map[tPosZ - 235+1, tPosX - 235,   2] = dI;
        map[tPosZ - 235+1, tPosX - 235+1, 2] = dI;

        map[tPosZ - 235 - 1, tPosX - 235 - 1, 1] = sN;
        map[tPosZ - 235 - 1, tPosX - 235,     1] = sN;
        map[tPosZ - 235 - 1, tPosX - 235 + 1, 1] = sN;
        map[tPosZ - 235, tPosX - 235 - 1,     1] = sN;
        map[tPosZ - 235, tPosX - 235,         1] = sN;
        map[tPosZ - 235, tPosX - 235 + 1,     1] = sN;
        map[tPosZ - 235 + 1, tPosX - 235 - 1, 1] = sN;
        map[tPosZ - 235 + 1, tPosX - 235,     1] = sN;
        map[tPosZ - 235 + 1, tPosX - 235 + 1, 1] = sN;

        map[tPosZ - 235 - 1, tPosX - 235 - 1, 0] = gR;
        map[tPosZ - 235 - 1, tPosX - 235,     0] = gR;
        map[tPosZ - 235- 1, tPosX - 235 + 1, 0] = gR;
        map[tPosZ - 235, tPosX - 235 - 1,     0] = gR;
        map[tPosZ - 235, tPosX - 235,         0] = gR;
        map[tPosZ - 235, tPosX - 235 + 1,     0] = gR;
        map[tPosZ - 235 + 1, tPosX - 235 - 1, 0] = gR;
        map[tPosZ - 235 + 1, tPosX - 235,     0] = gR;
        map[tPosZ - 235 + 1, tPosX - 235 + 1, 0] = gR;
        tur.terrainData.SetAlphamaps(235, 235, map);
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
            if(toolList[curTool] == "Select")
            {
                topRightImg.sprite = noYTR;
                aText.text = "View Plant";
            }
            currentCol = collision;
        }
        else if (collision.gameObject.tag == "Pickup" && toolList[curTool] == "Select")
        {
            topRightImg.sprite = noYTR;
            aText.text = "Sell Item";
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
            if(toolList[curTool] == "Select")
            {
                topRightImg.sprite = noATR;
                aText.text = "";
            }
        }
        else if (collision.gameObject.tag == "Pickup" && toolList[curTool] == "Select")
        {
            topRightImg.sprite = noATR;
            aText.text = "";
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
            if (toolList[curTool] == "Select")
            {
                topRightImg.sprite = noYTR;
                aText.text = "View Plant";
            }
            currentCol = collision;
        }
    }

    void FixedUpdate()
    {

        //reading the input:
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        var camera = Camera.main;

        var forward = camera.transform.forward;
        var right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        var desiredMoveDirection = forward * verticalAxis + right * horizontalAxis;

        transform.GetChild(0).Rotate(new Vector3(0, 100 * Time.deltaTime, 0));

        if(!shovelAnim.GetCurrentAnimatorStateInfo(0).IsName("Shovel_Dig") && !toolPickerUp)
        {
            transform.Translate(desiredMoveDirection * speed * Time.deltaTime);
        }

        float h = 115 * Input.GetAxis("axisName") * Time.deltaTime;
        cam.transform.Rotate(0, h, 0);
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
