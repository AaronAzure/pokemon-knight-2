using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;
// using Com.LuisPedroFonseca.ProCamera2D;

public class PlayerControls : MonoBehaviour
{
    private Rewired.Player player;
    public int playerID = 0;
    [SerializeField] private GameObject rewiredInputSystem;
    [SerializeField] private GameObject lookTarget;
    private int gameNumber;
    

    [Space] [Header("Ui")]
    public Image hpEffectImg;
    public Image hpImg;
    public TextMeshProUGUI hpTxt;
    [Space]
    public Image expEffectImg;  // white
    public Image expImg;        // blue
    [Space]
    public TextMeshProUGUI lvText;
    [SerializeField] private float effectSpeed = 0.005f;
    public Animator transitionAnim;
    public Sprite emptySprite;
    
    [Space] [SerializeField] private GameObject mapMenu;
    [SerializeField] private RectTransform mapMenuRect;
    public Dictionary<string, SceneMap> sceneMaps;
    public SceneMap lastScene;
    public Vector2 mapOffset;


    [Space] 
    public Image pokeballY1;
    public Image pokeballX1;
    public Image pokeballA1;
    public Image pokeballY2;
    public Image pokeballX2;
    public Image pokeballA2;

    [Space] public GameObject gaugesHolder;
    public Image gaugeImg;
    public GameObject gaugeButton;
    public GameObject gaugeIndicator;
    public GameObject gaugeGlow;
    public int spMax=100;
    public int spReq=100;
    public int sp;

    [Space] 
    [Tooltip("In game = cooldown")] public Image[] partyPokemonsUI;

    [Space] 
    [Tooltip("Paused settings = equipped")] public BoxPokemonButton[] pokemonInTeamBenchSettings;

    [Space] public GameObject locationAnimObj;
    public TextMeshProUGUI locationName;

    
    [Space] [Header("Pokemon (Allies)")]
    [SerializeField] private Transform spawnPos;    // Place to Summon Pokemon
    [Space][SerializeField] private GameObject doubleJumpObj;   //Butterfree

    [Space] public bool isSet1=true; // true = first set, false = second set
    public bool canSwitchSets=true;
    
    [Space] [HideInInspector] public Ally[] allies;

    [HideInInspector] public bool butterfreeOut;
    [HideInInspector] public int butterfreeSlot = -1;
    private bool doubleJumpBeforeAtk;


    [Header("Pre-init Pokemon for PlayerPrefs")]
    [SerializeField] private AllyTeamUI bulbasaur;
    [Space] [SerializeField] private AllyTeamUI charmander;
    [Space] [SerializeField] private AllyTeamUI squirtle;
    [Space] [SerializeField] private AllyTeamUI pidgey;
    [Space] [SerializeField] private AllyTeamUI butterfree;
    [Space] [SerializeField] private AllyTeamUI oddish;
    [Space] [SerializeField] private AllyTeamUI tangela;



    [Space] [Header("Settings UI")]

    [SerializeField] private ReturnToTitleScreen titleScreenObj;
    [Space] [SerializeField] private Animator settings;
    [Space] [SerializeField] private Animator equimentSettings;
    private bool onPokemonTab=true;
    [Space] [SerializeField] private Animator pokemonSets;

    [Space] [SerializeField] private Canvas pokemonSet1;
    [SerializeField] private Canvas pokemonSet2;
    
    [Space] [SerializeField] private BoxPokemonButton[] boxPokemonsToActivate;
    [Space] [SerializeField] private ItemUi[] itemsToActivate;

    [Space] [SerializeField] private Button[] partyPokemonButtons;
    [SerializeField] private Button[] boxPokemonButtons;

