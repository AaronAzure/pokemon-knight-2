using System.Collections;
// using System.Collections.Generic;
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
    
    [Space] 
    public Image pokeballY1;
    public Image pokeballX1;
    public Image pokeballA1;
    public Image pokemonY1;
    public Image pokemonX1;
    public Image pokemonA1;

    [Space] 
    public Image pokeballY2;
    public Image pokeballX2;
    public Image pokeballA2;
    public Image pokemonY2;
    public Image pokemonX2;
    public Image pokemonA2;

    
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


    [Space] [Header("Platformer Mechanics")]
    [SerializeField] private Rigidbody2D rb;
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
    private bool receivingKnockback;
    private int dashes = 1;
    private bool dashing;
    private bool canPressButtonNorth = true;   // (X)
    private bool canPressButtonWest = true;    // (Y)
    private bool canPressButtonEast = true;    // (A)
    private bool canPressButtonNorth2 = true;   // (X)
    private bool canPressButtonWest2 = true;    // (Y)
    private bool canPressButtonEast2 = true;    // (A)
    private int nPokemonOut;
    private int maxPokemonOut = 1;
    private float localX;
    [SerializeField] private GameObject holder;
    [SerializeField] private Animator anim;
    private bool inCutscene;


    //* Powerups
    [Header("Powerups")]
    public bool canDoubleJump;
    private int nExtraJumps = 1;
    private int nExtraJumpsLeft = 1;
    [SerializeField] private Transform doubleJumpSpawnPos;
    public bool canDash;
    private MusicManager musicManager;
    // private bool groundedDouble = true;


    [Header("Pokemon (Allies)")]
    [SerializeField] private Transform spawnPos;    // Place to Summon Pokemon
    [SerializeField] private GameObject bulbasaur;
    [Space][SerializeField] private GameObject squirtle;
    [Space][SerializeField] private GameObject charmander;
    [Space][SerializeField] private GameObject doubleJumpObj;
    [SerializeField] private float dmgMultiplier = 1;


    
    private static PlayerControls playerInstance;   // There can only be one
         
    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        if (playerInstance == null)
            playerInstance = this;
        else 
            Destroy(this.gameObject);
        
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        localX = holder.transform.localScale.x;
    }

    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        hp = maxHp;
        
        if (transitionAnim != null)
            transitionAnim.SetTrigger("fromBlack");
        
        if (musicManager == null)
        {
            musicManager = GameObject.Find("Music Manager").GetComponent<MusicManager>();
            StartingMusic();
        }

        canDoubleJump = PlayerPrefsElite.GetBoolean("canDoubleJump");
        canDash = PlayerPrefsElite.GetBoolean("canDash");
    }
    void Update()
    {
        if (hp > 0 && !inCutscene)
        {
            float xValue = player.GetAxis("Move Horizontal");
            Walk(xValue);

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
                //* Flip character
                playerDirection(xValue);

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
                if (canPressButtonWest && player.GetButtonDown("Y"))
                {
                    if (bulbasaur != null)
                    {
                        nPokemonOut++;
                        var pokemon = Instantiate(bulbasaur, spawnPos.position, bulbasaur.transform.rotation);
                        Ally ally = pokemon.GetComponent<Ally>();
                        ally.atkDmg = (int) (dmgMultiplier * ally.atkDmg);
                        // ally.body.velocity = Vector2.up * this.rb.velocity.y;
                        ally.body.velocity = this.rb.velocity;
                        ally.trainer = this;
                        ally.button = "Y";

                        PokemonSummoned("Y");
                        // StartCoroutine( PokemonYCooldown(ally.outTime, ally.resummonTime) );
                        
                        //* Looking left
                        if (holder.transform.eulerAngles.y > 0)
                            pokemon.transform.eulerAngles = new Vector3(0,-180);
                    }
                }
                if (canPressButtonEast && player.GetButtonDown("A"))
                {
                    if (squirtle != null)
                    {
                        nPokemonOut++;
                        var pokemon = Instantiate(squirtle, spawnPos.position, squirtle.transform.rotation);
                        Ally ally = pokemon.GetComponent<Ally>();
                        ally.atkDmg = (int) (dmgMultiplier * ally.atkDmg);
                        // ally.body.velocity = Vector2.up * this.rb.velocity.y;
                        ally.body.velocity = this.rb.velocity;
                        ally.trainer = this;
                        ally.button = "A";

                        PokemonSummoned("A");
                        // StartCoroutine( PokemonACooldown(ally.outTime, ally.resummonTime) );
                        
                        //* Looking left
                        if (holder.transform.eulerAngles.y > 0)
                            pokemon.transform.eulerAngles = new Vector3(0,-180);
                    }
                }
                if (canPressButtonNorth && player.GetButtonDown("X"))
                {
                    if (charmander != null)
                    {
                        nPokemonOut++;
                        var pokemon = Instantiate(charmander, spawnPos.position, charmander.transform.rotation);
                        Ally ally = pokemon.GetComponent<Ally>();
                        ally.atkDmg = (int) (dmgMultiplier * ally.atkDmg);
                        // ally.body.velocity = Vector2.up * this.rb.velocity.y;
                        ally.body.velocity = this.rb.velocity;
                        ally.trainer = this;
                        ally.button = "X";

                        PokemonSummoned("X");
                        // StartCoroutine( PokemonXCooldown(ally.outTime, ally.resummonTime) );
                        
                        //* Looking left
                        if (holder.transform.eulerAngles.y > 0)
                            pokemon.transform.eulerAngles = new Vector3(0,-180);
                    }
                }
            }
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
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(feetPos.position, feetBox);
    }

    private void Walk(float xValue)
    {
        if (Mathf.Abs(xValue) < 0.1f)
            xValue = 0;
        if (!receivingKnockback)
            rb.velocity = new Vector2(xValue * moveSpeed, rb.velocity.y);
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
            // var pokemon = Instantiate(doubleJumpObj, doubleJumpSpawnPos.position, doubleJumpObj.transform.rotation, holder.transform);
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
    private void playerDirection(float xValue=0)
    {
        if (xValue < -0.01f)
            holder.transform.eulerAngles = new Vector3(0,180);
        else if (xValue > 0.01f)
            holder.transform.eulerAngles = new Vector3(0,0);
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


    public void TakeDamage(int dmg=0, Transform opponent=null, float force=0)
    {
        if (hp > 0)
        {
            StartCoroutine( IgnoreEnemyCollision() );
            
            hp -= dmg;
            if (dmg > 0 && hp > 0)
                StartCoroutine( Flash() );

            if (force > 0 && opponent != null)
                StartCoroutine( ApplyKnockback(opponent, force) );

            if (hp <= 0)
            {
                StartCoroutine( Died() );
            }
        }
    }

    IEnumerator IgnoreEnemyCollision()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        
        yield return new WaitForSeconds(0.75f);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        
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
        direction = new Vector2(direction.x,-1);
        rb.AddForce(-direction * force, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        receivingKnockback = false;
    }


    public void GainExp(int expGained, int enemyLevel)
    {
        // exp += (int) (expGained);
        exp += (int) (expGained * Mathf.Min(3f, enemyLevel / lv));
    }

    void LevelUp()
    {
        levelUpObj.SetActive(false);
        expEffectImg.fillAmount = 0;
        expImg.fillAmount = 0;
        levelUpObj.SetActive(true);
        lv++;
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
            lvText.text = "Lv " + lv;
    }

    IEnumerator Died()
    {
        rb.velocity = Vector2.zero;
        anim.SetTrigger("died");
        Time.timeScale = 0.25f;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

        yield return new WaitForSeconds(0.4f);
        if (transitionAnim != null)
            transitionAnim.SetTrigger("toBlack");
    }

    public void SetNextArea(string nextArea, float xPos, float yPos, bool walkLeft)
    {
        if (!inCutscene)
        {
            if (transitionAnim != null)
                transitionAnim.SetTrigger("toBlack");
            StartCoroutine( MovingToNextArea(nextArea, xPos, yPos, walkLeft) );
        }
    }
    public IEnumerator MovingToNextArea(string nextArea, float xPos, float yPos, bool walkLeft)
    {
        if (!inCutscene)
        {
            inCutscene = true;
            bool walkingRight = (rb.velocity.x > 0);

            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(nextArea);
            rb.velocity = Vector2.zero;

            yield return new WaitForEndOfFrame();
            this.transform.position = new Vector3(xPos,yPos);

            yield return new WaitForSeconds(0.1f);
            if (transitionAnim != null)
                transitionAnim.SetTrigger("fromBlack");
            
            yield return new WaitForSeconds(0.4f);

            if (!walkingRight)
                StartCoroutine( WalkingLeft() );
            else
                StartCoroutine( WalkingRight() );
        }
    }

    IEnumerator WalkingRight()  // Moving right
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isFalling", false);

        rb.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        inCutscene = false;
        rb.velocity = Vector2.zero;
    }
    IEnumerator WalkingLeft()  // Moving left
    {
        yield return new WaitForEndOfFrame();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isFalling", false);

        rb.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        inCutscene = false;
        rb.velocity = Vector2.zero;
    }

    // todo ------------------------------------------------------------------------------------
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Roar"))
            EngagedBossRoar();
        if (other.CompareTag("Rage") && musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.bossOutroMusic) );;
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
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
            StartCoroutine( musicManager.LowerMusic(musicManager.currentMusic) );
    }
    
    
    // todo ------------------------------------------------------------------------------------

    public void EngagedBossRoar()
    {
        inCutscene = true;
        anim.speed = 1;
        anim.SetBool("inBossFight", true);
        anim.SetTrigger("engaged");
        rb.velocity = Vector2.zero;

        if (musicManager != null)
            StartCoroutine( musicManager.TransitionMusic(musicManager.bossIntroMusic, true) );
    }
    
    public void RoarOver()
    {
        anim.SetBool("inBossFight", false);
        inCutscene = false;
        rb.velocity = Vector2.zero;
    }


    public void GainPowerup(string powerupName)
    {
        switch (powerupName)
        {
            case "butterfree": 
                canDoubleJump = true;
                PlayerPrefsElite.SetBoolean("canDoubleJump", true);
                break;
            default:
                Debug.LogError("PlayerControls.GainPowerup - unregistered powerup (ADD TO SWITCH CASE)");
                break;
        }
        musicManager.StartMusic(musicManager.previousMusic);
    }

    void PokemonSummoned(string button)
    {
        switch (button.ToUpper())
        {
            case "Y":
                pokemonY1.color = new Color(0.3f,0.3f,0.3f);
                canPressButtonWest = false;
                pokeballY1.fillAmount = 0;
                break;
            case "X":
                pokemonX1.color = new Color(0.3f,0.3f,0.3f);
                canPressButtonNorth = false;
                pokeballX1.fillAmount = 0;
                break;
            case "A":
                pokemonA1.color = new Color(0.3f,0.3f,0.3f);
                canPressButtonEast = false;
                pokeballA1.fillAmount = 0;
                break;
            case "Y2":
                pokemonY2.color = new Color(0.3f,0.3f,0.3f);
                canPressButtonWest2 = false;
                pokeballY2.fillAmount = 0;
                break;
            case "X2":
                pokemonX2.color = new Color(0.3f,0.3f,0.3f);
                canPressButtonNorth2 = false;
                pokeballX2.fillAmount = 0;
                break;
            case "A2":
                pokemonA2.color = new Color(0.3f,0.3f,0.3f);
                canPressButtonEast2 = false;
                pokeballA2.fillAmount = 0;
                break;

            default:
                Debug.LogError("Not a registered (Wrong) Button");
                break;
        }
    }
    public void PokemonReturned(string button, float coolDown)
    {
        if (button == "")
            return;
        StartCoroutine( PokemonCooldown(button.ToUpper(), coolDown) );
    }
    IEnumerator PokemonCooldown(string button, float cooldown) //* NORTH
    {
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
                pokemonY1.color = new Color(1,1,1);
                break;

            case "X":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballX1.fillAmount += amount;
                }
                canPressButtonNorth = true;
                pokemonX1.color = new Color(1,1,1);
                break;

            case "A":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballA1.fillAmount += amount;
                }
                canPressButtonEast = true;
                pokemonA1.color = new Color(1,1,1);
                break;

            case "Y2":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballY2.fillAmount += amount;
                }
                canPressButtonWest2 = true;
                pokemonY2.color = new Color(1,1,1);
                break;

            case "X2":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballX2.fillAmount += amount;
                }
                canPressButtonNorth2 = true;
                pokemonX2.color = new Color(1,1,1);
                break;

            case "A2":
                for (int i=0 ; i<repeat ; i++)
                {
                    yield return new WaitForSeconds(s);
                    pokeballA2.fillAmount += amount;
                }
                canPressButtonEast2 = true;
                pokemonA2.color = new Color(1,1,1);
                break;

            default:
                Debug.LogError("PokemonCooldown() - Unregister button (" + button + ")");
                break;
        }

    }


}
