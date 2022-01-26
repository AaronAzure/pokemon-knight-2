using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;
// using Com.LuisPedroFonseca.ProCamera2D;

public class PlayerControls : MonoBehaviour
{
    private Player player;
    public int playerID = 0;
    [SerializeField] private GameObject rewiredInputSystem;
    

    [Space] [Header("Ui")]
    public Image hpEffectImg;
    public Image hpImg;
    [Space]
    public Image expEffectImg;  // white
    public Image expImg;        // blue
    [Space]
    public TextMeshProUGUI lvText;
    [SerializeField] private float effectSpeed = 0.005f;
    public Animator transitionAnim;
    public Sprite emptySprite;
    
    [Space] 
    public Image pokeballY1;
    public Image pokeballX1;
    public Image pokeballA1;
    public Image pokeballY2;
    public Image pokeballX2;
    public Image pokeballA2;

    [Space] 
    public Image[] partyPokemonsUI;
    // public Image pokemonY1;
    // public Image pokemonX1;
    // public Image pokemonA1;
    // public Image pokemonY2;
    // public Image pokemonX2;
    // public Image pokemonA2;

    [Space] 
    public Image[] partyPokemonSettings;
    // public Image pokemonY1Settings;
    // public Image pokemonX1Settings;
    // public Image pokemonA1Settings;
    // public Image pokemonY2Settings;
    // public Image pokemonX2Settings;
    // public Image pokemonA2Settings;

    [Header("Pokemon (Allies)")]
    [SerializeField] private Transform spawnPos;    // Place to Summon Pokemon
    [Space][SerializeField] private GameObject doubleJumpObj;   //Butterfree

    [Space] public bool isSet1=true; // true = first set, false = second set
    public bool canSwitchSets=true;
    
    [Space] [HideInInspector] public Ally[] allies;


    [Header("Pre-init Pokemon for PlayerPrefs")]
    [SerializeField] private AllyTeamUI bulbasaur;
    [Space] [SerializeField] private AllyTeamUI charmander;
    [Space] [SerializeField] private AllyTeamUI squirtle;
    [Space] [SerializeField] private AllyTeamUI pidgey;
    [Space] [SerializeField] private AllyTeamUI butterfree;



    [Space] [Header("Settings UI")]

    [Space] [SerializeField] private Animator settings;
    [Space] [SerializeField] private Animator equimentSettings;
    [Space] [SerializeField] private Animator pokemonSets;

    [Space] [SerializeField] private Canvas pokemonSet1;
    [SerializeField] private Canvas pokemonSet2;
    
    [Space] [SerializeField] private BoxPokemonButton[] boxPokemonsToActivate;
    [Space] [SerializeField] private ItemUi[] itemsToActivate;