    [Space] [SerializeField] private Button partyPokemonFirstSelected;
    [SerializeField] private Button boxPokemonFirstSelected;
    [Space] [SerializeField] private Button partyPokemonLastSelected;
    [SerializeField] private Button boxPokemonLastSelected;
    [Space] [SerializeField] private string oldButtonSymbol;
    private bool isClosing=false;
    private Ally newAllyToEquip;
    private Sprite newAllySpriteToEquip;

    
    [Space]
    [Header("Damage Related")]
    [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Material flashMat;
    [SerializeField] private Material origMat;
    [SerializeField] private Animator damageIndicatorAnim;


    [Space]
    [Header("Player data")]
    public int maxHp;
    public int hp;  // current hp
    public int extraHp=0;
    private int lastHp;
    public int lv=1;
    [Space] [SerializeField] private int expNeeded=100;
    [SerializeField] private int exp;  // current exp
    [SerializeField] private int expJustGained;
    [SerializeField] private TextMeshProUGUI expGainText;
    [Space] [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private GameObject levelUpObj;
    [SerializeField] private AudioSource levelUpSound;
    [SerializeField] private GameObject playerUi;
    [Space] [SerializeField] private List<string> visitedScenes;
    [Space] [SerializeField] private List<string> roomsBeaten;
    [Space] [SerializeField] private List<string> pokemonsCaught;
    [Space] [SerializeField] private List<string> itemsObtained;
    [Space] [SerializeField] private List<string> berriesCollected;
    [Space] [SerializeField] private List<string> spareKeychainCollected;
    [Space] [SerializeField] private List<string> enemyDefeated = new List<string>();
    public Item currentItem;
    public Berry currentBerry;
    public SpareKeychain currentKeychain;
    public int nBerries;


    [Space] [Header("Platformer Mechanics")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Collider2D col;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float dashSpeed = 50;
    // [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private float jumpHeight = 10;
    [SerializeField] private float jumpTimer = 0.35f;
    private float jumpTimerCounter = 0;
    private Sprite origFace;


    [Space]
    [SerializeField] private Transform feetPos; // To detect ground
    [SerializeField] private float feetRadius;
    [SerializeField] private Vector2 feetBox;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWater;
    [SerializeField] public bool grounded = true;
    [HideInInspector] public bool jumping = false;
    [HideInInspector] public bool crouching = false;
    [HideInInspector] public bool ledgeGrabbing = false;
    [SerializeField] private Vector2 wallDetectBox;
    private bool receivingKnockback;
    private int dashes = 1;
    private bool dashing;
    private bool movingToDifferentScene;
    private bool dodging;
    private bool canDodge = true;
    [SerializeField] private bool isInvincible;
    [SerializeField] private GameObject glint;
    private bool dodgingThruScene;
    private float dodgeSpeed = 7.5f;

    [SerializeField] private bool inWater;
    private bool aboveWaterCheck, inWaterCheck;
    [SerializeField] private Transform aboveWaterCheckOffset, inWaterCheckOffset;
    private bool greenBox, redBox;
    private bool wallCheck, ledgeCheck;
    [SerializeField] private Transform wallCheckOffset, ledgeCheckOffset;
    [SerializeField] private float checkDist=0.3f;
    private float origGrav;


    private bool canPressButtonNorth = true;   // (X)
    private bool canPressButtonWest = true;    // (Y)
    private bool canPressButtonEast = true;    // (A)
    private bool canPressButtonNorth2 = true;   // (X)
    private bool canPressButtonWest2 = true;    // (Y)
    private bool canPressButtonEast2 = true;    // (A)
    private int nPokemonOut;
    public int maxPokemonOut = 1;
    [Space] [SerializeField] private GameObject holder;
    [SerializeField] private Animator anim;
    public bool inCutscene;
    private bool returningToTitle;


    [Space] [Header("Moomoo milk")]
    public int nMoomooMilkLeft = 1;
    public int moomooMilkRecovery = 50;
    public int nMoomooMilkUpgrade=0;
    public Animator[] moomooUi;
    public MoomooMilkUi[] extraMoomooUis;
    public bool drinking;
    public GameObject healEffect;
    public AudioSource healSound;

    [Space] public bool canRest;
    public bool resting;
    public Transform restBench;
    [Space] public bool canEnter;
    public string newSceneName;
    public Vector2 newScenePos;
    [Tooltip("true = moving left\nfalse = moving right")] public bool moveLeftFromDoor; 


    [Space][Header("Items")] 
    public EquipmentUi equipmentUi;
    public Button[] itemButtons;
    public Image[] equippedItems;
    public List<string> equippedItemNames;
    [SerializeField] private Animator weightAnim;
    [SerializeField] private AudioSource itemFoundlSound;
    public int nEquipped;
    public int currentWeight=0; 
    public int maxWeight=3; 
    public int extraWeight=0; 
    public bool canNavigate=true;
    [Space] public bool quickCharm;
    public bool chuggerCharm;
    public bool crisisCharm;
    public bool graciousHeartCharm;
    public bool milkAddictCharm;
    [SerializeField] private GameObject furyYellowObj;
    [SerializeField] private GameObject furyRedObj;
    public bool dualCharm;
    public TextMeshProUGUI weightText;
    

    //* Powerups
    [Header("Powerups")]
    [SerializeField] private Animator doubleJumpScreen;
    public bool canDoubleJump;
    private int nExtraJumps = 1;
    private int nExtraJumpsLeft = 1;
    [SerializeField] private Transform doubleJumpSpawnPos;
    [Space] public bool canDash;
    public bool canSwim;
    public bool canUseUlt;
    private MusicManager musicManager;
    

    [Header("Status Conditions")]
    public bool isSleeping;
    private enum HpEffect { none, heal, lost };
    private HpEffect healthStatus = HpEffect.none;
    [SerializeField] private ParticleSystem sleepingEffect;
    [SerializeField] private SpriteRenderer face;
    [SerializeField] private Sprite sleepFace;

    
    [Header("CHEATS")]
    [SerializeField] private float dmgMultiplier = 1;
    [SerializeField] private float expMultiplier = 1;
    [SerializeField] private bool noCoolDown;
    [SerializeField] private bool infiniteGauge;
    public bool extraRange;

    private static PlayerControls playerInstance;   // There can only be one

    
         
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        if (playerInstance == null)
            playerInstance = this;
        else 
            Destroy(this.gameObject);
        
        if (body == null)
            body = GetComponent<Rigidbody2D>();
        
    }

    void Start()
    {
        
        equippedItemNames = new List<string>();
        player = ReInput.players.GetPlayer(playerID);

        gameNumber = PlayerPrefsElite.GetInt("gameNumber");
        origFace = face.sprite;

        PlayerPrefsElite.SetStringArray("enemyDefeated", new string[0]);

        // Last save
        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump" + gameNumber))
            canDoubleJump = PlayerPrefsElite.GetBoolean("canDoubleJump" + gameNumber);

        if (PlayerPrefsElite.VerifyBoolean("canUseUlt" + gameNumber))
            canUseUlt = PlayerPrefsElite.GetBoolean("canUseUlt" + gameNumber);
        if (canUseUlt)
            gaugesHolder.SetActive(true);
        else
            gaugesHolder.SetActive(false);
        gaugeImg.fillAmount = 0;
        gaugeGlow.SetActive(false);


        if (PlayerPrefsElite.VerifyBoolean("canDash" + gameNumber))
            canDash = PlayerPrefsElite.GetBoolean("canDash" + gameNumber);
            
        if (PlayerPrefsElite.VerifyInt("playerLevel" + gameNumber))
        {
            lv = PlayerPrefsElite.GetInt("playerLevel" + gameNumber);
            CalculateMaxHp();
            if (lvText != null)
                 lvText.text = "Lv. " + lv;
        }
        expNeeded = (int) (100 * Mathf.Pow(1.2f, lv - 1));
        if (PlayerPrefsElite.VerifyInt("playerExp" + gameNumber))
            exp = PlayerPrefsElite.GetInt("playerExp" + gameNumber);

        hp = maxHp;

        origGrav = body.gravityScale;
        col.enabled = false;
        body.gravityScale = 0;
        
        string sceneName = SceneManager.GetActiveScene().name;
        if (PlayerPrefsElite.VerifyString("checkpointScene" + gameNumber))
        {
            StartCoroutine( CannotChangeSceneAgain() ); inCutscene = true;
            sceneName = PlayerPrefsElite.GetString("checkpointScene" + gameNumber);
            SceneManager.LoadScene(sceneName);
            // Debug.Log("<color=#0EB8BF>"+PlayerPrefsElite.GetString("checkpointScene" + gameNumber)+"</color>");
        }
        else 
        {
            PlayerPrefsElite.SetString("checkpointScene" + gameNumber, sceneName);
            body.gravityScale = 3;
            inCutscene = false;
            col.enabled = true;
        }

        if (sceneMaps == null)
            sceneMaps = new Dictionary<string, SceneMap>();
        if (sceneMaps.ContainsKey(sceneName))
            lastScene = sceneMaps[sceneName];
        else
            Debug.LogError("CANNOT FIND MATCHING MAP NAME : for " + sceneName);

        if (PlayerPrefsElite.VerifyVector3("checkpointPos" + gameNumber))
            this.transform.position = PlayerPrefsElite.GetVector3("checkpointPos" + gameNumber);
        else 
            PlayerPrefsElite.SetVector3("checkpointPos" + gameNumber, this.transform.position + new Vector3(0,0.25f));

        if (PlayerPrefsElite.VerifyArray("visitedScenes" + gameNumber))
            visitedScenes = new List<string>(PlayerPrefsElite.GetStringArray("visitedScenes" + gameNumber));
        else 
        {
            visitedScenes = new List<string>();
            PlayerPrefsElite.SetStringArray("visitedScenes" + gameNumber, new string[0]);
        }
        if (!visitedScenes.Contains(sceneName))
        {
            visitedScenes.Add(sceneName);
            PlayerPrefsElite.SetStringArray("visitedScenes" + gameNumber, visitedScenes.ToArray());
            if (sceneMaps.ContainsKey(sceneName))
                sceneMaps[sceneName].Visited();
        }
        if (sceneMaps.ContainsKey(sceneName))
        {
            sceneMaps[sceneName].EnterScene();
        }


        if (PlayerPrefsElite.VerifyArray("roomsBeaten" + gameNumber))
            roomsBeaten = new List<string>(PlayerPrefsElite.GetStringArray("roomsBeaten" + gameNumber));
        else 
        {
            roomsBeaten = new List<string>();
            PlayerPrefsElite.SetStringArray("roomsBeaten" + gameNumber, new string[0]);
        }
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught" + gameNumber))
            pokemonsCaught = new List<string>( PlayerPrefsElite.GetStringArray("pokemonsCaught" + gameNumber) );
        
        // MOOMOO MILK POTENCY
        if (PlayerPrefsElite.VerifyArray("berriesCollected" + gameNumber))
        {
            berriesCollected = new List<string>( PlayerPrefsElite.GetStringArray("berriesCollected" + gameNumber) );
            var berriesSet = new HashSet<string>(berriesCollected);
            if (berriesSet.Contains(""))
                berriesSet.Remove("");
            nBerries = berriesSet.Count;
            Debug.Log("<color=green>Collected " + nBerries + " berries</color>");
        }
        else 
        {
            berriesCollected = new List<string>();
            PlayerPrefsElite.SetStringArray("berriesCollected" + gameNumber, berriesCollected.ToArray());
        }
        
        if (PlayerPrefsElite.VerifyArray("spareKeychain" + gameNumber))
        {
            spareKeychainCollected = new List<string>( PlayerPrefsElite.GetStringArray("spareKeychain" + gameNumber) );
            var spareKeychainSet = new HashSet<string>(spareKeychainCollected);
            if (spareKeychainSet.Contains(""))
                spareKeychainSet.Remove("");
            extraWeight = spareKeychainSet.Count;
        }
        else 
        {
            spareKeychainCollected = new List<string>();
            PlayerPrefsElite.SetStringArray("spareKeychain" + gameNumber, spareKeychainCollected.ToArray());
        }

        
        if (transitionAnim != null)
        {
            transitionAnim.gameObject.SetActive(true);
            transitionAnim.SetTrigger("fromBlack");
        }

        // Hide settings UI
        if (settings != null)
            settings.gameObject.SetActive(false);
        if (equimentSettings != null)
            equimentSettings.gameObject.SetActive(false);
        
        // Last Pokemon team
        if (PlayerPrefsElite.VerifyArray("buttonAllocatedPokemons" + gameNumber))
        {
            allies = new Ally[6];

            string[] buttonAllocatedPokemons = PlayerPrefsElite.GetStringArray("buttonAllocatedPokemons" + gameNumber);

            for (int i=0 ; i<buttonAllocatedPokemons.Length ; i++)
            {
                // Debug.Log(buttonAllocatedPokemons[i].ToLower());
                switch (buttonAllocatedPokemons[i].ToLower())
                {
                    case "bulbasaur":
                        allies[i] = bulbasaur.summonable;
                        pokemonInTeamBenchSettings[i].img.sprite = bulbasaur.sprite;
                        pokemonInTeamBenchSettings[i].ally = bulbasaur.summonable;
                        partyPokemonsUI[i].sprite = bulbasaur.sprite;
                        break;
                    case "charmander":
                        allies[i] = charmander.summonable;
                        pokemonInTeamBenchSettings[i].img.sprite = charmander.sprite;
                        pokemonInTeamBenchSettings[i].ally = charmander.summonable;
                        partyPokemonsUI[i].sprite = charmander.sprite;
                        break;
                    case "squirtle":
                        allies[i] = squirtle.summonable;
                        pokemonInTeamBenchSettings[i].img.sprite = squirtle.sprite;
                        pokemonInTeamBenchSettings[i].ally = squirtle.summonable;
                        partyPokemonsUI[i].sprite = squirtle.sprite;
                        break;
                    case "pidgey":
                        allies[i] = pidgey.summonable;
                        pokemonInTeamBenchSettings[i].img.sprite = pidgey.sprite;
                        pokemonInTeamBenchSettings[i].ally = pidgey.summonable;
                        partyPokemonsUI[i].sprite = pidgey.sprite;
                        break;
                    case "butterfree":
                        allies[i] = butterfree.summonable;
                        pokemonInTeamBenchSettings[i].img.sprite = butterfree.sprite;
                        pokemonInTeamBenchSettings[i].ally = butterfree.summonable;
                        partyPokemonsUI[i].sprite = butterfree.sprite;
                        break;
                    case "oddish":
                        allies[i] = oddish.summonable;
                        pokemonInTeamBenchSettings[i].img.sprite = oddish.sprite;
                        pokemonInTeamBenchSettings[i].ally = oddish.summonable;
                        partyPokemonsUI[i].sprite = oddish.sprite;
                        break;
                    case "tangela":
                        allies[i] = tangela.summonable;
                        pokemonInTeamBenchSettings[i].img.sprite = tangela.sprite;
                        pokemonInTeamBenchSettings[i].ally = tangela.summonable;
                        partyPokemonsUI[i].sprite = tangela.sprite;
                        break;
                    case "":
                        pokemonInTeamBenchSettings[i].img.sprite = emptySprite;
                        pokemonInTeamBenchSettings[i].ally = null;
                        partyPokemonsUI[i].sprite = emptySprite;
                        break;
                    default:
                        Debug.LogError(buttonAllocatedPokemons[i] + " has not been added", this.gameObject);
                        break;
                }
            }
            SavePokemonTeam();
        }
        else
        {
            allies = new Ally[6];
            //* Default pokemon team
            allies[0] = bulbasaur.summonable;
            pokemonInTeamBenchSettings[0].ally = bulbasaur.summonable;
            partyPokemonsUI[0].sprite = bulbasaur.sprite;
            allies[1] = charmander.summonable;
            pokemonInTeamBenchSettings[1].ally = charmander.summonable;
            partyPokemonsUI[1].sprite = charmander.sprite;
            allies[2] = squirtle.summonable;
            pokemonInTeamBenchSettings[2].ally = squirtle.summonable;
            partyPokemonsUI[2].sprite = squirtle.sprite;
            allies[3] = null;
            partyPokemonsUI[3].sprite = emptySprite;
            allies[4] = null;
            partyPokemonsUI[4].sprite = emptySprite;
            allies[5] = null;
            partyPokemonsUI[5].sprite = emptySprite;
            SavePokemonTeam();
        }
    

        if (PlayerPrefsElite.VerifyArray("equippedItems" + gameNumber))
        {
            equippedItemNames = new List<string>( PlayerPrefsElite.GetStringArray("equippedItems" + gameNumber) );
            HashSet<string> set = new HashSet<string>(equippedItemNames);
            if (set.Contains(""))
                set.Remove("");
            HashSet<ItemUi> itemSet = new HashSet<ItemUi>(itemsToActivate);
            foreach (ItemUi item in itemSet)
                if (set.Contains(item.itemName))
                    item.TOGGLE_ITEM();
            SaveEquippedItems();
        }
        else
        {
            Debug.Log("NEW GAME items");
            PlayerPrefsElite.SetStringArray("equippedItems" + gameNumber, equippedItemNames.ToArray());
            SaveEquippedItems();
        }

        
        CheckEquippablePokemon();
        CheckObtainedItems();

        string sceneFirstWord = PlayerPrefsElite.GetString("checkpointScene" + gameNumber).Split(' ')[0];
        PlayerPrefsElite.SetString("currentArea" + gameNumber, sceneFirstWord);
        
        if (musicManager == null && GameObject.Find("Music Manager") != null)
        {
            musicManager = GameObject.Find("Music Manager").GetComponent<MusicManager>();
            StartingMusic(sceneFirstWord);
            if (titleScreenObj != null)
                titleScreenObj.musicManager = this.musicManager;
        }

        // foreach(string sm in sceneMaps.Keys)
        //     Debug.Log(sm, this.gameObject);

        mapMenu.SetActive(false);
        weightText.text = currentWeight + "/" + (maxWeight + extraWeight);
    }
    IEnumerator CannotChangeSceneAgain()
    {
        inCutscene = true;
        yield return new WaitForSeconds(0.2f);
        body.gravityScale = 3;
        inCutscene = false;
        col.enabled = true;
    }
    void Update()
    {
        if (player.GetButtonDown("START") && !settings.gameObject.activeSelf && 
            !equimentSettings.gameObject.activeSelf && !returningToTitle)
        {
            Time.timeScale = 0;
            inCutscene = true;
            canNavigate = true;
            isClosing = false;
            
            // Exit MENU
            if (!resting)
                settings.gameObject.SetActive(true);
            // Rest MENU
            else
            {
                weightText.text = currentWeight + "/" + (maxWeight + extraWeight);
                equimentSettings.gameObject.SetActive(true);
                if (equipmentUi.onPokemonTab)
                    boxPokemonFirstSelected.Select();
                else
                    SelectDefaultItem();
                foreach (Button button in partyPokemonButtons)
                    button.interactable = false;
                foreach (Button button in boxPokemonButtons)
                    button.interactable = true;
            }
        }
        //* EXTRA NAVIGATION IN UI SETTINGS MENU
        else if ((settings.gameObject.activeSelf || equimentSettings.gameObject.activeSelf))
        {   
            // EQUIMENT MENU
            if (resting)
            {
                // EXIT MENU
                if (player.GetButtonDown("START") || player.GetButtonDown("B"))
                    EXIT_EQUIPMENT_MENU();
                else if (player.GetButtonDown("L") && canNavigate)
                {
                    equipmentUi.ChangeTabs(false);
                    boxPokemonFirstSelected.Select();
                    foreach (Button button in partyPokemonButtons)
                        button.interactable = false;
                    foreach (Button button in boxPokemonButtons)
                        button.interactable = true;
                }
                // Items
                else if (player.GetButtonDown("R") && canNavigate)
                {
                    equipmentUi.ChangeTabs(true);
                    SelectDefaultItem();
                }
            }
            // SETTINGS MENU
            else
            {
                // EXIT MENU
                if (player.GetButtonDown("START") || player.GetButtonDown("B"))
                    EXIT_PAUSE_MENU();

            }

        }
        //* Paused
        if (settings.gameObject.activeSelf || equimentSettings.gameObject.activeSelf) {}
        
        else if (ledgeGrabbing)
        {

        }
        //* STOP MOOMOO MILK
        else if (drinking)
        {
            if (player.GetButtonUp("L"))
            {
                anim.speed = 1;
                body.velocity = new Vector2(0, body.velocity.y);
                anim.SetBool("isDrinking", false);
                drinking = false;
            }
        }
        //* DRINKING MOOMOO MILK
        else if (nMoomooMilkLeft > 0 && hp != maxHp && hp > 0 && player.GetButtonDown("L"))
        {
            DrinkingMoomooMilk();
        }
        //* RESTING
        else if (resting)
        {
            if (player.GetButtonDown("B"))
                LeaveBench();
        }
        
        else if (dodging)
        {
            if (holder.transform.eulerAngles.y < 180)   // right
                body.velocity = new Vector2(dodgeSpeed, body.velocity.y);
            else
                body.velocity = new Vector2(-dodgeSpeed, body.velocity.y);
        }
        //* Walking, Dashing, Summoning, jumping, Interacting
        else if (hp > 0 && !inCutscene && !dodging && !isSleeping)
        {
            if (!crouching && player.GetAxis("Move Vertical") <= -0.75f && grounded)
            {
                body.velocity = Vector2.zero;
                crouching = true;
                anim.SetBool("isCrouching", crouching);
                anim.speed = 1;
            }
            else if (crouching && player.GetAxis("Move Vertical") > -0.75f || !grounded)
            {
                crouching = false;
                anim.SetBool("isCrouching", crouching);
            }

            if (inWater && CheckAtWaterSurface())
                nExtraJumpsLeft = nExtraJumps;

            if (!ledgeGrabbing)
                ledgeGrabbing = CheckLedgeGrab();

            if (canRest && Interact())
                RestOnBench();

            if (canEnter && Interact())
                StartCoroutine( EnteringDoor() );

            if (player.GetButtonDown("R"))
                SwitchPokemonSet();

            if (currentBerry != null && Interact())
            {
                body.velocity = Vector2.zero;
                inCutscene = true;
                currentBerry.PickupBerry();
                anim.speed = 1;
                anim.SetTrigger("pickup");
                currentBerry = null;
                if (itemFoundlSound != null) 
                    itemFoundlSound.Play();
            }
            else if (currentKeychain != null && Interact())
            {
                if (currentKeychain.player == null)
                    currentKeychain.player = this;
                body.velocity = Vector2.zero;
                inCutscene = true;
                currentKeychain.PickupSpareKeychain();
                anim.speed = 1;
                anim.SetTrigger("pickup");
                currentKeychain = null;
                if (itemFoundlSound != null) 
                    itemFoundlSound.Play();
            }
            else if (currentItem != null && Interact())
            {
                body.velocity = Vector2.zero;
                inCutscene = true;
                currentItem.PickupItem();
                anim.speed = 1;
                anim.SetTrigger("pickup");
                currentItem = null;
                if (itemFoundlSound != null) 
                    itemFoundlSound.Play();
            }
            else if (grounded && canDodge && player.GetButtonDown("ZR") && !canRest && !canEnter && currentItem == null)
            {
                canDodge = false;
                StartCoroutine( Dodge() );
            }


            grounded = Physics2D.OverlapBox(feetPos.position, feetBox, 0, whatIsGround);
            // Touched floor
            if (body.velocity.y == 0 && grounded)
            {
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                nExtraJumpsLeft = nExtraJumps;
            }

            if (player.GetButtonDown("MINUS"))
                ToggleMap();

            if (mapMenu.activeSelf)
            {
                MoveMap();
                if (player.GetButtonDown("Right Stick"))
                    LockToCurrentPos();
            }
            
            if (body.velocity.y < -0.1f && !grounded)
            {
                anim.SetTrigger("fall");
                anim.SetBool("isFalling", true);
            }

            //* Walking & jumping
            if (!dashing)
            {

                if (dashes > 0)
                {
                    if (grounded && player.GetButtonDown("B"))
                        Jump();
                    //* Double Jump (mid air jump)
                    if (canDoubleJump)
                    {
                        if (!inWater && !butterfreeOut && nExtraJumpsLeft > 0 && !grounded && player.GetButtonDown("B"))
                        {
                            nExtraJumpsLeft--;
                            MidairJump();
                        }
                        else if (inWater && !butterfreeOut && nExtraJumpsLeft > 0 && CheckAtWaterSurface()
                            && player.GetButtonDown("B"))
                        {
                            nExtraJumpsLeft--;
                            MidairJump();
                        }
                    }

                    if (player.GetButton("B") && jumping)
                    {
                        if (jumpTimerCounter > 0)
                        {
                            body.velocity = new Vector2(body.velocity.x, jumpHeight);
                            jumpTimerCounter -= Time.deltaTime;
                        } 
                        else
                        {
                            jumping = false;
                        }
                    }
                    if (player.GetButtonUp("B"))
                        jumping = false;
                }
                if (canDash && dashes > 0 && player.GetButtonDown("ZR"))
                    Dash();
            }
            // * Dashing
            else
            {
                if (holder.transform.eulerAngles.y < 180)
                    body.velocity = Vector2.right * dashSpeed;
                else
                    body.velocity = Vector2.left * dashSpeed;
            }


            //* Summon Pokemon
            if (nPokemonOut < maxPokemonOut)
            {
                if (isSet1)
                {
                    if      (canPressButtonWest && player.GetButtonDown("Y"))
                    {
                        SummonPokemon(0, "Y");
                    }
                    else if (canPressButtonNorth && player.GetButtonDown("X"))
                    {
                        SummonPokemon(1, "X");
                    }
                    else if (canPressButtonEast && player.GetButtonDown("A"))
                    {
                        SummonPokemon(2, "A");
                    }
                }
                else
                {
                    if      (canPressButtonWest2 && player.GetButtonDown("Y"))
                    {
                        SummonPokemon(3, "Y2");
                    }
                    else if (canPressButtonNorth2 && player.GetButtonDown("X"))
                    {
                        SummonPokemon(4, "X2");
                    }
                    else if (canPressButtonEast2 && player.GetButtonDown("A"))
                    {
                        SummonPokemon(5, "A2");
                    }
                }
            }
        }
        
        //* Powerup or new pokemon cutscene
        else if (inCutscene && doubleJumpScreen.gameObject.activeSelf)
        {
            if (PressedStandardButton())
                doubleJumpScreen.SetTrigger("confirm");
        }
    }

    private void SummonPokemon(int slot, string button)
    {
        if (allies[ slot ] != null && (!inWater || allies[ slot ].aquatic))
        {
            if (allies[ slot ] != butterfree.summonable || allies[ slot ] == butterfree.summonable && !butterfreeOut)
            {
                if (allies[ slot ] == butterfree.summonable)
                    butterfreeOut = true;

                if (!noCoolDown) nPokemonOut++;
                var pokemon = Instantiate(allies[ slot ], spawnPos.position, allies[ slot ].transform.rotation);
                pokemon.atkDmg = (int) (dmgMultiplier * pokemon.atkDmg);
                pokemon.body.velocity = this.body.velocity;
                pokemon.trainer = this;
                pokemon.button = button;
                    
                if (canUseUlt && (sp >= spReq || infiniteGauge) && player.GetButton("ZL"))
                {
                    pokemon.useUlt = true;
                    sp -= spReq;
                    gaugeGlow.SetActive(false);
                }

                PokemonSummonedIndicator(button);
                
                //* Looking left
                if (holder.transform.eulerAngles.y > 0)
                    pokemon.transform.eulerAngles = new Vector3(0,-180);
            }
        }
    }

    private void SwitchPokemonSet()
    {
        if (canSwitchSets)
        {
            canSwitchSets = false;
            pokemonSets.SetTrigger("switch");
            isSet1 = !isSet1;
        }
    }
    public void CanSwitchSetsAgain()
    {
        canSwitchSets = true;
    }

    void FixedUpdate()
    {
        //* Paused
        if (settings.gameObject.activeSelf) {}
        else if (ledgeGrabbing) {}
        else if (resting) {}
        else if (hp > 0 && !inCutscene && !dodging && !drinking && !isSleeping)
        {
            float xValue = player.GetAxis("Move Horizontal");
            if (inWater && canSwim)
            {
                float yValue = player.GetAxis("Move Vertical");

                body.velocity = new Vector2(xValue, yValue) * moveSpeed;
            }
            else if (!crouching)
            {
                Walk(xValue);

                // Walking animation
                if (Mathf.Abs(xValue) > 0)
                {
                    anim.SetBool("isWalking", true);
                    anim.speed = Mathf.Min(Mathf.Abs(xValue) * moveSpeed, 3);
                }
                // Not moving (idle)
                else
                {
                    anim.SetBool("isWalking", false);
                    anim.speed = 1;
                }
            }

            float lookX = player.GetAxis("Look Horizontal");
            float lookY = player.GetAxis("Look Vertical");
            // Debug.Log("right stick = (" + lookX + ", " + lookY + ")");
            // lookTarget.transform.localPosition = new Vector3(lookX, lookY);
            lookTarget.transform.position = holder.transform.position + new Vector3(3*lookX, 3*lookY);

            //* Flip character
            playerDirection(xValue);
        }
    }
    private void LateUpdate() 
    {
        //* Gauge meter
        if (canUseUlt && gaugeImg != null)
        {
            float temp = ((float)sp /(float) spMax);
            // gaugeImg.fillAmount = ((float)sp /(float) spMax);
            if (gaugeImg.fillAmount < temp)
                gaugeImg.fillAmount += effectSpeed;
                if (gaugeImg.fillAmount > temp)
                    gaugeImg.fillAmount = temp;
            else if (gaugeImg.fillAmount > temp)
                gaugeImg.fillAmount -= effectSpeed;
                if (gaugeImg.fillAmount < temp)
                    gaugeImg.fillAmount = temp;

            if (gaugeGlow != null)
                if (gaugeImg.fillAmount < 1)
                {
                    gaugeGlow.SetActive(false);
                    gaugeButton.SetActive(false);
                }
                else
                {
                    gaugeGlow.SetActive(true);
                    gaugeButton.SetActive(true);
                    gaugeIndicator.SetActive(false);
                    gaugeIndicator.SetActive(true);
                }
        }

        //* Exp
        if (expImg != null)
        {
            expEffectImg.fillAmount = ((float)exp /(float) expNeeded);
            if (expImg.fillAmount < expEffectImg.fillAmount)
                    expImg.fillAmount += effectSpeed;
            else 
                expImg.fillAmount = expEffectImg.fillAmount;
            if (expImg.fillAmount >= 1)
                LevelUp();
        }
        
        if (hpTxt != null)
        {
            hpTxt.text = hp + "/" + maxHp;
            if (hp < 0)
                hpTxt.text = "0/" + maxHp;
        }
        
        //* Hp
        if (hpImg != null && hpEffectImg != null)
        {
            float currentHpPercent = ((float) Mathf.Max(0,hp) /(float) maxHp);

            if      (hpImg.fillAmount < currentHpPercent)    //* RECOVERED HEALTH
            {
                hpEffectImg.fillAmount = currentHpPercent;
                healthStatus = HpEffect.heal;
            }
            else if (hpImg.fillAmount > currentHpPercent)    //* LOST HEALTH
            {
                hpImg.fillAmount = currentHpPercent;
                healthStatus = HpEffect.lost;
            }
            
            //* Hp gain Effect
            if (healthStatus == HpEffect.heal)
            {
                if (hpImg.fillAmount < hpEffectImg.fillAmount)
                    if (hp > 0)
                        hpImg.fillAmount += effectSpeed;
                    else
                        hpImg.fillAmount += (effectSpeed/2);
                else 
                {
                    hpImg.fillAmount = hpEffectImg.fillAmount;
                    healthStatus = HpEffect.none;
                }
            }
            //* Hp lost 
            else if (healthStatus == HpEffect.lost)
            {
                if (hpEffectImg.fillAmount > hpImg.fillAmount)
                    if (hp > 0)
                        hpEffectImg.fillAmount -= effectSpeed;
                    else
                        hpEffectImg.fillAmount -= (effectSpeed/4);
                else
                {
                    hpEffectImg.fillAmount = hpImg.fillAmount;
                    healthStatus = HpEffect.none;
                }
            }
                    
            
            //* Hp color Effect
            if (hpImg.fillAmount <= 0.25f)
                hpImg.color = new Color(0.8f, 0.01f, 0f);
            else if (hpImg.fillAmount <= 0.5f)
                hpImg.color = new Color(1f, 0.65f, 0f);
            else
                hpImg.color = new Color(0f, 0.85f, 0f);

            ChangeInHp(currentHpPercent);
        }
        
    }

    private void ChangeInHp(float hpPercent)
    {
        if (crisisCharm && lastHp != hp && hp > 0)
        {
            lastHp = hp;

            if (furyRedObj != null)
                furyRedObj.SetActive( (hp > 0) && (hpPercent <= 0.25f) );
            if (furyYellowObj != null)
                furyYellowObj.SetActive( (hpPercent > 0.25f) && (hpPercent <= 0.5f) );
        }
    }

        private void OnDrawGizmosSelected() //! ------- I M P O R T A N T -------
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(feetPos.position, feetBox);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(inWaterCheckOffset.position, inWaterCheckOffset.position + new Vector3(checkDist,0));
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(aboveWaterCheckOffset.position, aboveWaterCheckOffset.position + new Vector3(checkDist,0));
        if (holder.transform.eulerAngles.y > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ledgeCheckOffset.position, ledgeCheckOffset.position + new Vector3(-checkDist,0));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(wallCheckOffset.position, wallCheckOffset.position + new Vector3(-checkDist,0));
        }
        // Right
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ledgeCheckOffset.position, ledgeCheckOffset.position + new Vector3(checkDist,0));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(wallCheckOffset.position, wallCheckOffset.position + new Vector3(checkDist,0));
        }
    }


    // todo ------------------------------------------------------------------------------------
    // todo -----------------  M E C H A N I C S  ----------------------------------------------

    bool Interact()
    {
        return player.GetButtonDown("ZR");
        // float yValue = Mathf.Abs( player.GetAxis("Move Vertical") );
        // return (yValue > 0.5f);
    }
    private void Walk(float xValue)
    {
        if (Mathf.Abs(xValue) < 0.1f)
        {
            xValue = 0;
        }
        if (!receivingKnockback)
            body.velocity = new Vector2(xValue * moveSpeed, body.velocity.y);
    }
    private void playerDirection(float xValue=0)
    {
        if (xValue < -0.01f)
            holder.transform.eulerAngles = new Vector3(0,180);  // left
        else if (xValue > 0.01f)
            holder.transform.eulerAngles = new Vector3(0,0);    // right
    }
    private void Jump()
    {
        anim.SetBool("isGrounded", false);
        anim.SetBool("isFalling", false);
        anim.SetTrigger("jump");
        anim.speed = 1;
        jumping = true;
        jumpTimerCounter = jumpTimer;
        body.velocity = new Vector2(body.velocity.x, jumpHeight);
    }
    private void MidairJump()
    {
        anim.SetBool("isGrounded", false);
        anim.SetBool("isFalling", false);
        anim.SetTrigger("jump");
        anim.speed = 1;
        jumping = true;
        jumpTimerCounter = jumpTimer;
        body.velocity = new Vector2(body.velocity.x, jumpHeight);

        // Butterfree jump
        if (doubleJumpObj != null)
        {
            var pokemon = Instantiate(doubleJumpObj, doubleJumpSpawnPos.position, doubleJumpObj.transform.rotation);
            pokemon.transform.SetParent(doubleJumpSpawnPos, true);
            Ally ally = pokemon.GetComponent<Ally>();
            ally.trainer = this;
            butterfreeOut = true;
            if (butterfreeSlot != -1 && partyPokemonsUI[ butterfreeSlot ].color != new Color(0.3f,0.3f,0.3f))
            {
                doubleJumpBeforeAtk = true;
                partyPokemonsUI[ butterfreeSlot ].color = new Color(0.3f,0.3f,0.3f);
            }

            //* Looking left
            if (holder.transform.eulerAngles.y > 0)
                pokemon.transform.eulerAngles = new Vector3(0,-180);

        }

    }

    private bool CheckLedgeGrab()
    {
        // Left
        if (holder.transform.eulerAngles.y > 0)
        {
            wallCheck = Physics2D.Raycast(wallCheckOffset.position, Vector2.left, checkDist, whatIsGround);
            ledgeCheck = Physics2D.Raycast(ledgeCheckOffset.position, Vector2.left, checkDist, whatIsGround);
        }
        // Right
        else
        {
            wallCheck = Physics2D.Raycast(wallCheckOffset.position, Vector2.right, checkDist, whatIsGround);
            ledgeCheck = Physics2D.Raycast(ledgeCheckOffset.position, Vector2.right, checkDist, whatIsGround);
        }

        if (wallCheck && !ledgeCheck && !grounded && player.GetAxis("Move Vertical") >= -0.3f)
        {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
            anim.speed = 1;
            if (holder.transform.eulerAngles.y > 0)
                anim.SetTrigger("ledgeGrabLeft");
            else
                anim.SetTrigger("ledgeGrab");
            ledgeGrabbing = true;
            jumping = false;
            return true;
        }
        return false;
    }
    private bool CheckAtWaterSurface()
    {
        inWaterCheck = Physics2D.Raycast(inWaterCheckOffset.position, Vector2.left, checkDist, whatIsWater);
        aboveWaterCheck = Physics2D.Raycast(aboveWaterCheckOffset.position, Vector2.left, checkDist, whatIsWater);

        return (inWaterCheck && !aboveWaterCheck && !grounded);
    }

    public void NO_VELOCITY()
    {
        body.gravityScale = 0;
        body.velocity = Vector2.zero;
    }

    public void CLIMB_LEDGE()
    {
        if (holder.transform.eulerAngles.y > 0)
            transform.position += new Vector3(-1.5f * holder.transform.localScale.x, 4 * holder.transform.localScale.y);
        else
            transform.position += new Vector3(1.5f * holder.transform.localScale.x, 4 * holder.transform.localScale.y);

        body.gravityScale = origGrav;
        ledgeGrabbing = false;
    }


    private void Dash()
    {
        dashes = 0;
        StartCoroutine( restoreDash() );
    }
    IEnumerator restoreDash()
    {
        dashing = true;
        if (holder.transform.eulerAngles.y > 0)
            body.velocity = Vector2.left * dashSpeed;
        else
            body.velocity = Vector2.right * dashSpeed;

        yield return new WaitForSeconds(0.1f);
        dashing = false;
        body.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.3f);
        dashes = 1;
    }

    bool PressedStandardButton()
    {
        return (player.GetButtonDown("A") || player.GetButtonDown("B") || player.GetButtonDown("X") || player.GetButtonDown("Y"));
    }

    IEnumerator Dodge()
    {
        if (glint != null)
            glint.SetActive(false);
        anim.speed = 1;
        body.velocity = Vector2.zero;
        dodging = true;
        canDodge = false;
        anim.SetTrigger("dodge");
        Invincible(true);

        yield return new WaitForSeconds(0.5f);
        if (!dodgingThruScene)
            body.velocity = Vector2.zero;
        Invincible(false);
        dodging = false;
        
        yield return new WaitForSeconds(0.5f);
        canDodge = true;
        if (glint != null)
            glint.SetActive(true);
    }

    void Invincible(bool active)
    {
        isInvincible = active;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyProjectile"), active);
    }

    public void DrinkingMoomooMilk()
    {
        drinking = true;
        body.velocity = new Vector2(0, body.velocity.y);
        anim.speed = 1;
        if (chuggerCharm)
            anim.speed = 2;
        anim.SetBool("isDrinking", true);
        anim.SetTrigger("drink");
    }
    public void DrankMoomooMilk()
    {
        if (healEffect != null)
            Instantiate(healEffect, this.transform.position + new Vector3(0,1), Quaternion.identity, this.transform);
        drinking = false;
        hp += ((25 * nBerries) + moomooMilkRecovery);
        if (hp > maxHp)
            hp = maxHp;
        Debug.Log( "<color=#4CFF00> Healed " + ((25 * nBerries) + moomooMilkRecovery) + " hp</color>");
        nMoomooMilkLeft--;
        float newSpeed = (float) hp / (float) maxHp;
        damageIndicatorAnim.SetFloat("fadeSpeed", newSpeed * newSpeed);

        if (healSound != null)
            healSound.Play();

        // UI indication
        if (moomooUi != null && nMoomooMilkLeft < moomooUi.Length)
            moomooUi[ nMoomooMilkLeft ].SetTrigger("used");
        
    }

    private void SelectDefaultItem()
    {
        foreach (ItemUi iu in itemsToActivate)
        {
            if (iu.gameObject.activeSelf && iu.transform.parent.gameObject.activeSelf)
            {
                iu.button.Select();
                break;
            }
        }
    }

    public void IncreaseNumberOfMoomooMilk()
    {
        foreach (Animator moomooAnim in extraMoomooUis[moomooUi.Length].excludedMoomooUi)
            moomooAnim.gameObject.SetActive(false);
        foreach (Animator moomooAnim in extraMoomooUis[moomooUi.Length].moomooUi)
            moomooAnim.gameObject.SetActive(true);
        moomooUi = extraMoomooUis[moomooUi.Length].moomooUi;
        nMoomooMilkLeft = moomooUi.Length;
    }
    public void DecreaseNumberOfMoomooMilk()
    {
        foreach (Animator moomooAnim in extraMoomooUis[moomooUi.Length - 2].excludedMoomooUi)
            moomooAnim.gameObject.SetActive(false);
        foreach (Animator moomooAnim in extraMoomooUis[moomooUi.Length - 2].moomooUi)
            moomooAnim.gameObject.SetActive(true);
        moomooUi = extraMoomooUis[moomooUi.Length - 2].moomooUi;
        nMoomooMilkLeft = moomooUi.Length;
    }

    public void FullRestore()
    {
        hp = maxHp;
    }
    public void CalculateMaxHp()
    {
        maxHp = 100 + (5 * (lv - 1));
        if (graciousHeartCharm)
            extraHp = Mathf.CeilToInt(maxHp * 0.2f);
        else
            extraHp = 0;
        maxHp += extraHp;
    }
    public void RecalculateHp()
    {
        if (hp > maxHp)
            hp = maxHp;
    }

    private void ToggleMap()
    {
        mapMenu.SetActive(!mapMenu.activeSelf);
        Debug.Log(lastScene.parentRect.localPosition + " : " + lastScene.rect.localPosition);
        if (mapMenu.activeSelf)
            mapMenuRect.localPosition = -lastScene.rect.localPosition + (Vector3) mapOffset;
    }
    private void LockToCurrentPos()
    {
        if (mapMenu.activeSelf)
            mapMenuRect.localPosition = -lastScene.rect.localPosition + (Vector3) mapOffset;
    }
    private void MoveMap()
    {
        float xVal = 10 * player.GetAxis("Look Horizontal");
        float yVal = 10 * player.GetAxis("Look Vertical");
        mapMenuRect.anchoredPosition -= new Vector2(xVal, yVal);
    }

    public void KilledEnemy(string enemyName)
    {
        if (enemyDefeated == null)
            enemyDefeated = new List<string>();

        enemyDefeated.Add(enemyName);
        PlayerPrefsElite.SetStringArray("enemyDefeated", enemyDefeated.ToArray());
    }


    // todo -----------------  D A M A G E  ------------------------------------------------
    public void TakeSpecialDamage(int dmg=0, Transform opponent=null, float force=0)
    {
        if (hp > 0 && !inCutscene)
        {
            Debug.Log("<color=#FF8800>Took " + dmg + " dmg</color>");
            anim.SetBool("isDrinking", false);
            hp -= dmg;
            if (dmg > 0 && hp > 0)
            {
                StartCoroutine( Flash() );
                
                //* STRONG ATTACK
                if (damageIndicatorAnim != null && force > 0)
                {
                    damageIndicatorAnim.SetTrigger("injured");
                    damageIndicatorAnim.SetFloat("fadeSpeed", hpImg.fillAmount * hpImg.fillAmount);
                }
                //* WEAK ATTACK (NO knockback)
                else if (damageIndicatorAnim != null && force == 0)
                {
                    damageIndicatorAnim.SetTrigger("injuredWeak");
                    damageIndicatorAnim.SetFloat("fadeSpeed", hpImg.fillAmount * hpImg.fillAmount);
                }
            }
                

            if (force > 0 && opponent != null)
            {
                if (hp > 0) StartCoroutine( Invincibility() );
                StartCoroutine( ApplyKnockback(opponent, force) );
            }

            if (hp <= 0)
                StartCoroutine( Died() );
        }
    }
    public void TakeDamage(int dmg=0, Transform opponent=null, float force=0)
    {
        // if (hp > 0 && !inCutscene)
        // if (hp > 0 && !inCutscene && !isInvincible)
        if (hp > 0 && !isInvincible)
        {
            Debug.Log("<color=#FF8800>Took " + dmg + " dmg</color>");
            anim.SetBool("isDrinking", false);
            hp -= dmg;
            if (dmg > 0 && hp > 0)
            {
                StartCoroutine( Flash() );
                
                //* STRONG ATTACK
                if (damageIndicatorAnim != null && force > 0)
                {
                    damageIndicatorAnim.SetTrigger("injured");
                    damageIndicatorAnim.SetFloat("fadeSpeed", hpImg.fillAmount * hpImg.fillAmount);
                }
                //* WEAK ATTACK (NO knockback)
                else if (damageIndicatorAnim != null && force == 0)
                {
                    damageIndicatorAnim.SetTrigger("injuredWeak");
                    damageIndicatorAnim.SetFloat("fadeSpeed", hpImg.fillAmount * hpImg.fillAmount);
                }
            }
                

            if (force > 0 && opponent != null)
            {
                if (hp > 0) StartCoroutine( Invincibility() );
                StartCoroutine( ApplyKnockback(opponent, force) );
            }

            if (hp <= 0)
                StartCoroutine( Died() );
        }
    }
    IEnumerator Invincibility()
    {
        Invincible(true);
        
        yield return new WaitForSeconds(0.75f);
        if (!dodging)
            Invincible(false);
    }
    IEnumerator Flash()
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null && origMat != null)
                renderer.material = flashMat;
        }
        
        yield return new WaitForSeconds(0.1f);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null && origMat != null)
                renderer.material = origMat;
        }
    }
    public IEnumerator ApplyKnockback(Transform opponent, float force)
    {

        receivingKnockback = true;
        Vector2 direction = (opponent.position - this.transform.position).normalized;
        // direction = new Vector2(direction.x, 0);
        body.velocity = new Vector2(-direction.x * force, 1);
        // body..MovePosition(-direction * force);
        
        yield return new WaitForSeconds(0.1f);
        body.velocity = Vector2.zero;
        receivingKnockback = false;
    }
    public void PutToSleep(float delay=0)
    {
        StartCoroutine( Sleeping(delay, Random.Range(0,3)) );   // sleep for 2 - 4 seconds
    }
    IEnumerator Sleeping(float delay, float duration)
    {
        if (sleepingEffect != null)
        {
            sleepingEffect.gameObject.SetActive(true);
            sleepingEffect.Play();
        }
        yield return new WaitForSeconds(delay + 0.5f);

        if (isSleeping || hp <= 0)
            yield break;
        isSleeping = true;

        anim.SetBool("isWalking", false);
        anim.SetBool("isGrounded", true);
        anim.SetBool("isFalling", false);
        anim.SetTrigger("reset");
        anim.speed = 1;
        body.velocity = Vector2.zero;
        if (face != null && sleepFace != null)
            face.sprite = sleepFace;

        yield return new WaitForSeconds(duration);
        if (face != null)
            face.sprite = origFace;
        if (sleepingEffect != null)
            sleepingEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        
        yield return new WaitForSeconds(1);
        isSleeping = false;
    }



    public void GainExp(int expGained, int enemyLevel)
    {
        if (lv < 100)
        {
            int totalExpGained = (int) ((expGained * Mathf.Min(3f, (float) enemyLevel / lv)) * expMultiplier);
            exp += totalExpGained;
            
            if  (expGainText != null)
            {
                // if (expGainText.gameObject.activeSelf)
                // {
                //     // New Added Bonus
                //     expGainText.text = (totalExpGained + expJustGained).ToString();
                //     expGainText.transform.parent.gameObject.SetActive(false);
                //     expGainText.transform.parent.gameObject.SetActive(true);
                //     expJustGained += totalExpGained;
                // }
                // else
                // {
                    // expJustGained = 0;
                    // New Bonus
                    expGainText.text = totalExpGained.ToString();
                    expGainText.transform.parent.gameObject.SetActive(false);
                    expGainText.transform.parent.gameObject.SetActive(true);
                //     expJustGained = totalExpGained;
                // }
            }
        }
        else
            exp = 0;
    }

    public void FillGauge(int spGained)
    {
        if (canUseUlt)
        {
            if ((sp + spGained) < spMax)
                sp += spGained;
            else
                sp = spMax;
            // gaugeImg.fillAmount += amount;
            if (gaugeGlow != null)
                if (gaugeImg.fillAmount < 1)
                    gaugeGlow.SetActive(false);
                else
                    gaugeGlow.SetActive(true);

        }
    }

    void LevelUp()
    {
        levelUpObj.SetActive(false);
        expEffectImg.fillAmount = 0;
        expImg.fillAmount = 0;
        levelUpObj.SetActive(true);
        lv++;
        if (levelUpSound != null)
            levelUpSound.Play();

        exp -= expNeeded;
        if (levelUpEffect != null)
        {
            var obj = Instantiate(levelUpEffect, this.transform.position, levelUpEffect.transform.rotation, this.transform);
            Destroy(obj.gameObject, 3);
        }
        expNeeded = (int) (expNeeded * 1.2f);

        // Increase health
        CalculateMaxHp();
        hp += 5;

        if (lvText != null)
             lvText.text = "Lv. " + lv;
        
        if (lv >= 100)
            exp = 0;

        if (exp < 0)
            exp = 0;
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  C U T S C E N E  ------------------------------------------------
    IEnumerator Died()
    {
        body.velocity = Vector2.zero;
        anim.SetTrigger("died");
        if (damageIndicatorAnim != null)
        {
            damageIndicatorAnim.SetFloat("fadeSpeed", 1);
            damageIndicatorAnim.SetTrigger("died");
        }
        if (healSound != null && healSound.isPlaying)
            healSound.Stop();
        if (furyRedObj != null)
            furyRedObj.SetActive(false);
        if (furyYellowObj != null)
            furyYellowObj.SetActive(false);
        
        if (musicManager != null)
            StartCoroutine( musicManager.LowerMusic(musicManager.currentMusic, 0.5f) );
        body.gravityScale = 0;
        if (col != null)
            col.enabled = false;
        Invincible(false);


        yield return new WaitForSeconds(2f);
        if (transitionAnim != null)
            transitionAnim.SetTrigger("toBlack");
        if (musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.previousMusic) );
        
        yield return new WaitForSeconds(1f);
        StartCoroutine( Respawn() );
        body.gravityScale = origGrav;
        if (col != null)
            col.enabled = true;
    }
    IEnumerator Respawn()
    {
        if (musicManager != null && musicManager.previousMusic != null)
        {
            // StartCoroutine( musicManager.StopMusic(musicManager.currentMusic) );
            yield return new WaitForSeconds(0.5f);
            // musicManager.StartMusic(musicManager.previousMusic);
        }
        if (damageIndicatorAnim != null)
        {
            damageIndicatorAnim.SetFloat("fadeSpeed", 1);
            damageIndicatorAnim.SetTrigger("respawn");
        }
            
        ReloadState();
        face.sprite = origFace;

        Invincible(false);
        hp = maxHp;
        anim.SetTrigger("reset");

        hpImg.fillAmount = 1;
        hpEffectImg.fillAmount = 1;
        if (furyRedObj != null)
            furyRedObj.SetActive(false);
        if (furyYellowObj != null)
            furyYellowObj.SetActive(false);

        isSleeping = false;
        inCutscene = false;
        ledgeGrabbing = false;

        if (transitionAnim != null)
            transitionAnim.SetTrigger("fromBlack");
    }

    public void SetNextArea(string nextArea, Vector2 newPos, bool exitingDoor=false, string sceneChanger="")
    {
        if (!inCutscene)
        {
            if (dodging)
                dodgingThruScene = true;
            else
                dodgingThruScene = false;
            
            if (transitionAnim != null)
                transitionAnim.SetTrigger("toBlack");
            StartCoroutine( MovingToNextArea(nextArea, newPos, exitingDoor, sceneChanger) );
        }
    }
    public IEnumerator MovingToNextArea(string nextArea, Vector2 newPos, bool exitingDoor, string sceneChanger="")
    {
        if (!inCutscene)
        {
            inCutscene = true;
            bool walkingRight = (body.velocity.x > 0);

            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(nextArea); Debug.Log("<color=#0EB8BF>" + nextArea + "</color> - " + sceneChanger);
            body.velocity = Vector2.zero;

            yield return new WaitForEndOfFrame();
            this.transform.position = newPos;

            yield return new WaitForSeconds(0.5f);
            string sceneName = SceneManager.GetActiveScene().name;
            if (!visitedScenes.Contains(sceneName))
            {
                visitedScenes.Add(sceneName);
                PlayerPrefsElite.SetStringArray("visitedScenes" + gameNumber, visitedScenes.ToArray());
                sceneMaps[sceneName].Visited();
            }
            if (sceneMaps.ContainsKey(sceneName))
            {
                lastScene.LeftScene();
                sceneMaps[sceneName].EnterScene();
                lastScene = sceneMaps[sceneName];
            }
            
            string newSceneFirstWord = sceneName.Split(' ')[0];
            if (newSceneFirstWord != PlayerPrefsElite.GetString("currentArea" + gameNumber))
            {
                PlayerPrefsElite.SetString("currentArea" + gameNumber, newSceneFirstWord);
                if (musicManager != null)
                    StartingMusic(newSceneFirstWord);
            }
            if (transitionAnim != null)
                transitionAnim.SetTrigger("fromBlack");

            if (mapMenu.activeSelf)
                mapMenuRect.localPosition = -lastScene.rect.localPosition + (Vector3) mapOffset;
            
            yield return new WaitForSeconds(0.4f);
            AllPokemonReturned();

            if (!exitingDoor)
            {
                if (!walkingRight)
                    StartCoroutine( WalkingLeft() );
                else
                    StartCoroutine( WalkingRight() );
            }
            else
                StartCoroutine( ExitingDoor() );
        }
    }

    IEnumerator WalkingRight()  // Moving right
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isFalling", false);
        anim.SetTrigger("reset");

        if (dodgingThruScene)
            anim.SetTrigger("dodge");

        body.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        inCutscene = false;
        canEnter = false;
        body.velocity = Vector2.zero;
    }
    IEnumerator WalkingLeft()  // Moving left
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isFalling", false);
        anim.SetTrigger("reset");
        
        if (dodgingThruScene)
            anim.SetTrigger("dodge");

        body.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        inCutscene = false;
        canEnter = false;
        body.velocity = Vector2.zero;
    }
    IEnumerator ExitingDoor()  // Moving left
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isFalling", false);
        anim.SetBool("isWalking", false);
        anim.SetTrigger("reset");

        yield return new WaitForSeconds(0.5f);
        inCutscene = false;
        canEnter = false;
        body.velocity = Vector2.zero;
    }

    public IEnumerator EnteringDoor()
    {
        if (!inCutscene)
        {
            body.velocity = Vector2.zero;
            inCutscene = true;

            if (transitionAnim != null)
                transitionAnim.SetTrigger("toBlack");

            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(newSceneName); Debug.Log("<color=#0EB8BF>" + newSceneName + "</color>");
            body.velocity = Vector2.zero;

            yield return new WaitForEndOfFrame();
            this.transform.position = newScenePos;

            yield return new WaitForSeconds(0.1f);
            if (transitionAnim != null)
                transitionAnim.SetTrigger("fromBlack");
            
            yield return new WaitForSeconds(0.4f);

            if (moveLeftFromDoor)
            {
                holder.transform.eulerAngles = new Vector3(0,180);
                StartCoroutine( WalkingLeft() );
            }
            else
            {
                holder.transform.eulerAngles = new Vector3(0,0);
                StartCoroutine( WalkingRight() );
            }
        }
    }

    public void CUTSCENE_EVENT_ON()
    {
        inCutscene = true;
    }
    public void CUTSCENE_EVENT_OFF()
    {
        inCutscene = false;
    }

    public void ReturnToTitle()
    {
        returningToTitle = true;
        EXIT_PAUSE_MENU();
        if (titleScreenObj != null)
        {
            titleScreenObj.toDestroy = this.gameObject;
            titleScreenObj.transform.parent = null;
            titleScreenObj.ReturnToTitle();
        }
        playerInstance = null;
        
        Time.timeScale = 1;
        this.enabled = false;
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  T R I G G E R S  ------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Underwater"))
            inWater = true;
            // nExtraJumpsLeft = nExtraJumps;
        if (other.CompareTag("Rage") && musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.bossOutroMusic) );
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Underwater"))
            inWater = false;
        if (other.CompareTag("Roar"))
            RoarOver();
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  M U S I C  ------------------------------------------------------
    
    public void StartingMusic(string location="forest")
    {
        if (musicManager != null)
        {
            bool notNull = (locationAnimObj != null && locationName != null);
            if (notNull)
            {
                locationAnimObj.SetActive(false);
                locationAnimObj.SetActive(true);
            }

            switch (location.ToLower())
            {
                case "start":
                    StartCoroutine( musicManager.TransitionMusic(musicManager.forestMusic) );
                    break;
                case "forest":
                    StartCoroutine( musicManager.TransitionMusic(musicManager.forestMusic) );
                    if (notNull)
                        locationName.text = "Whispering Woods";
                    break;
                case "swamp":
                    StartCoroutine( musicManager.TransitionMusic(musicManager.swampMusic) );
                    if (notNull)
                        locationName.text = "Somber Swamplands";
                    break;
                default:
                    Debug.LogError("Location has not been register in switch PlayerControls.StartingMusic()");
                    break;
            }
        }
    }
    
    public void BossBattleOver()
    {
        if (musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.previousMusic) );
    }
    
    
    // todo ------------------------------------------------------------------------------------
    // todo ---------------------  B O S S  ----------------------------------------------------

    public void EngagedBossRoar(string musicName)
    {
        if (ledgeGrabbing)
        {
            body.gravityScale = origGrav;
            ledgeGrabbing = false;
        }
        inCutscene = true;
        anim.speed = 1;
        anim.SetBool("inBossFight", true);
        anim.SetTrigger("engaged");
        body.velocity = Vector2.zero;

        if (musicManager != null && musicName.ToLower() == "rosary")
            StartCoroutine( musicManager.TransitionMusic(musicManager.bossIntroMusic, true) );
        if (musicManager != null && musicName.ToLower() == "accolade")
            StartCoroutine( musicManager.TransitionMusic(musicManager.accoladeOutroMusic) );
    }

    public void EnteredWaveRoom()
    {
        if (musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.accoladeIntroMusic, true) );
    }


    
    public void RoarOver()
    {
        anim.SetBool("inBossFight", false);
        inCutscene = false;
        body.velocity = Vector2.zero;
    }

    // todo ------------------------------------------------------------------------------------

    public void SelectAllyToReplace(string buttonSymbol, Button lastButton)
    {
        oldButtonSymbol = buttonSymbol;

        if      (allies[0] == newAllyToEquip) { allies[0] = null; partyPokemonsUI[0].sprite = emptySprite; 
            pokemonInTeamBenchSettings[0].img.sprite = emptySprite; pokemonInTeamBenchSettings[0].ally = null;}
        else if (allies[1] == newAllyToEquip) { allies[1] = null; partyPokemonsUI[1].sprite = emptySprite; 
            pokemonInTeamBenchSettings[1].img.sprite = emptySprite; pokemonInTeamBenchSettings[1].ally = null;}
        else if (allies[2] == newAllyToEquip) { allies[2] = null; partyPokemonsUI[2].sprite = emptySprite; 
            pokemonInTeamBenchSettings[2].img.sprite = emptySprite; pokemonInTeamBenchSettings[2].ally = null;}
        else if (allies[3] == newAllyToEquip) { allies[3] = null; partyPokemonsUI[3].sprite = emptySprite; 
            pokemonInTeamBenchSettings[3].img.sprite = emptySprite; pokemonInTeamBenchSettings[3].ally = null;}
        else if (allies[4] == newAllyToEquip) { allies[4] = null; partyPokemonsUI[4].sprite = emptySprite; 
            pokemonInTeamBenchSettings[4].img.sprite = emptySprite; pokemonInTeamBenchSettings[4].ally = null;}
        else if (allies[5] == newAllyToEquip) { allies[5] = null; partyPokemonsUI[5].sprite = emptySprite; 
            pokemonInTeamBenchSettings[5].img.sprite = emptySprite; pokemonInTeamBenchSettings[5].ally = null;}


        switch (oldButtonSymbol.ToUpper())
        {
            case "Y1": 
                allies[0] = newAllyToEquip; partyPokemonsUI[0].sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[0].img.sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[0].ally = newAllyToEquip;
                break;
            case "X1": 
                allies[1] = newAllyToEquip; partyPokemonsUI[1].sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[1].img.sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[1].ally = newAllyToEquip;
                break;
            case "A1": 
                allies[2] = newAllyToEquip; partyPokemonsUI[2].sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[2].img.sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[2].ally = newAllyToEquip;
                break;
            case "Y2": 
                allies[3] = newAllyToEquip; partyPokemonsUI[3].sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[3].img.sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[3].ally = newAllyToEquip;
                break;
            case "X2": 
                allies[4] = newAllyToEquip; partyPokemonsUI[4].sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[4].img.sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[4].ally = newAllyToEquip;
                break;
            case "A2": 
                allies[5] = newAllyToEquip; partyPokemonsUI[5].sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[5].img.sprite = newAllySpriteToEquip; 
                pokemonInTeamBenchSettings[5].ally = newAllyToEquip;
                break;
        }

        foreach (Button button in partyPokemonButtons)
            button.interactable = false;
        foreach (Button button in boxPokemonButtons)
            button.interactable = true;

        if (boxPokemonLastSelected != null)
            boxPokemonLastSelected.Select();
        else if (boxPokemonFirstSelected != null)
            boxPokemonFirstSelected.Select();
        partyPokemonLastSelected = lastButton;
    }

    public void SetNewAlly(Ally newAlly, Sprite newSprite, Button lastButton)
    {
        newAllyToEquip = newAlly;
        newAllySpriteToEquip = newSprite;

        foreach (Button button in boxPokemonButtons)
            button.interactable = false;
        foreach (Button button in partyPokemonButtons)
            button.interactable = true;


        if (partyPokemonLastSelected != null)
            partyPokemonLastSelected.Select();
        else if (partyPokemonFirstSelected != null)
            partyPokemonFirstSelected.Select();
        boxPokemonLastSelected = lastButton;
    }


    public void GainPowerup(string powerupName)
    {
        Debug.Log("Gained a power = " + powerupName);

        switch (powerupName)
        {
            case "butterfree": 
                canDoubleJump = true;
                PlayerPrefsElite.SetBoolean("canDoubleJump" + gameNumber, canDoubleJump);
                inCutscene = true;
                doubleJumpScreen.gameObject.SetActive(true);
                CaughtAPokemon("butterfree");
                break;
            case "pidgey": 
                CaughtAPokemon("pidgey");
                break;
            case "oddish": 
                CaughtAPokemon("oddish");
                break;
            case "tangela": 
                CaughtAPokemon("tangela");
                break;
            default:
                CaughtAPokemon(powerupName);
                Debug.LogError("PlayerControls.GainPowerup - unregistered powerup (ADD TO SWITCH CASE)");
                break;
        }
        body.velocity = Vector2.zero;
    }

    
    public void EquipItem(Sprite itemSprite)
    {
        equippedItems[nEquipped].sprite = itemSprite;
        nEquipped++;
        weightText.text = currentWeight + "/" + (maxWeight + extraWeight);
    }
    public void TooHeavy()
    {
        if (weightAnim != null)
        {
            weightAnim.SetTrigger("tooHeavy");
            Debug.LogError("TOO HEAVY");
        }
    }
    public void UnequipItem(Sprite itemSprite)
    {
        bool unequipped = false;
        for (int i=0 ; i<equippedItems.Length ; i++)
        {
            if (equippedItems[i].sprite == itemSprite || unequipped)
            {
                unequipped = true;
                equippedItems[i].sprite = emptySprite;
                if (i < equippedItems.Length - 1)
                    equippedItems[i].sprite = equippedItems[i+1].sprite;
            }
        }
        nEquipped--;
    }

    public void IncreaseMaxPokemonOut()
    {
        maxPokemonOut++;
    }

    public void EXIT_EQUIPMENT_MENU()
    {
        if (!isClosing)
        {
            isClosing = true;
            equimentSettings.SetTrigger("close");
        }
    }
    public void EXIT_PAUSE_MENU()
    {
        settings.SetTrigger("confirm");
    }
    
    public void Resume()
    {
        settings.gameObject.SetActive(false);
        equimentSettings.gameObject.SetActive(false);
        equimentSettings.SetTrigger("close");
        Time.timeScale = 1;
        StartCoroutine( CanPressButtonsagain() );
    }
    IEnumerator CanPressButtonsagain()
    {
        yield return new WaitForEndOfFrame();
        inCutscene = false;
    }
    public void FinishedCutscene()
    {
        inCutscene = false;
    }

    public void RestOnBench()
    {
        body.velocity = Vector2.zero;
        resting = true;
        FullRestore();
        damageIndicatorAnim.SetFloat("fadeSpeed", 1);
        anim.speed = 1;
        anim.SetTrigger("rest");
        anim.SetBool("isResting", true);
        if (moomooUi != null)
        {
            for (int i=nMoomooMilkLeft ; i<moomooUi.Length ; i++)
                moomooUi[i].SetTrigger("restored");
        }
        nMoomooMilkLeft = moomooUi.Length;
        enemyDefeated.Clear(); PlayerPrefsElite.SetStringArray("enemyDefeated", new string[0]);

        SaveState();
    }

    public void LeaveBench()
    {
        resting = false;
        anim.SetBool("isResting", false);

        SavePokemonTeam();
        SaveEquippedItems();
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  P R E F S  ------------------------------------------------------

    public void ReloadState()
    {
        nMoomooMilkLeft = moomooUi.Length;
        // UI indication
        if (moomooUi != null)
            foreach (Animator ui in moomooUi)
                ui.SetTrigger("restored");

        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump" + gameNumber))
            canDoubleJump = PlayerPrefsElite.GetBoolean("canDoubleJump" + gameNumber);
        if (PlayerPrefsElite.VerifyBoolean("canDash" + gameNumber))
            canDash = PlayerPrefsElite.GetBoolean("canDash" + gameNumber);
        // if (PlayerPrefsElite.VerifyInt("playerLevel" + gameNumber))
        // {
        //     lv = PlayerPrefsElite.GetInt("playerLevel" + gameNumber);
        //     CalculateMaxHp();
        //     if (lvText != null)
        //          lvText.text = "Lv. " + lv;
        // }
        // expNeeded = (int) (100 * Mathf.Pow(1.2f, lv - 1));

        // if (PlayerPrefsElite.VerifyInt("playerExp" + gameNumber))
        //     exp = PlayerPrefsElite.GetInt("playerExp" + gameNumber);

        hp = maxHp;
        if (PlayerPrefsElite.VerifyString("checkpointScene" + gameNumber) && PlayerPrefsElite.VerifyVector3("checkpointPos" + gameNumber))
        {
            SceneManager.LoadScene(PlayerPrefsElite.GetString("checkpointScene" + gameNumber));
            this.transform.position = PlayerPrefsElite.GetVector3("checkpointPos" + gameNumber);
            Debug.Log("<color=#0EB8BF>Reload " + PlayerPrefsElite.GetString("checkpointScene" + gameNumber) + "</color>");
        }

        if (PlayerPrefsElite.VerifyArray("roomsBeaten" + gameNumber))
            PlayerPrefsElite.SetStringArray("roomsBeaten" + gameNumber, roomsBeaten.ToArray());
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught" + gameNumber))
            PlayerPrefsElite.SetStringArray("pokemonsCaught" + gameNumber, pokemonsCaught.ToArray());
        if (PlayerPrefsElite.VerifyArray("itemsObtained" + gameNumber))
            PlayerPrefsElite.SetStringArray("itemsObtained" + gameNumber, itemsObtained.ToArray());
        if (PlayerPrefsElite.VerifyArray("berriesCollected" + gameNumber))
            PlayerPrefsElite.SetStringArray("berriesCollected" + gameNumber, berriesCollected.ToArray());
        if (PlayerPrefsElite.VerifyArray("spareKeychain" + gameNumber))
            PlayerPrefsElite.SetStringArray("spareKeychain" + gameNumber, spareKeychainCollected.ToArray());
        
        var berriesSet = new HashSet<string>(berriesCollected);
        if (berriesSet.Contains(""))
            berriesSet.Remove("");
        nBerries = berriesSet.Count;
        var spareKeychainSet = new HashSet<string>(spareKeychainCollected);
        if (spareKeychainSet.Contains(""))
            spareKeychainSet.Remove("");
        extraWeight = spareKeychainCollected.Count;
        

        CheckEquippablePokemon();
        CheckObtainedItems();

        enemyDefeated.Clear(); PlayerPrefsElite.SetStringArray("enemyDefeated", new string[0]);
    }
    public void SaveState()
    {
        PlayerPrefsElite.SetString("checkpointScene" + gameNumber, SceneManager.GetActiveScene().name);
        PlayerPrefsElite.SetVector3("checkpointPos" + gameNumber, this.transform.position + new Vector3(0,0.5f));
        PlayerPrefsElite.SetInt("playerExp" + gameNumber, exp);
        PlayerPrefsElite.SetInt("playerLevel" + gameNumber, lv);
        PlayerPrefsElite.SetBoolean("canDoubleJump" + gameNumber, canDoubleJump);
        PlayerPrefsElite.SetBoolean("canDash" + gameNumber, canDash);

        if (PlayerPrefsElite.VerifyArray("roomsBeaten" + gameNumber))
            roomsBeaten = new List<string>( PlayerPrefsElite.GetStringArray("roomsBeaten" + gameNumber) );
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught" + gameNumber))
            pokemonsCaught = new List<string>( PlayerPrefsElite.GetStringArray("pokemonsCaught" + gameNumber) );
        if (PlayerPrefsElite.VerifyArray("itemsObtained" + gameNumber))
            itemsObtained = new List<string>( PlayerPrefsElite.GetStringArray("itemsObtained" + gameNumber) );
        if (PlayerPrefsElite.VerifyArray("berriesCollected" + gameNumber))
            berriesCollected = new List<string>( PlayerPrefsElite.GetStringArray("berriesCollected" + gameNumber) );
        if (PlayerPrefsElite.VerifyArray("spareKeychain" + gameNumber))
            spareKeychainCollected = new List<string>( PlayerPrefsElite.GetStringArray("spareKeychain" + gameNumber) );
    }

    private void SavePokemonTeam()
    {
        string[] buttonAllocatedPokemons = new string[6];
        if (allies[0] != null) buttonAllocatedPokemons[0] = allies[0].name;
        if (allies[1] != null) buttonAllocatedPokemons[1] = allies[1].name;
        if (allies[2] != null) buttonAllocatedPokemons[2] = allies[2].name;
        if (allies[3] != null) buttonAllocatedPokemons[3] = allies[3].name;
        if (allies[4] != null) buttonAllocatedPokemons[4] = allies[4].name;
        if (allies[5] != null) buttonAllocatedPokemons[5] = allies[5].name;
        PlayerPrefsElite.SetStringArray("buttonAllocatedPokemons" + gameNumber, buttonAllocatedPokemons);

        butterfreeSlot = RememberSpecialPokemonSlot( SpecialPokemon.butterfree );
    }
    private void SaveEquippedItems()
    {
        PlayerPrefsElite.SetStringArray("equippedItems" + gameNumber, equippedItemNames.ToArray());
    }

    //* REMEMBER SPECIAL POKEMON SLOTS (DYNAMIC PROGRAMMING) 
    private int RememberSpecialPokemonSlot(SpecialPokemon sp)
    {
        for (int i=0 ; i<allies.Length ; i++)
        {
            if (sp == SpecialPokemon.butterfree && allies[i] == butterfree.summonable)
                return i;
        }
        return -1;
    }

    public void CaughtAPokemon(string pokemonName)
    {
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught" + gameNumber))
        {
            List<string> temp = new List<string>( PlayerPrefsElite.GetStringArray("pokemonsCaught" + gameNumber) );
            temp.Add(pokemonName);
            PlayerPrefsElite.SetStringArray("pokemonsCaught" + gameNumber, temp.ToArray());
            var set = new HashSet<string>(temp);

            Debug.Log("pokemons caught set = " + set.Count);
            bool foundMatch = false;
            foreach (BoxPokemonButton boxPokemon in boxPokemonsToActivate)
            {
                if (set.Contains(boxPokemon.pokemonName))
                {
                    foundMatch = true;
                    boxPokemon.transform.parent.gameObject.SetActive(true);
                    boxPokemon.gameObject.SetActive(true);
                }
            }
            if (!foundMatch)
                Debug.LogError("PlayerControls.CaughtAPokemon - unregistered pokemon (ADD TO boxPokemonsToActivate)");

            CheckEquippablePokemon();
        }
    }
    public void CheckEquippablePokemon()
    {
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught" + gameNumber))
        {
            List<string> temp = new List<string>( PlayerPrefsElite.GetStringArray("pokemonsCaught" + gameNumber) );
            var set = new HashSet<string>(temp);
            
            // Debug.Log("pokemons caught set = " + set.Count);
            
            foreach (BoxPokemonButton boxPokemon in boxPokemonsToActivate)
            {
                if (boxPokemon.pokemonName != null && set.Contains(boxPokemon.pokemonName))
                {
                    boxPokemon.transform.parent.gameObject.SetActive(true);
                    boxPokemon.gameObject.SetActive(true);
                }
                else
                {
                    boxPokemon.transform.parent.gameObject.SetActive(false);
                    boxPokemon.gameObject.SetActive(false);
                }
            }
        }
    }
    
    //* Set all obtained items gameObject (buttons) active - Start(), GainItem()
    public void CheckObtainedItems()
    {
        if (PlayerPrefsElite.VerifyArray("itemsObtained" + gameNumber))
        {
            itemsObtained = new List<string>( PlayerPrefsElite.GetStringArray("itemsObtained" + gameNumber) );
            var set = new HashSet<string>(itemsObtained);
            foreach (ItemUi heldItem in itemsToActivate)
            {
                if (set.Contains(heldItem.itemName))
                {
                    heldItem.transform.parent.gameObject.SetActive(true);
                    heldItem.gameObject.SetActive(true);
                }
                else
                {
                    heldItem.transform.parent.gameObject.SetActive(false);
                    heldItem.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            PlayerPrefsElite.SetStringArray("itemsObtained" + gameNumber, new string[0]);
        }
    }

    // todo ------------------------------------------------------------------------------------

    public enum SpecialPokemon { none, butterfree, alakazam, starmie };
    private SpecialPokemon IsSpecialPokemon(Ally pokemonSummoned)
    {
        if (pokemonSummoned == butterfree.summonable)
        {
            
            return SpecialPokemon.butterfree;
        }
        return SpecialPokemon.none;
    }


    void PokemonSummonedIndicator(string button)
    {
        if (noCoolDown)
            return;
        switch (button.ToUpper())
        {
            case "Y":
                partyPokemonsUI[0].color = new Color(0.3f,0.3f,0.3f);
                canPressButtonWest = false;
                pokeballY1.fillAmount = 0;
                break;
            case "X":
                partyPokemonsUI[1].color = new Color(0.3f,0.3f,0.3f);
                canPressButtonNorth = false;
                pokeballX1.fillAmount = 0;
                break;
            case "A":
                partyPokemonsUI[2].color = new Color(0.3f,0.3f,0.3f);
                canPressButtonEast = false;
                pokeballA1.fillAmount = 0;
                break;
            case "Y2":
                partyPokemonsUI[3].color = new Color(0.3f,0.3f,0.3f);
                canPressButtonWest2 = false;
                pokeballY2.fillAmount = 0;
                break;
            case "X2":
                partyPokemonsUI[4].color = new Color(0.3f,0.3f,0.3f);
                canPressButtonNorth2 = false;
                pokeballX2.fillAmount = 0;
                break;
            case "A2":
                partyPokemonsUI[5].color = new Color(0.3f,0.3f,0.3f);
                canPressButtonEast2 = false;
                pokeballA2.fillAmount = 0;
                break;

            default:
                Debug.LogError("Not a registered (Wrong) Button");
                break;
        }
    }
    
    public void ButterfreeReturned()
    {
        if (doubleJumpBeforeAtk && butterfreeSlot != -1 && butterfreeSlot < partyPokemonsUI.Length)
        {
            partyPokemonsUI[ butterfreeSlot ].color = new Color(1,1,1);
            doubleJumpBeforeAtk = false;
        }
    }
    public void AllPokemonReturned()
    {
        nPokemonOut = 0;
        butterfreeOut = false;

        canPressButtonWest = true;
        partyPokemonsUI[0].color = new Color(1,1,1);
        canPressButtonNorth = true;
        partyPokemonsUI[1].color = new Color(1,1,1);
        canPressButtonEast = true;
        partyPokemonsUI[2].color = new Color(1,1,1);
        canPressButtonWest2 = true;
        partyPokemonsUI[3].color = new Color(1,1,1);
        canPressButtonNorth2 = true;
        partyPokemonsUI[4].color = new Color(1,1,1);
        canPressButtonEast2 = true;
        partyPokemonsUI[5].color = new Color(1,1,1);

        pokeballY1.fillAmount = 1;
        pokeballX1.fillAmount = 1;
        pokeballA1.fillAmount = 1;
        pokeballY2.fillAmount = 1;
        pokeballX2.fillAmount = 1;
        pokeballA2.fillAmount = 1;
    }
    public void PokemonReturned(string button, float coolDown)
    {
        if (button == "")
            return;
        StartCoroutine( PokemonCooldown(button.ToUpper(), coolDown) );
    }
    IEnumerator PokemonCooldown(string button, float cooldown) //* NORTH
    {
        if (nPokemonOut > 0)
            nPokemonOut--;  // pokemon returned to ball
        int repeat = 40;
        float s = cooldown / repeat;
        float amount = 1f / repeat;
        switch (button.ToUpper())
        {
            case "Y":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballY1.fillAmount += amount;
                }
                canPressButtonWest = true;
                partyPokemonsUI[0].color = new Color(1,1,1);
                break;

            case "X":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballX1.fillAmount += amount;
                }
                canPressButtonNorth = true;
                partyPokemonsUI[1].color = new Color(1,1,1);
                break;

            case "A":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballA1.fillAmount += amount;
                }
                canPressButtonEast = true;
                partyPokemonsUI[2].color = new Color(1,1,1);
                break;

            case "Y2":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballY2.fillAmount += amount;
                }
                canPressButtonWest2 = true;
                partyPokemonsUI[3].color = new Color(1,1,1);
                break;

            case "X2":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballX2.fillAmount += amount;
                }
                canPressButtonNorth2 = true;
                partyPokemonsUI[4].color = new Color(1,1,1);
                break;

            case "A2":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballA2.fillAmount += amount;
                }
                canPressButtonEast2 = true;
                partyPokemonsUI[5].color = new Color(1,1,1);
                break;

            default:
                Debug.LogError("PokemonCooldown() - Unregister button (" + button + ")");
                break;
        }

    }

    public void SetPokemon(string button, Ally ally)
    {
        switch (button.ToUpper())
        {
            case "Y":    allies[0] = ally;   break;
            case "X":    allies[1] = ally;   break;
            case "A":    allies[2] = ally;   break;
            case "Y2":   allies[3] = ally;   break;
            case "X2":   allies[4] = ally;   break;
            case "A2":   allies[5] = ally;   break;

            default:
                Debug.LogError("Not a registered (Wrong) Button");
                break;
        }
    }

    public void UnlockGauge()
    {
        if (canUseUlt)
        {
            canUseUlt = false;
            gaugesHolder.SetActive(false);
        }
        else
        {
            canUseUlt = true;
            gaugesHolder.SetActive(true);
        }
        gaugeImg.fillAmount = 0;
        gaugeGlow.SetActive(false);
    }
    public bool InfiniteGauge()
    {
        infiniteGauge = !infiniteGauge;
        return infiniteGauge;
    }

}



[System.Serializable]
public class AllyTeamUI
{
    public Ally summonable;
    public Sprite sprite;
}

[System.Serializable]
public class MoomooMilkUi
{
    public Animator[] moomooUi;
    public Animator[] excludedMoomooUi;
}

[CanEditMultipleObjects] [CustomEditor(typeof(PlayerControls), true)]
public class PlayerControlsEditor : Editor
{
    private bool infiniteSp;
    public override void OnInspectorGUI()
    {

        PlayerControls playerScript = (PlayerControls)target;

        if (playerScript.canUseUlt && GUILayout.Button("Lock Gauge"))
            playerScript.UnlockGauge();
        else if (!playerScript.canUseUlt && GUILayout.Button("Unlock Gauge"))
            playerScript.UnlockGauge();
        EditorGUILayout.Space();

        if (!infiniteSp && GUILayout.Button("Infinite Gauge"))
            infiniteSp = playerScript.InfiniteGauge();
        if (infiniteSp && GUILayout.Button("Finite Gauge"))
            infiniteSp = playerScript.InfiniteGauge();
        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}