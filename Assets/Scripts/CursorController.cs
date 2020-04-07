using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    //Initaliaze Variables
    #region
    private bool InGarden = true;
    private bool paused = false;
    private bool toolPickerUp = false;
    private bool justPaused = false;
    private int tPosX;
    private int tPosZ;
    private int plantCounter = 0;
    private int curTool = 0;
    private int selectedTool = 0;
    private int waterStrength = 20;
    private float[] textureValues = new float[3];
    private float[,,] originalTerrain;
    private float speed = 10;
    private float aim_angle = 0;
    private string selectedSeed = "Rose";
    private string curTexture = "Grass";
    private string[] toolList = { "Select", "Shovel", "Watering Can", "Surface Packet", "Seed Bag"};
    private ParticleSystem toolFX;
    private GardenManager GM;
    private GameObject plantUI;
    private GameObject pinataUI;
    private Image waterMask;
    private GameObject cam;
    private GameObject seedObj;
    private GameObject shovelObj;
    private GameObject waterObj;
    private GameObject surfaceObj;
    private GameObject waterFX;
    private GameObject plantToSpawn;
    private GameObject selectedObj;
    private GameObject pauseUI;
    private GameObject arrowUI;
    private GameObject rose;
    private GameObject bluebell;
    private Collider currentCol;
    private Animator shovelAnim;
    private Animator seedAnim;
    private Animator canAnim;
    private Animator packetAnim;
    private Animator UIFlip;
    private Animator toolUIAnim;
    private Material grassPack;
    private Material sandPack;
    private Material dirtPack;
    private Terrain tur;
    private Sprite fullTR;
    private Sprite noYTR;
    private Sprite noATR;
    private Image topRightImg;
    private TMPro.TextMeshProUGUI residentTxt;
    private Text sellText;
    private Text itemNick;
    private Text candyText;
    private Text aText;
    private Text yText;
    #endregion

    //Start sets variables and handling game start code
    void Start()
    {
        //<Set Baseline Variables and find Resources>
        //-------------------------------------------

        //Obtain and set the terrain textures and related variables
        grassPack = Resources.Load<Material>("Mats/GrassPacket");
        sandPack = Resources.Load<Material>("Mats/SandPacket");
        dirtPack = Resources.Load<Material>("Mats/DirtPacket");
        tur = GameObject.Find("Terrain 0").GetComponent<Terrain>();
        originalTerrain = tur.terrainData.GetAlphamaps(0, 0, tur.terrainData.alphamapWidth, tur.terrainData.alphamapHeight);

        //Obtain and set plant Prefabs
        rose = Resources.Load<GameObject>("Prefabs/Rose");
        bluebell = Resources.Load<GameObject>("Prefabs/Bluebell");
        plantToSpawn = rose;

        //Obtain and set UI Sprites
        fullTR = Resources.Load<Sprite>("Images/uiPiece");
        noYTR = Resources.Load<Sprite>("Images/uiPiece_noY");
        noATR = Resources.Load<Sprite>("Images/uiPiece_noA");

        //UI Variables
        aText = GameObject.Find("A_TEXT").GetComponent<Text>();
        yText = GameObject.Find("Y_TEXT").GetComponent<Text>();
        sellText = GameObject.Find("sellText").GetComponent<Text>();
        itemNick = GameObject.Find("plantName").GetComponent<Text>();
        candyText = GameObject.Find("Candy Text").GetComponent<Text>();
        residentTxt = GameObject.Find("PinataCanvas/PinataUI/prText").GetComponent<TMPro.TextMeshProUGUI>();
        pauseUI = GameObject.Find("Pause UI");
        arrowUI = GameObject.Find("Tool Picker Arrow");
        plantUI = GameObject.Find("PlantCanvas");
        pinataUI = GameObject.Find("PinataCanvas");
        waterMask = GameObject.Find("water").GetComponent<Image>();

        //Tool Specific Variables
        waterFX = GameObject.Find("WaterFX");
        toolFX = GameObject.Find("ToolFX").GetComponent<ParticleSystem>();
        topRightImg = GameObject.Find("Selector Image").GetComponent<Image>();
        shovelObj = GameObject.Find("Shovel");
        waterObj = GameObject.Find("Can");
        surfaceObj = GameObject.Find("Surface Packet");
        seedObj = GameObject.Find("Seed Bag");

        //Disable Relevant Objects
        sellText.gameObject.SetActive(false);
        waterFX.SetActive(false);
        plantUI.SetActive(false);
        pinataUI.SetActive(false);
        pauseUI.SetActive(false);

        //Animation Variables
        toolUIAnim = GameObject.Find("Tool Picker UI").GetComponent<Animator>();
        UIFlip = GameObject.Find("Top Right UI").GetComponent<Animator>();
        shovelAnim = shovelObj.GetComponent<Animator>();
        canAnim = waterObj.GetComponent<Animator>();
        packetAnim = surfaceObj.GetComponent<Animator>();
        seedAnim = seedObj.GetComponent<Animator>();

        //Misc. Variables
        GM = GameObject.Find("GardenObject").GetComponent<GardenManager>();
        cam = GameObject.Find("cameraHelper");

    }

    //Currently update is used to handle Pausing, UI updates, and tool usage, futher optimization for tool handling may be needed
    void Update()
    {
        //GAME PAUSED
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
        
        //GAME NOT PAUSED, PAUSE WAS PRESSED
        if (!paused && Input.GetButtonDown("Pause") && !justPaused)
        {
            Time.timeScale = 0f;
            paused = true;
            pauseUI.SetActive(true);
        }
        
        //Tool UI is Up
        if (toolUIAnim.GetCurrentAnimatorStateInfo(0).IsName("UP_IDLE") && toolUIAnim.GetBool("IsSwitchingIn") == false)
        {
            ToolPickerIsUp();
        }

        //Nothing was moving, Tool Picker button pressed, open the picker
        if (Input.GetButtonDown("X") && !shovelAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE") && !canAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE") && !packetAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE") && !toolUIAnim.GetCurrentAnimatorStateInfo(0).IsName("UP_IDLE"))
        {
            toolPickerUp = true;
            toolUIAnim.SetBool("IsSwitchingIn", true);
        }

        //The game isn't paused and the tool picker isn't up ***TOOL ACTIONS HAPPEN HERE***
        if (!paused && !toolPickerUp)
        {
            //Ensures proper pause timing
            justPaused = false;
            
            //B is pressed, back out of UI or set tool to Select
            if (Input.GetButtonDown("B"))
            {
                BackButtonPressed();
            }

            //The Can is pouring, handle particles and interact with potential plant
            if(canAnim.GetCurrentAnimatorStateInfo(0).IsName("Can_Pour"))
            {
                Watering();
            }

            //--------------------------------------------------------------------
            //<Tool Updating, check if tool is out, then do action based on input>
            //--------------------------------------------------------------------

            //Selection tool is out
            if (toolList[curTool] == "Select")
            {
                //A is pressed and you are on a plant
                if (Input.GetButtonDown("A") && currentCol != null && currentCol.tag == "Plant")
                {
                    plantUI.SetActive(true);
                    selectedObj = currentCol.gameObject;
                    plantUI.transform.position = selectedObj.transform.position;
                }
                else if(Input.GetButtonDown("A") && currentCol != null && currentCol.tag == "Pinata")
                {
                    pinataUI.SetActive(true);
                    selectedObj = currentCol.gameObject;
                    pinataUI.transform.position = selectedObj.transform.position;
                    bool isResident = selectedObj.GetComponent<Pinata>().resident;
                    if (isResident)
                    {
                        residentTxt.text = "Resident: Yes";
                    }
                    else if(!isResident)
                    {
                        residentTxt.text = "Resident: No";
                    }
                }
                //A is pressed and you are on a pickup
                else if (Input.GetButtonDown("A") && currentCol != null && currentCol.tag == "Pickup")
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
            }
            //Shovel is out
            else if (toolList[curTool] == "Shovel")
            {
                //Dig Hole
            }
            //Watering Can is out
            else if (toolList[curTool] == "Watering Can")
            {
                if (Input.GetButtonDown("A") && !canAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE"))
                {
                    canAnim.SetBool("IsPouring", true);
                }
            }
            //Surface Packet is out
            else if (toolList[curTool] == "Surface Packet")
            {
                //A pressed, in garden, place the surface | **NEEDS OPTIMIZING**
                if (Input.GetButton("A") && InGarden)
                {
                    //play anim
                    PlaceTexture(curTexture);
                }

                //Y Pressed, change the surface
                if (Input.GetButtonDown("Y"))
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
            }
            //Seed Bag is out
            else if(toolList[curTool] == "Seed Bag")
            {
                Debug.Log(currentCol);
                if (Input.GetButtonDown("A") && (seedAnim.GetCurrentAnimatorStateInfo(0).IsName("SeedBag_Idle") && currentCol == null))
                {
                    seedAnim.SetBool("IsPlacing", true);
                    SeedPlant(selectedSeed);
                }
                else if(Input.GetButtonDown("Y"))
                {
                    //switch seed
                }
            }

            //Plant Profile is up, update the info
            if (plantUI.activeSelf && !pinataUI.activeSelf)
            {
                waterMask.fillAmount = selectedObj.GetComponent<Plant>().waterLevel/200;
            }
        }
    }
    
    //Currently fixed update is used to 
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

        if (!shovelAnim.GetCurrentAnimatorStateInfo(0).IsName("Shovel_Dig") && !seedAnim.GetCurrentAnimatorStateInfo(0).IsName("SeedBag_Place") && !toolPickerUp)
        {
            transform.Translate(desiredMoveDirection * speed * Time.deltaTime);
        }

        float h = 115 * Input.GetAxis("axisName") * Time.deltaTime;
        cam.transform.Rotate(0, h, 0);
        plantUI.transform.Rotate(0, h, 0);
        pinataUI.transform.Rotate(0, h, 0);
    }

    //Ran if X is pressed outside pause and no moving animation playing
    void ChangeTool(int cT, int nT) 
    {
        //Fade out of tool if not 0 (select)
        if(cT != 0)
        {
            if (cT == 1 && nT != 1)
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
            else if (cT == 4 && nT != 4)
            {
                toolFX.Play();
                seedAnim.SetBool("IsSwitchingOut", true);
            }
        }
        
        //Fade in tool if not 0
        if(nT != 0)
        {
            UIFlip.SetBool("IsFlipping", true);
            if (nT == 1 && cT != 1)
            {
                curTool = 1;
                topRightImg.sprite = noYTR;
                aText.text = "Dig Hole";
                yText.text = "";
                shovelAnim.SetBool("IsSwitchingIn", true);
            }
            else if (nT == 2 && cT != 2)
            {
                curTool = 2;
                topRightImg.sprite = noYTR;
                aText.text = "Pour Water";
                canAnim.SetBool("IsSwitchingIn", true);
            }
            else if (nT == 3 && cT != 3)
            {
                curTool = 3;
                topRightImg.sprite = fullTR;
                aText.text = "Pour Surface";
                yText.text = "Alt. Surface";
                packetAnim.SetBool("IsSwitchingIn", true);
            }
            else if (nT == 4 && cT != 4)
            {
                curTool = 4;
                topRightImg.sprite = fullTR;
                aText.text = "Plant Seed";
                yText.text = "Change Seed";
                seedAnim.SetBool("IsSwitchingIn", true);
            }
        }
        else if(nT == 0 && cT != 0)
        {
            curTool = 0;
            topRightImg.sprite = noATR;
            aText.text = "";
            yText.text = "";
            UIFlip.SetBool("IsFlipping", true);
        }

        toolPickerUp = false;
        toolUIAnim.SetBool("IsSwitchingOut", true);
    }

    //Place Texture accoriding to terrain settings, called if Surface Packet out and A pressed
    void PlaceTexture(string tx)
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

    //Check to see which texture the player is currently on top of
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

    //Player left the garden, update the cursor color
    public IEnumerator LeftGarden()
    {
        float ElapsedTime = 0.0f;
        float TotalTime = 0.25f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.gray, (ElapsedTime / TotalTime));
            yield return null;
        }
    }

    //Player entered the garden, update the cursor color
    public IEnumerator EnterGarden()
    {
        float ElapsedTime = 0.0f;
        float TotalTime = 0.25f;
        while (ElapsedTime < TotalTime)
        {
            ElapsedTime += Time.deltaTime;
            transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.gray, Color.red, (ElapsedTime / TotalTime));
            yield return null;
        }
    }
    //Candy is to be subtracted, update visuals gracefully
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
    //Plant died, make sure UI updates properly
    public void InformedDeath(GameObject died)
    {
        if (selectedObj == died)
        {
            plantUI.SetActive(false);
            pinataUI.SetActive(false);
            selectedObj = null;
        }
    }
    //The Tool Picker is up, allow interaction and tool selection
    private void ToolPickerIsUp()
    {
        bool recievedInput = false;
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            recievedInput = true;
            float x = -Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            aim_angle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
        }
        if (aim_angle > -40 && aim_angle < 40)
        {
            aim_angle = 0;
            selectedTool = 1;
        }
        else if (aim_angle < -40 && aim_angle > -140)
        {
            aim_angle = -90;
            selectedTool = 2;
        }
        else if (aim_angle < -140 || aim_angle > 140)
        {
            aim_angle = 180;
            selectedTool = 3;
        }
        else if (aim_angle < 140 && aim_angle > 40)
        {
            aim_angle = 90;
            selectedTool = 4;
        }
        if(recievedInput)
        {
            arrowUI.transform.rotation = Quaternion.AngleAxis(aim_angle, Vector3.forward);
        }

        if (Input.GetButtonDown("A"))
        {
            ChangeTool(curTool, selectedTool);
            toolUIAnim.SetBool("IsSwitchingOut", true);
        }
        else if (Input.GetButtonDown("B"))
        {
            ChangeTool(curTool, 0);
            toolUIAnim.SetBool("IsSwitchingOut", true);
        }
        return;
    }
    //B was pressed, close UI or make tool Select
    private void BackButtonPressed()
    {
        if(plantUI.activeSelf || pinataUI.activeSelf)
        {
            pinataUI.SetActive(false);
            plantUI.SetActive(false);
        }
        else if(!canAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE") && !seedAnim.GetCurrentAnimatorStateInfo(0).IsTag("MOVE"))
        {
            ChangeTool(curTool,0);
        }
        //back out of tool if not select and UI wasn't up
    }
    //Watering can is pouring, update the particles and check if over plant (event. pinata too)
    private void Watering()
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
            plantUI.SetActive(true);
            plantUI.transform.position = currentCol.transform.position;
            selectedObj = currentCol.gameObject;
            //itemNick.text = selectedObj.GetComponent<Plant>().pNick;
            selectedObj.GetComponent<Plant>().waterLevel += Time.deltaTime * waterStrength;

        }
    }

    private void SeedPlant(string seedToPlace)
    {
        if(TextureOnTopOf() != "Sand")
        {
            GameObject plant = Resources.Load<GameObject>("Prefabs/" + seedToPlace);
            int cost = plant.GetComponent<Plant>().price;
            if (GM.candyCount >= cost)
            {
                shovelAnim.SetBool("IsDigging", true);
                var euler = transform.eulerAngles;
                euler.y = Random.Range(0.0f, 360.0f);
                GameObject justIn = Instantiate(plant, new Vector3(transform.GetChild(0).position.x, -.02f, transform.GetChild(0).position.z), Quaternion.identity);
                justIn.transform.Rotate(euler);
                justIn.GetComponent<Plant>().pNick += " (" + plantCounter + ")";
                plantCounter++;
                GM.addPlant(justIn);
                StartCoroutine(UpdateCandy(GM.candyCount, cost));
                GM.candyCount -= cost;
                candyText.text = GM.candyCount.ToString("N0");
            }
        }
        else
        {
            //on sand, inform user
        }
    }
    //Below functions are standard functions that involve colliders and quitting the game
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Plant")
        {
            //Debug.Log("Entered: " +collision.gameObject.name);
            if (toolList[curTool] == "Select")
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
        else if (collision.gameObject.tag == "Pinata" && toolList[curTool] == "Select")
        {
            topRightImg.sprite = noYTR;
            aText.text = "Select Pinata";
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
            if (toolList[curTool] == "Select")
            {
                topRightImg.sprite = noATR;
                aText.text = "";
            }
        }
        else if (collision.gameObject.tag == "Pinata")
        {
            currentCol = null;
            if (toolList[curTool] == "Select")
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
    void OnApplicationQuit()
    {
        //Update the terrain with original alpha map (why are terrains like this?)
        tur.terrainData.SetAlphamaps(0, 0, originalTerrain);
    }
}