    [Space] [SerializeField] private Button partyPokemonFirstSelected;
    [SerializeField] private Button boxPokemonFirstSelected;
    [Space] [SerializeField] private Button partyPokemonLastSelected;
    [SerializeField] private Button boxPokemonLastSelected;
    [Space] [SerializeField] private string oldButtonSymbol;

    
    [Space]
    [Header("Damage Related")]
    [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Material flashMat;
    [SerializeField] private Material origMat;


    [Space]
    [Header("Player data")]
    public int maxHp;
    public int hp;  // current hp
    public int lv=1;
    private int expNeeded=100;
    private int exp;  // current exp
    [SerializeField] private GameObject levelUpEffect;
    [SerializeField] private GameObject levelUpObj;
    [SerializeField] private AudioSource levelUpSound;
    [SerializeField] private GameObject playerUi;
    [Space] [SerializeField] private string[] roomsBeaten;
    [Space] [SerializeField] private string[] pokemonsCaught;
    [Space] [SerializeField] private string[] itemsObtained;
    public Item currentItem;


    [Space] [Header("Platformer Mechanics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float dashSpeed = 50;
    [SerializeField] private float dashTime = 0.3f;
    [SerializeField] private float jumpHeight = 10;
    [SerializeField] private float jumpTimer = 0.35f;
    private float jumpTimerCounter = 0;

    [Space]
    [SerializeField] private Transform feetPos; // To detect ground
    [SerializeField] private float feetRadius;
    [SerializeField] private Vector2 feetBox;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] public bool grounded = true;
    [HideInInspector] public bool jumping = false;
    [Space] [SerializeField] private bool leftWallDetect = true;
    [SerializeField] private Transform leftWallPos;
    [SerializeField] private bool rightWallDetect = true;
    [SerializeField] private Transform rightWallPos;
    [SerializeField] private Vector2 wallDetectBox;
    private bool receivingKnockback;
    private int dashes = 1;
    private bool dashing;
    private bool dodging;
    private bool canDodge = true;
    private bool dodgingThruScene;
    private float dodgeSpeed = 7.5f;

    [SerializeField] private bool inWater;


    private bool canPressButtonNorth = true;   // (X)
    private bool canPressButtonWest = true;    // (Y)
    private bool canPressButtonEast = true;    // (A)
    private bool canPressButtonNorth2 = true;   // (X)
    private bool canPressButtonWest2 = true;    // (Y)
    private bool canPressButtonEast2 = true;    // (A)
    private int nPokemonOut;
    private int maxPokemonOut = 1;
    [Space] [SerializeField] private GameObject holder;
    [SerializeField] private Animator anim;
    public bool inCutscene;
    private bool returningToTitle;


    [Space] [Header("Moomoo milk")]
    public int nMoomooMilk = 1;
    public int nMoomooMilkLeft = 1;
    public int moomooMilkRecovery = 50;
    public Animator[] moomooUi;
    public bool drinking;
    public GameObject healEffect;
    [SerializeField] private AudioSource healSound;

    [Space] public bool canRest;
    public bool resting;
    [Space] public bool canEnter;
    public string newSceneName;
    public Vector2 newScenePos;
    [Tooltip("true = moving left\nfalse = moving right")] public bool moveLeftFromDoor; 


    [Space][Header("Items")] 
    public EquipmentUi equipmentUi;
    public Button[] itemButtons;
    public Image[] equippedItems;
    public int nEquipped;
    public int maxWeight=3; 
    public bool canNavigate=true;
    [Space] public bool speedScarf;
    public bool amberNecklace;
    

    //* Powerups
    [Header("Powerups")]
    [SerializeField] private Animator doubleJumpScreen;
    public bool canDoubleJump;
    private int nExtraJumps = 1;
    private int nExtraJumpsLeft = 1;
    [SerializeField] private Transform doubleJumpSpawnPos;
    [Space] public bool canDash;
    public bool canSwim;
    private MusicManager musicManager;
    
    
    [Header("CHEATS")]
    [SerializeField] private float dmgMultiplier = 1;
    [SerializeField] private float expMultiplier = 1;

    private static PlayerControls playerInstance;   // There can only be one

    
         
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        if (playerInstance == null)
            playerInstance = this;
        else 
            Destroy(this.gameObject);
        
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
    }

    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        
        if (musicManager == null && GameObject.Find("Music Manager") != null)
        {
            musicManager = GameObject.Find("Music Manager").GetComponent<MusicManager>();
            StartingMusic();
        }

        // Last save
        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump"))
            canDoubleJump = PlayerPrefsElite.GetBoolean("canDoubleJump");
        if (PlayerPrefsElite.VerifyBoolean("canDash"))
            canDash = PlayerPrefsElite.GetBoolean("canDash");
        if (PlayerPrefsElite.VerifyInt("playerLevel"))
        {
            lv = PlayerPrefsElite.GetInt("playerLevel");
            maxHp += (5 * (lv - 1));
            if (lvText != null)
                 lvText.text = "Lv. " + lv;
        }
        for (int i=1 ; i<lv ; i++)
            expNeeded = (int) (expNeeded * 1.2f);
        if (PlayerPrefsElite.VerifyInt("playerExp"))
            exp = PlayerPrefsElite.GetInt("playerExp");
        if (PlayerPrefsElite.VerifyBoolean("item1") && PlayerPrefsElite.GetBoolean("item1"))
            IncreaseMaxPokemonOut();

        hp = maxHp;

        if (PlayerPrefsElite.VerifyString("checkpointScene"))
            SceneManager.LoadScene(PlayerPrefsElite.GetString("checkpointScene"));
        else 
            PlayerPrefsElite.SetString("checkpointScene", SceneManager.GetActiveScene().name);
        if (PlayerPrefsElite.VerifyVector3("checkpointPos"))
            this.transform.position = PlayerPrefsElite.GetVector3("checkpointPos");
        else 
            PlayerPrefsElite.SetVector3("checkpointPos", this.transform.position + new Vector3(0,0.25f));

        if (PlayerPrefsElite.VerifyArray("roomsBeaten"))
            roomsBeaten = PlayerPrefsElite.GetStringArray("roomsBeaten");
        else 
            roomsBeaten = new string[100];

        
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
        if (PlayerPrefsElite.VerifyArray("buttonAllocatedPokemons"))
        {
            allies = new Ally[6];

            string[] buttonAllocatedPokemons = PlayerPrefsElite.GetStringArray("buttonAllocatedPokemons");
            foreach (string bap in buttonAllocatedPokemons)
                Debug.Log(bap);

            for (int i=0 ; i<buttonAllocatedPokemons.Length ; i++)
            {
                Debug.Log(buttonAllocatedPokemons[i].ToLower());
                switch (buttonAllocatedPokemons[i].ToLower())
                {
                    case "bulbasaur":
                        allies[i] = bulbasaur.summonable;
                        partyPokemonSettings[i].sprite = bulbasaur.sprite;
                        partyPokemonsUI[i].sprite = bulbasaur.sprite;
                        break;
                    case "charmander":
                        allies[i] = charmander.summonable;
                        partyPokemonSettings[i].sprite = charmander.sprite;
                        partyPokemonsUI[i].sprite = charmander.sprite;
                        break;
                    case "squirtle":
                        allies[i] = squirtle.summonable;
                        partyPokemonSettings[i].sprite = squirtle.sprite;
                        partyPokemonsUI[i].sprite = squirtle.sprite;
                        break;
                    case "pidgey":
                        allies[i] = pidgey.summonable;
                        partyPokemonSettings[i].sprite = pidgey.sprite;
                        partyPokemonsUI[i].sprite = pidgey.sprite;
                        break;
                    case "butterfree":
                        allies[i] = butterfree.summonable;
                        partyPokemonSettings[i].sprite = butterfree.sprite;
                        partyPokemonsUI[i].sprite = butterfree.sprite;
                        break;
                    case "":
                        partyPokemonSettings[i].sprite = emptySprite;
                        partyPokemonsUI[i].sprite = emptySprite;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            allies = new Ally[6];
            //* Default pokemon team
            allies[0] = bulbasaur.summonable;
            partyPokemonsUI[0].sprite = bulbasaur.sprite;
            allies[1] = charmander.summonable;
            partyPokemonsUI[1].sprite = charmander.sprite;
            allies[2] = squirtle.summonable;
            partyPokemonsUI[2].sprite = squirtle.sprite;
            allies[3] = null;
            partyPokemonsUI[3].sprite = emptySprite;
            allies[4] = null;
            partyPokemonsUI[4].sprite = emptySprite;
            allies[5] = null;
            partyPokemonsUI[5].sprite = emptySprite;
            SavePokemonTeam();
        }
    
        CheckEquippablePokemon();
        CheckObtainedItems();
    }
    void Update()
    {
        leftWallDetect = Physics2D.OverlapBox(leftWallPos.position, wallDetectBox, 0, whatIsGround);
        rightWallDetect = Physics2D.OverlapBox(rightWallPos.position, wallDetectBox, 0, whatIsGround);
        
        
        if (player.GetButtonDown("START") && !settings.gameObject.activeSelf && 
            !equimentSettings.gameObject.activeSelf && !returningToTitle)
        {
            Time.timeScale = 0;
            inCutscene = true;
            canNavigate = true;
            if (!resting)
                settings.gameObject.SetActive(true);
            else
                equimentSettings.gameObject.SetActive(true);
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
                    partyPokemonFirstSelected.Select();
                    // foreach (ItemUi iu in itemsToActivate)
                    // {
                    //     if (iu.gameObject.activeSelf)
                    //     {
                    //         Debug.Log("**" + iu.button.name);
                    //         iu.button.Select();
                    //         break;
                    //     }
                    // }
                }
                // Items
                else if (player.GetButtonDown("R") && canNavigate)
                {
                    equipmentUi.ChangeTabs(true);
                    foreach (ItemUi iu in itemsToActivate)
                    {
                        if (iu.gameObject.activeSelf)
                        {
                            Debug.Log("**" + iu.button.name);
                            iu.button.Select();
                            break;
                        }
                    }
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
        //* STOP MOOMOO MILK
        else if (drinking)
        {
            if (player.GetButtonUp("L"))
            {
                anim.speed = 1;
                rb.velocity = Vector2.zero;
                anim.SetBool("isDrinking", false);
                drinking = false;
            }
        }
        //* DRINKING MOOMOO MILK
        else if (nMoomooMilkLeft > 0 && hp != maxHp && player.GetButtonDown("L"))
        {
            DrinkingMoomooMilk();
        }
        //* RESTING
        else if (resting)
        {
            if (PressedStandardButton())
                LeaveBench();
        }
        //* Walking, Dashing, Summoning, jumping, Interacting
        else if (hp > 0 && !inCutscene && !dodging)
        {
            if (canRest && Interact())
                RestOnBench();

            if (canEnter && Interact())
                StartCoroutine( EnteringDoor() );

            if (player.GetButtonDown("R"))
                SwitchPokemonSet();

            if (currentItem != null && Interact())
            {
                inCutscene = true;
                currentItem.PickupItem();
                anim.SetTrigger("pickup");
                currentItem = null;
            }
            else if (grounded && canDodge && player.GetButtonDown("ZR") && !canRest && !canEnter && currentItem == null)
            {
                canDodge = false;
                StartCoroutine( Dodge() );
            }


            grounded = Physics2D.OverlapBox(feetPos.position, feetBox, 0, whatIsGround);
            // Touched floor
            if (rb.velocity.y == 0 && grounded)
            {
                anim.SetBool("isGrounded", true);
                anim.SetBool("isFalling", false);
                nExtraJumpsLeft = nExtraJumps;
            }
            
            if (rb.velocity.y < -0.1f && !grounded)
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
                    if (canDoubleJump && nExtraJumpsLeft > 0 && !grounded && player.GetButtonDown("B"))
                    {
                        nExtraJumpsLeft--;
                        MidairJump();
                    }

                    if (player.GetButton("B") && jumping)
                    {
                        if (jumpTimerCounter > 0)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
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
                    rb.velocity = Vector2.right * dashSpeed;
                else
                    rb.velocity = Vector2.left * dashSpeed;
            }


            //* Summon Pokemon
            if (nPokemonOut < maxPokemonOut)
            {
                if (isSet1)
                {
                    if      (canPressButtonWest && player.GetButtonDown("Y"))
                    {
                        if (allies[0] != null && (!inWater || allies[0].aquatic))
                        {
                            nPokemonOut++;
                            var pokemon = Instantiate(allies[0], spawnPos.position, allies[0].transform.rotation);
                            pokemon.atkDmg = (int) (dmgMultiplier * pokemon.atkDmg);
                            pokemon.body.velocity = this.rb.velocity;
                            pokemon.trainer = this;
                            pokemon.button = "Y";

                            PokemonSummoned("Y");
                            
                            //* Looking left
                            if (holder.transform.eulerAngles.y > 0)
                                pokemon.transform.eulerAngles = new Vector3(0,-180);
                        }
                    }
                    else if (canPressButtonNorth && player.GetButtonDown("X"))
                    {
                        if (allies[1] != null && (!inWater || allies[1].aquatic))
                        {
                            nPokemonOut++;
                            var pokemon = Instantiate(allies[1], spawnPos.position, allies[1].transform.rotation);
                            pokemon.atkDmg = (int) (dmgMultiplier * pokemon.atkDmg);
                            pokemon.body.velocity = this.rb.velocity;
                            pokemon.trainer = this;
                            pokemon.button = "X";

                            PokemonSummoned("X");
                            
                            //* Looking left
                            if (holder.transform.eulerAngles.y > 0)
                                pokemon.transform.eulerAngles = new Vector3(0,-180);
                        }
                    }
                    else if (canPressButtonEast && player.GetButtonDown("A"))
                    {
                        if (allies[2] != null && (!inWater || allies[2].aquatic))
                        {
                            nPokemonOut++;
                            var pokemon = Instantiate(allies[2], spawnPos.position, allies[2].transform.rotation);
                            pokemon.atkDmg = (int) (dmgMultiplier * pokemon.atkDmg);
                            pokemon.body.velocity = this.rb.velocity;
                            pokemon.trainer = this;
                            pokemon.button = "A";

                            PokemonSummoned("A");
                            
                            //* Looking left
                            if (holder.transform.eulerAngles.y > 0)
                                pokemon.transform.eulerAngles = new Vector3(0,-180);
                        }
                    }
                }
                else
                {
                    if      (canPressButtonWest2 && player.GetButtonDown("Y"))
                    {
                        if (allies[3] != null && (!inWater || allies[3].aquatic))
                        {
                            nPokemonOut++;
                            var pokemon = Instantiate(allies[3], spawnPos.position, allies[3].transform.rotation);
                            pokemon.atkDmg = (int) (dmgMultiplier * pokemon.atkDmg);
                            pokemon.body.velocity = this.rb.velocity;
                            pokemon.trainer = this;
                            pokemon.button = "Y2";

                            PokemonSummoned("Y2");
                            
                            //* Looking left
                            if (holder.transform.eulerAngles.y > 0)
                                pokemon.transform.eulerAngles = new Vector3(0,-180);
                        }
                    }
                    else if (canPressButtonNorth2 && player.GetButtonDown("X"))
                    {
                        if (allies[4] != null && (!inWater || allies[4].aquatic))
                        {
                            nPokemonOut++;
                            var pokemon = Instantiate(allies[4], spawnPos.position, allies[4].transform.rotation);
                            pokemon.atkDmg = (int) (dmgMultiplier * pokemon.atkDmg);
                            pokemon.body.velocity = this.rb.velocity;
                            pokemon.trainer = this;
                            pokemon.button = "X2";

                            PokemonSummoned("X2");
                            
                            //* Looking left
                            if (holder.transform.eulerAngles.y > 0)
                                pokemon.transform.eulerAngles = new Vector3(0,-180);
                        }
                    }
                    else if (canPressButtonEast2 && player.GetButtonDown("A"))
                    {
                        if (allies[5] != null && (!inWater || allies[5].aquatic))
                        {
                            nPokemonOut++;
                            var pokemon = Instantiate(allies[5], spawnPos.position, allies[5].transform.rotation);
                            pokemon.atkDmg = (int) (dmgMultiplier * pokemon.atkDmg);
                            pokemon.body.velocity = this.rb.velocity;
                            pokemon.trainer = this;
                            pokemon.button = "A2";

                            PokemonSummoned("A2");
                            
                            //* Looking left
                            if (holder.transform.eulerAngles.y > 0)
                                pokemon.transform.eulerAngles = new Vector3(0,-180);
                        }
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
        else if (resting) {}
        else if (hp > 0 && !inCutscene && !dodging && !drinking)
        {
            float xValue = player.GetAxis("Move Horizontal");
            if (inWater && canSwim)
            {
                float yValue = player.GetAxis("Move Vertical");
                rb.velocity = new Vector2(xValue, yValue) * moveSpeed;
            }
            else
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

            //* Flip character
            playerDirection(xValue);
        }
    }
    private void LateUpdate() 
    {
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
        //* Hp
        if (hpImg != null && hpEffectImg != null)
        {
            hpImg.fillAmount = ((float)hp /(float) maxHp);
            
            //* Hp lost Effect
            if (hpEffectImg.fillAmount > hpImg.fillAmount)
                if (hp > 0)
                    hpEffectImg.fillAmount -= effectSpeed;
                else
                    hpEffectImg.fillAmount -= (effectSpeed/4);
            else 
                hpEffectImg.fillAmount = hpImg.fillAmount;
            
            //* Hp color Effect
            if (hpImg.fillAmount <= 0.25f)
                hpImg.color = new Color(0.8f, 0.01f, 0f);
            else if (hpImg.fillAmount <= 0.5f)
                hpImg.color = new Color(1f, 0.65f, 0f);
            else
                hpImg.color = new Color(0f, 0.85f, 0f);
        }
    }
        private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(feetPos.position, feetBox);
        Gizmos.DrawWireCube(leftWallPos.position, wallDetectBox);
        Gizmos.DrawWireCube(rightWallPos.position, wallDetectBox);
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
            xValue = 0;
        if (!receivingKnockback)
            rb.velocity = new Vector2(xValue * moveSpeed, rb.velocity.y);
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
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
    }
    private void MidairJump()
    {
        anim.SetBool("isGrounded", false);
        anim.SetBool("isFalling", false);
        anim.SetTrigger("jump");
        anim.speed = 1;
        jumping = true;
        jumpTimerCounter = jumpTimer;
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);

        // Butterfree jump
        if (doubleJumpObj != null)
        {
            var pokemon = Instantiate(doubleJumpObj, doubleJumpSpawnPos.position, doubleJumpObj.transform.rotation);
            pokemon.transform.SetParent(doubleJumpSpawnPos, true);
            Ally ally = pokemon.GetComponent<Ally>();
            ally.trainer = this;

            //* Looking left
            if (holder.transform.eulerAngles.y > 0)
                pokemon.transform.eulerAngles = new Vector3(0,-180);

        }

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
            rb.velocity = Vector2.left * dashSpeed;
        else
            rb.velocity = Vector2.right * dashSpeed;

        yield return new WaitForSeconds(0.1f);
        dashing = false;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.3f);
        dashes = 1;
    }

    bool PressedStandardButton()
    {
        return (player.GetButtonDown("A") || player.GetButtonDown("B") || player.GetButtonDown("X") || player.GetButtonDown("Y"));
    }

    IEnumerator Dodge()
    {
        anim.speed = 1;
        rb.velocity = Vector2.zero;
        dodging = true;
        canDodge = false;
        anim.SetTrigger("dodge");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        if (holder.transform.eulerAngles.y < 180)   // right
            rb.AddForce(Vector2.right * dodgeSpeed, ForceMode2D.Impulse);
        else    // left
            rb.AddForce(Vector2.left * dodgeSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
        rb.velocity = Vector2.zero;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        dodging = false;
        
        yield return new WaitForSeconds(0.5f);
        canDodge = true;

    }

    public void DrinkingMoomooMilk()
    {
        drinking = true;
        rb.velocity = Vector2.zero;
        anim.speed = 1;
        anim.SetBool("isDrinking", true);
        anim.SetTrigger("drink");
    }
    public void DrankMoomooMilk()
    {
        if (healEffect != null)
            Instantiate(healEffect, this.transform.position + new Vector3(0,1), Quaternion.identity, this.transform);
        drinking = false;
        hp += moomooMilkRecovery;
        nMoomooMilkLeft--;

        if (healSound != null)
            healSound.Play();

        // UI indication
        if (moomooUi != null && nMoomooMilkLeft < moomooUi.Length)
            moomooUi[ nMoomooMilkLeft ].SetTrigger("used");
        
    }
    public void FullRestore()
    {
        hp = maxHp;
    }


    // todo -----------------  D A M A G E  ------------------------------------------------
    public void TakeDamage(int dmg=0, Transform opponent=null, float force=0)
    {
        if (hp > 0 && !inCutscene)
        {
            anim.SetBool("isDrinking", false);
            hp -= dmg;
            if (dmg > 0 && hp > 0)
                StartCoroutine( Flash() );

            if (force > 0 && opponent != null)
            {
                if (hp > 0) StartCoroutine( IgnoreEnemyCollision() );
                StartCoroutine( ApplyKnockback(opponent, force/2) );
            }

            if (hp <= 0)
            {
                StartCoroutine( Died() );
            }
        }
    }
    IEnumerator IgnoreEnemyCollision()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        
        yield return new WaitForSeconds(0.75f);
        if (!dodging)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
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
        if (leftWallDetect)
        {
            Vector2 direction = new Vector2(1,1);
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
        else if (rightWallDetect)
        {
            Vector2 direction = new Vector2(-1,1);
            rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
        else if (!leftWallDetect && !rightWallDetect)
        {
            Vector2 direction = (opponent.position - this.transform.position).normalized;
            direction = new Vector2(direction.x,-1);
            rb.AddForce(-direction * force, ForceMode2D.Impulse);
        }
        
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector2.zero;
        receivingKnockback = false;
    }


    public void GainExp(int expGained, int enemyLevel)
    {
        if (lv < 100)
            exp += (int) ((expGained * Mathf.Min(3f, (float) enemyLevel / lv)) * expMultiplier);
        else
            exp = 0;
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
        maxHp += 5;
        hp += 5;

        if (lvText != null)
             lvText.text = "Lv. " + lv;
        
        if (lv >= 100)
            exp = 0;
        
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  C U T S C E N E  ------------------------------------------------
    IEnumerator Died()
    {
        rb.velocity = Vector2.zero;
        anim.SetTrigger("died");
        
        // if (musicManager != null)
        //     StartCoroutine( musicManager.LowerMusic(musicManager.currentMusic, 0.75f) );
        
        
        // Time.timeScale = 0.25f;
        float origGravity = rb.gravityScale;
        rb.gravityScale = 0;
        if (col != null)
            col.enabled = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);


        yield return new WaitForSeconds(2f);
        if (transitionAnim != null)
            transitionAnim.SetTrigger("toBlack");
        if (musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.previousMusic) );
        
        yield return new WaitForSeconds(1f);
        StartCoroutine( Respawn() );
        rb.gravityScale = origGravity;
        if (col != null)
            col.enabled = true;
        // Time.timeScale = 1;
        // ReturnToTitle();
        // yield return new WaitForSeconds(1f);
    }
    IEnumerator Respawn()
    {
        if (musicManager != null && musicManager.previousMusic != null)
        {
            // StartCoroutine( musicManager.StopMusic(musicManager.currentMusic) );
            yield return new WaitForSeconds(0.5f);
            // musicManager.StartMusic(musicManager.previousMusic);
        }
            
        ReloadState();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        hp = maxHp;
        anim.SetTrigger("reset");

        if (transitionAnim != null)
            transitionAnim.SetTrigger("fromBlack");
    }

    public void SetNextArea(string nextArea, Vector2 newPos, bool exitingDoor=false)
    {
        if (!inCutscene)
        {
            if (dodging)
                dodgingThruScene = true;
            else
                dodgingThruScene = false;
            
            if (transitionAnim != null)
                transitionAnim.SetTrigger("toBlack");
            StartCoroutine( MovingToNextArea(nextArea, newPos, exitingDoor) );
        }
    }
    public IEnumerator MovingToNextArea(string nextArea, Vector2 newPos, bool exitingDoor)
    {
        if (!inCutscene)
        {
            inCutscene = true;
            bool walkingRight = (rb.velocity.x > 0);

            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(nextArea);
            rb.velocity = Vector2.zero;

            yield return new WaitForEndOfFrame();
            this.transform.position = newPos;

            yield return new WaitForSeconds(0.1f);
            if (transitionAnim != null)
                transitionAnim.SetTrigger("fromBlack");
            
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

        rb.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        inCutscene = false;
        canEnter = false;
        rb.velocity = Vector2.zero;
    }
    IEnumerator WalkingLeft()  // Moving left
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isFalling", false);
        anim.SetTrigger("reset");
        
        if (dodgingThruScene)
            anim.SetTrigger("dodge");

        rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        inCutscene = false;
        canEnter = false;
        rb.velocity = Vector2.zero;
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
        rb.velocity = Vector2.zero;
    }

    public IEnumerator EnteringDoor()
    {
        if (!inCutscene)
        {
            rb.velocity = Vector2.zero;
            inCutscene = true;

            if (transitionAnim != null)
                transitionAnim.SetTrigger("toBlack");

            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(newSceneName);
            rb.velocity = Vector2.zero;

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
        if (rewiredInputSystem != null)
            Destroy(rewiredInputSystem);
        
        Time.timeScale = 1;
        settings.gameObject.SetActive(false);
        StartCoroutine(ReturningToTitleScreen());
    }
    IEnumerator ReturningToTitleScreen()
    {
        if (transitionAnim != null)
            transitionAnim.SetTrigger("toBlack");

        if (playerUi != null)
            playerUi.SetActive(false);
        playerInstance = null;

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("0Title");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        
        if (musicManager != null)
            musicManager.BackToTitle();

        yield return new WaitForSeconds(0.5f);
        if (transitionAnim != null)
            transitionAnim.SetTrigger("fromBlack");
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  T R I G G E R S  ------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Underwater"))
            inWater = true;
        // if (other.CompareTag("Roar"))
        //     EngagedBossRoar();
        if (other.CompareTag("Rage") && musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.bossOutroMusic) );
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Underwater"))
        {
            inWater = false;
            nExtraJumpsLeft = nExtraJumps;
        }
        if (other.CompareTag("Roar"))
            RoarOver();
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  M U S I C  ------------------------------------------------------
    
    public void StartingMusic(string location="forest")
    {
        if (musicManager != null)
        {
            switch (location)
            {
                case "forest":
                    StartCoroutine( musicManager.TransitionMusic(musicManager.forestMusic) );
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
        inCutscene = true;
        anim.speed = 1;
        anim.SetBool("inBossFight", true);
        anim.SetTrigger("engaged");
        rb.velocity = Vector2.zero;

        if (musicManager != null && musicName.ToLower() == "rosary")
            StartCoroutine( musicManager.TransitionMusic(musicManager.bossIntroMusic, true) );
    }
    
    public void RoarOver()
    {
        anim.SetBool("inBossFight", false);
        inCutscene = false;
        rb.velocity = Vector2.zero;
    }

    // todo ------------------------------------------------------------------------------------

    public void SelectAllyToReplace(string buttonSymbol, Button lastButton)
    {
        oldButtonSymbol = buttonSymbol;
        if (boxPokemonFirstSelected != null)
            boxPokemonFirstSelected.Select();
        partyPokemonLastSelected = lastButton;
    }

    public void SetNewAlly(Ally newAlly, Sprite newSprite)
    {
        //* If the pokemon is already assigned in another pokemon, then remove assigned
        if      (allies[0] == newAlly) { allies[0] = null; partyPokemonsUI[0].sprite = emptySprite; partyPokemonSettings[0].sprite = emptySprite; }
        else if (allies[1] == newAlly) { allies[1] = null; partyPokemonsUI[1].sprite = emptySprite; partyPokemonSettings[1].sprite = emptySprite; }
        else if (allies[2] == newAlly) { allies[2] = null; partyPokemonsUI[2].sprite = emptySprite; partyPokemonSettings[2].sprite = emptySprite; }
        else if (allies[3] == newAlly) { allies[3] = null; partyPokemonsUI[3].sprite = emptySprite; partyPokemonSettings[3].sprite = emptySprite; }
        else if (allies[4] == newAlly) { allies[4] = null; partyPokemonsUI[4].sprite = emptySprite; partyPokemonSettings[4].sprite = emptySprite; }
        else if (allies[5] == newAlly) { allies[5] = null; partyPokemonsUI[5].sprite = emptySprite; partyPokemonSettings[5].sprite = emptySprite; }


        switch (oldButtonSymbol.ToUpper())
        {
            case "Y1": allies[0] = newAlly; partyPokemonsUI[0].sprite = newSprite; partyPokemonSettings[0].sprite = newSprite; break;
            case "X1": allies[1] = newAlly; partyPokemonsUI[1].sprite = newSprite; partyPokemonSettings[1].sprite = newSprite; break;
            case "A1": allies[2] = newAlly; partyPokemonsUI[2].sprite = newSprite; partyPokemonSettings[2].sprite = newSprite; break;
            case "Y2": allies[3] = newAlly; partyPokemonsUI[3].sprite = newSprite; partyPokemonSettings[3].sprite = newSprite; break;
            case "X2": allies[4] = newAlly; partyPokemonsUI[4].sprite = newSprite; partyPokemonSettings[4].sprite = newSprite; break;
            case "A2": allies[5] = newAlly; partyPokemonsUI[5].sprite = newSprite; partyPokemonSettings[5].sprite = newSprite; break;
        }

        if (partyPokemonLastSelected != null)
            partyPokemonLastSelected.Select();
        else if (partyPokemonFirstSelected != null)
            partyPokemonFirstSelected.Select();
    }


    public void GainPowerup(string powerupName)
    {
        Debug.Log("Gained a power = " + powerupName);

        switch (powerupName)
        {
            case "butterfree": 
                canDoubleJump = true;
                PlayerPrefsElite.SetBoolean("canDoubleJump", canDoubleJump);
                inCutscene = true;
                doubleJumpScreen.gameObject.SetActive(true);
                break;
            case "pidgey": 
                CaughtAPokemon("pidgey");
                // inCutscene = true;
                // doubleJumpScreen.gameObject.SetActive(true);
                break;
            default:
                Debug.LogError("PlayerControls.GainPowerup - unregistered powerup (ADD TO SWITCH CASE)");
                break;
        }
        rb.velocity = Vector2.zero;
    }

    public void GainItem(string itemTypeName)
    {
        if (PlayerPrefsElite.VerifyArray("itemsObtained"))
            itemsObtained = PlayerPrefsElite.GetStringArray("itemsObtained");
        else
        {
            PlayerPrefsElite.SetStringArray("itemsObtained", new string[50]);
            itemsObtained = PlayerPrefsElite.GetStringArray("itemsObtained");
        }

        for (int i=0 ; i<itemsObtained.Length ; i++)
        {
            if (itemsObtained[i] == "")
            {
                Debug.Log("item slot " + i + " is set to " + itemTypeName);
                itemsObtained[i] = itemTypeName;
                break;
            }
        }
        PlayerPrefsElite.SetStringArray("itemsObtained", itemsObtained);
        CheckObtainedItems();
    }
    
    public void EquipItem(Sprite itemSprite)
    {
        equippedItems[nEquipped].sprite = itemSprite;
        nEquipped++;
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
        // equippedItems[nEquipped].sprite = itemSprite;
        nEquipped--;
    }

    public void IncreaseMaxPokemonOut()
    {
        maxPokemonOut++;
    }

    public void EXIT_EQUIPMENT_MENU()
    {
        //
        equimentSettings.SetTrigger("close");
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
        rb.velocity = Vector2.zero;
        resting = true;
        FullRestore();
        anim.speed = 1;
        anim.SetTrigger("rest");
        anim.SetBool("isResting", true);
        if (moomooUi != null)
        {
            for (int i=nMoomooMilkLeft ; i<nMoomooMilk ; i++)
                moomooUi[i].SetTrigger("restored");
        }
        nMoomooMilkLeft = nMoomooMilk;
        // UI indication

        SaveState();
    }

    public void LeaveBench()
    {
        resting = false;
        anim.SetBool("isResting", false);

        SavePokemonTeam();
    }

    // todo ------------------------------------------------------------------------------------
    // todo -----------------  P R E F S  ------------------------------------------------------

    public void ReloadState()
    {
        nMoomooMilkLeft = nMoomooMilk;
        // UI indication
        if (moomooUi != null)
        {
            foreach (Animator ui in moomooUi)
                ui.SetTrigger("restored");
        }


        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump"))
            canDoubleJump = PlayerPrefsElite.GetBoolean("canDoubleJump");
        if (PlayerPrefsElite.VerifyBoolean("canDash"))
            canDash = PlayerPrefsElite.GetBoolean("canDash");
        if (PlayerPrefsElite.VerifyInt("playerLevel"))
        {
            lv = PlayerPrefsElite.GetInt("playerLevel");
            maxHp += (5 * (lv - 1));
            if (lvText != null)
                 lvText.text = "Lv. " + lv;
        }
        for (int i=1 ; i<lv ; i++)
            expNeeded = (int) (expNeeded * 1.2f);
        if (PlayerPrefsElite.VerifyInt("playerExp"))
            exp = PlayerPrefsElite.GetInt("playerExp");
        if (PlayerPrefsElite.VerifyBoolean("item1") && PlayerPrefsElite.GetBoolean("item1"))
            IncreaseMaxPokemonOut();

        hp = maxHp;
        if (PlayerPrefsElite.VerifyString("checkpointScene") && PlayerPrefsElite.VerifyVector3("checkpointPos"))
        {
            SceneManager.LoadScene(PlayerPrefsElite.GetString("checkpointScene"));
            this.transform.position = PlayerPrefsElite.GetVector3("checkpointPos");
        }
    }
    public void SaveState()
    {
        PlayerPrefsElite.SetString("checkpointScene", SceneManager.GetActiveScene().name);
        PlayerPrefsElite.SetVector3("checkpointPos", this.transform.position + new Vector3(0,0.25f));
        PlayerPrefsElite.SetInt("playerExp", exp);
        PlayerPrefsElite.SetInt("playerLevel", lv);
        PlayerPrefsElite.SetBoolean("canDoubleJump", canDoubleJump);
        PlayerPrefsElite.SetBoolean("canDash", canDash);
        PlayerPrefsElite.SetStringArray("roomsBeaten", roomsBeaten);
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
        PlayerPrefsElite.SetStringArray("buttonAllocatedPokemons", buttonAllocatedPokemons);
    }

    public void CaughtAPokemon(string pokemonName)
    {
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught"))
        {
            pokemonsCaught = PlayerPrefsElite.GetStringArray("pokemonsCaught");
            for (int i=0 ; i<pokemonsCaught.Length ; i++)
            {
                if (pokemonsCaught[i] == "")
                {
                    pokemonsCaught[i] = pokemonName;
                    break;
                }
            }
            PlayerPrefsElite.SetStringArray("pokemonsCaught", pokemonsCaught);
            var set = new HashSet<string>(pokemonsCaught);
            foreach (BoxPokemonButton boxPokemon in boxPokemonsToActivate)
            {
                if (set.Contains(boxPokemon.pokemonName))
                    boxPokemon.gameObject.SetActive(true);
            }
            CheckEquippablePokemon();
        }
    }
    public void CheckEquippablePokemon()
    {
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught"))
        {
            pokemonsCaught = PlayerPrefsElite.GetStringArray("pokemonsCaught");
            var set = new HashSet<string>(pokemonsCaught);
            foreach (BoxPokemonButton boxPokemon in boxPokemonsToActivate)
            {
                if (set.Contains(boxPokemon.pokemonName))
                    boxPokemon.gameObject.SetActive(true);
            }
        }
        else
        {
            PlayerPrefsElite.SetStringArray("pokemonsCaught", new string[100]);
        }
    }
    
    //* Set all obtained items gameObject (buttons) active - Start(), GainItem()
    public void CheckObtainedItems()
    {
        if (PlayerPrefsElite.VerifyArray("itemsObtained"))
        {
            itemsObtained = PlayerPrefsElite.GetStringArray("itemsObtained");
            var set = new HashSet<string>(itemsObtained);
            foreach (ItemUi heldItem in itemsToActivate)
            {
                if (set.Contains(heldItem.itemName))
                    heldItem.gameObject.SetActive(true);
            }

        }
        else
        {
            PlayerPrefsElite.SetStringArray("itemsObtained", new string[50]);
        }
    }

    public void AddRoomBeaten(string roomName)
    {
        // roomsBeaten = PlayerPrefsElite.GetStringArray("roomsBeaten");
        // foreach(string r in roomsBeaten)
        //     Debug.Log("~" + r + "~");
        // for (int i=0 ; i < roomsBeaten.Length ; i++)
        // {
        //     if (roomsBeaten[i] == "")
        //     {
        //         roomsBeaten[i] = roomName;
        //         Debug.LogError("Added room " + roomName + " at " + i);
        //         break;
        //     }
        // }
        // PlayerPrefsElite.SetStringArray("roomsBeaten", roomsBeaten);
    }

    // todo ------------------------------------------------------------------------------------


    void PokemonSummoned(string button)
    {
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
    
    public void AllPokemonReturned()
    {
        nPokemonOut = 0;

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

}



[System.Serializable]
public class AllyTeamUI
{
    public Ally summonable;
    public Sprite sprite;
}