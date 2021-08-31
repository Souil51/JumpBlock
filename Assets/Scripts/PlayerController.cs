using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Slider = UnityEngine.UI.Slider;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    /*Public prop*/
    public int speed;
    public LayerMask groundLayer;
    public LayerMask fluidLayer;
    public GameController GameControllerGO;
    private Rigidbody2D rb2d;

    /*Config*/
    private float m_fOffsetDetection_X = 0.1f;
    private float m_fOffsetDetection_Y = 0.05f;

    /*States*/
    private bool m_bLastGroundedState = false;
    private bool m_bLastFluidState = false;

    /*Slider Noyade*/
    public GameObject slider;
    public Camera cam;
    public Canvas canvas;
    public float sliderValue;
    public float maxTime = 3.0f;
    public float frequenceSlider = 0.1f;

    /*Animations*/
    public Animator playerAnimator;

    bool m_bHitTheGroundEnabled = true;

    /*Sounds*/
    public AudioClip jumpSound;
    public AudioClip waterSound;

    /*Particules*/
    public ParticleSystem jumpParticles;
    public ParticleSystem waterJumpParticles;

    /*Autres*/
    private IEnumerator coroutine;
    private Vector3 m_vPositionTP;

    private string m_szTagRight = "";//Mis à jour à chaque début d'update pour pouvoir utiliser le tag du block en dessous à droite à différents endroits sans devoir refaire la méthode GetTagBelow
    private string m_szTagLeft = "";//Mis à jour à chaque début d'update pour pouvoir utiliser le tag du block en dessous à gauche à différent endroit sans devoir refaire la méthode GetTagBelow

    private bool m_bHasKey = false;

    private bool m_bReverseGravityAnim = false;

    private string HorizontalAxis = "";
    private string VerticalAxis = "";

    // Start is called before the first frame update
    void Start()
    {
        GameObject goInput = GameObject.FindGameObjectWithTag("InputController");
        InputController inputCtrl = goInput.GetComponent<InputController>();

        HorizontalAxis = inputCtrl.GetHorizontalAxis();
        VerticalAxis = inputCtrl.GetVerticalAxis();

        rb2d = transform.GetComponent<Rigidbody2D>();

        coroutine = WaitAndTest();
        StartCoroutine(coroutine);
    }

    // Update is called once per frame
    void Update()
    {
        m_szTagRight = GetTagBelowRightBound();
        m_szTagLeft = GetTagBelowLeftBound();

        if (GameController.IsPaused || GameController.IsDead) return;

        if (m_szTagRight == "VictoryBlock" || m_szTagLeft == "VictoryBlock")
            GameControllerGO.FinishLevel();

        float moveHorizontal = Input.GetAxisRaw(HorizontalAxis);
        float moveVertical = Input.GetAxisRaw(VerticalAxis);

        if (moveHorizontal > 0)
            rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
        else if (moveHorizontal < 0)
            rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
        else
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);

        bool bGrounded = isGrounded();
        bool bIsInFluid = isInFluid();
        bool bObjectTop = HasObjectTop(0.5f);

        if (!m_bLastFluidState && bIsInFluid)
        {
            PlayerEnterWater();
            ShowSliderOnPlayer();
        }
        
        if(m_bLastFluidState && !bIsInFluid)
        {
            HideSliderOnPlayer();
        }

        if (!m_bLastGroundedState && bGrounded && m_bHitTheGroundEnabled && !bIsInFluid)
        {
            PlayerHitTheGround();
        }

        if (moveVertical > 0 && bGrounded && !bObjectTop)
        {
            if(rb2d.gravityScale > 0)
                rb2d.velocity = new Vector2(rb2d.velocity.x, Vector2.up.y * speed * 3);
            else
                rb2d.velocity = new Vector2(rb2d.velocity.x, Vector2.down.y * speed * 3);
        }

        m_bLastGroundedState = bGrounded;
        m_bLastFluidState = bIsInFluid;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!HasObjectBelow(1.1f) && GameControllerGO.GetCapacityBlockLeft() > 0)
            {
                GameObject blockGO = (GameObject)Instantiate(Resources.Load("block"));

                Vector3 vOffset = new Vector3(0, 1.05f, 0);
                blockGO.transform.position = this.transform.position - vOffset;

                GameControllerGO.CapacityBlockUse();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject goTP = null;

            if (m_szTagLeft == "TeleportBlock")
                goTP = GetGOBelowLeftBound();
            else if (m_szTagRight == "TeleportBlock")
                goTP = GetGOBelowRightBound();

            if(goTP != null)
            {
                int tpIndex = goTP.GetComponent<TeleportBlockController>().BlockIndex;

                //On récupère l'autre block de TP
                GameObject[] goTPBlocks = GameObject.FindGameObjectsWithTag("TeleportBlock");

                List<GameObject> goOtherTP = goTPBlocks.Where(goSearch =>
                {
                    int localBlockIndex = goSearch.GetComponent<TeleportBlockController>().BlockIndex;
                    return localBlockIndex == tpIndex && (goSearch.transform.position.x != goTP.transform.position.x || goSearch.transform.position.y != goTP.transform.position.y);
                }).ToList<GameObject>();

                if(goOtherTP.Count > 0)
                {
                    Vector3 finalPosition = goOtherTP[0].transform.position;
                    finalPosition.y += 1.1f;

                    m_vPositionTP = finalPosition;

                    BeginDisappearAnimation();
                }
            }
        }

        //CheckPoint
        GameObject goCheckPoint = null;
        if (m_szTagRight == "CheckpointBlock")
        {
            goCheckPoint = GetGOBelowRightBound();
        }
        else if(m_szTagLeft == "CheckpointBlock")
        {
            goCheckPoint = GetGOBelowLeftBound();
        }

        if(goCheckPoint != null)
        {
            GameControllerGO.UpdateLastCheckpoint(goCheckPoint);
        }

        if (m_szTagRight == "KillBlock" || m_szTagLeft == "KillBlock" || transform.position.y < GameControllerGO.m_yDeath || transform.position.y > GameControllerGO.m_xDeath)
        {
            GameControllerGO.PlayerDied();
        }

        UpdateSliderPosition();

        if (Input.GetKeyDown(KeyCode.T) && GameControllerGO.IsGravityAllowed())
            ReverseGravity();
    }

    public void ChangeKeyboardLayout(string horizon, string vertical)
    {
        HorizontalAxis = horizon;
        VerticalAxis = vertical;
    }

    public bool isGrounded()
    {
        Vector2 positionRight = transform.position;
        positionRight.x += GetComponent<SpriteRenderer>().bounds.size.x / 2;
        positionRight.x -= m_fOffsetDetection_X;

       Vector2 positionLeft = transform.position;
        positionLeft.x -= GetComponent<SpriteRenderer>().bounds.size.x / 2;
        positionLeft.x += m_fOffsetDetection_X;

        Vector2 direction = rb2d.gravityScale > 0 ? Vector2.down : Vector2.up;
        float distance = GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hitRight = Physics2D.Raycast(positionRight, direction, distance, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(positionLeft, direction, distance, groundLayer);

        bool bRes = false;

        if (hitRight.collider != null || hitLeft.collider != null)
        {
            bRes = true;
        }

        return bRes;
    }

    public bool isInFluid()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.up;

        float distance = 0.0f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, fluidLayer);
        RaycastHit2D hit_2 = Physics2D.Raycast(position, direction, 1.0f, fluidLayer);

        //Debug.DrawLine(position, position + new Vector2(0, 0.2f), Color.green, 2f);

        if ((hit.collider != null && hit_2.collider == null) || (hit.collider == null && hit_2.collider != null))
        {
            return true;
        }

        return false;
    }

    public string GetTagBelowRightBound()
    {
        Vector2 position = transform.position;
        position.x += GetComponent<SpriteRenderer>().bounds.size.x / 2;
        position.x -= m_fOffsetDetection_X;

        Vector2 direction = rb2d.gravityScale > 0 ? Vector2.down : Vector2.up;
        float distance = GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return hit.transform.gameObject.tag;
        }

        return "";
    }

    public string GetTagBelowLeftBound()
    {
        Vector2 position = transform.position;
        position.x -= GetComponent<SpriteRenderer>().bounds.size.x / 2;
        position.x += m_fOffsetDetection_X;

        Vector2 direction = rb2d.gravityScale > 0 ? Vector2.down : Vector2.up;
        float distance = GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return hit.transform.gameObject.tag;
        }

        return "";
    }

    public GameObject GetGOBelowRightBound()
    {
        Vector2 position = transform.position;
        position.x += GetComponent<SpriteRenderer>().bounds.size.x / 2;
        position.x -= m_fOffsetDetection_X;

        Vector2 direction = rb2d.gravityScale > 0 ? Vector2.down : Vector2.up;
        float distance = GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return hit.transform.gameObject;
        }

        return null;
    }

    public GameObject GetGOBelowLeftBound()
    {
        Vector2 position = transform.position;
        position.x -= GetComponent<SpriteRenderer>().bounds.size.x / 2;
        position.x += m_fOffsetDetection_X;

        Vector2 direction = rb2d.gravityScale > 0 ? Vector2.down : Vector2.up;
        float distance = GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        if (hit.collider != null)
        {
            return hit.transform.gameObject;
        }

        return null;
    }

    private bool HasObjectBelow(float fDistance)
    {
        Vector2 positionRight = transform.position;
        positionRight.x += GetComponent<SpriteRenderer>().bounds.size.x / 2;

        Vector2 positionLeft = transform.position;
        positionLeft.x -= GetComponent<SpriteRenderer>().bounds.size.x / 2;

        Vector2 direction = rb2d.gravityScale > 0 ? Vector2.down : Vector2.up;
        float distance = fDistance + GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hitRight = Physics2D.Raycast(positionRight, direction, distance, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(positionLeft, direction, distance, groundLayer);

        bool bRes = false;

        if (hitRight.collider != null || hitLeft.collider != null)
        {
            bRes = true;
        }

        return bRes;
    }

    private bool HasObjectTop(float fDistance)
    {
        Vector2 positionRight = transform.position;
        positionRight.x += GetComponent<SpriteRenderer>().bounds.size.x / 2;
        positionRight.x -= m_fOffsetDetection_X;

        Vector2 positionLeft = transform.position;
        positionLeft.x -= GetComponent<SpriteRenderer>().bounds.size.x / 2;
        positionLeft.x += m_fOffsetDetection_X; ;


        Vector2 direction = rb2d.gravityScale > 0 ? Vector2.up : Vector2.down;
        float distance = fDistance + GetComponent<SpriteRenderer>().bounds.size.y / 2 + m_fOffsetDetection_Y;

        RaycastHit2D hitRight = Physics2D.Raycast(positionRight, direction, distance, groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(positionLeft, direction, distance, groundLayer);

        bool bRes = false;

        if (hitRight.collider != null || hitLeft.collider != null)
        {
            bRes = true;
        }

        return bRes;
    }

    public void ShowSliderOnPlayer()
    {
        slider.SetActive(true);
        sliderValue = 0;
        Slider sliderCompo = slider.GetComponent<UnityEngine.UI.Slider>();
        sliderCompo.value = sliderValue;

        CancelInvoke("UpdateFluidValue");
        InvokeRepeating("UpdateFluidValue", 0.0f, frequenceSlider);
    }

    public void HideSliderOnPlayer()
    {
        slider.SetActive(false);
        sliderValue = 0;
        Slider sliderCompo = slider.GetComponent<Slider>();
        sliderCompo.value = sliderValue;

        CancelInvoke("UpdateFluidValue");
    }

    private void UpdateSliderPosition()
    {
        if(slider.activeSelf)
        {
            Vector2 worldPosition = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 worldOffset = new Vector2(0, 0.7f);
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition + worldOffset);
            Vector2 screenOffset = new Vector2(0, 0);
            slider.transform.position = screenPosition + screenOffset;
        }
    }

    public void UpdateFluidValue()
    {
        sliderValue += 1f/(maxTime / frequenceSlider);

        Slider sliderCompo = slider.GetComponent<Slider>();
        sliderCompo.value = sliderValue;

        if (sliderValue >= 1)
        {
            CancelInvoke("UpdateFluidValue");
            HideSliderOnPlayer();

            GameControllerGO.PlayerDied();
        }
    }

    private IEnumerator WaitAndTest()
    {
        yield return new WaitForSeconds(2.0f);

        SpriteRenderer sprt = GetComponent<SpriteRenderer>();

        //ReverseGravity();
    }

    private void PlayerHitTheGround()
    {
        SpriteRenderer sprt = GetComponent<SpriteRenderer>();

        Vector2 vPositionParticlesBD = Utilitaire.GetPoint(Utilitaire.PointBlock.BasDroite, sprt, transform.position, rb2d.gravityScale < 0);
        Vector2 vPositionParticlesBG = Utilitaire.GetPoint(Utilitaire.PointBlock.BasGauche, sprt, transform.position, rb2d.gravityScale < 0);

        Vector2 vOffset = new Vector2(0, 0.1f);

        if (GetTagBelowRightBound() != "")
            ParticlesManager.InstantiateParticule(jumpParticles, vPositionParticlesBD + vOffset);

        if(GetTagBelowLeftBound() != "")
            ParticlesManager.InstantiateParticule(jumpParticles, vPositionParticlesBG + vOffset);

        SoundManager.MakeSound(jumpSound, transform.position);
        
        if(!m_bReverseGravityAnim)
            AnimationManager.PlayAnimation(playerAnimator, "hitGround");
    }

    private void PlayerEnterWater()
    {
        SpriteRenderer sprt = GetComponent<SpriteRenderer>();

        Vector2 vPositionParticlesBD = Utilitaire.GetPoint(Utilitaire.PointBlock.BasDroite, sprt, transform.position, rb2d.gravityScale < 0);
        Vector2 vPositionParticlesBG = Utilitaire.GetPoint(Utilitaire.PointBlock.BasGauche, sprt, transform.position, rb2d.gravityScale < 0);

        Vector2 vOffset = new Vector2(0, 1f);

        ParticlesManager.InstantiateParticule(waterJumpParticles, vPositionParticlesBD + vOffset);
        ParticlesManager.InstantiateParticule(waterJumpParticles, vPositionParticlesBG + vOffset);

        SoundManager.MakeSound(waterSound, transform.position);

        if (!m_bReverseGravityAnim)
            AnimationManager.PlayAnimation(playerAnimator, "hitGround");
    }


    public void BeginDisappearAnimation()
    {
        rb2d.bodyType = RigidbodyType2D.Static;
        m_bHitTheGroundEnabled = false;
        AnimationManager.PlayAnimation(playerAnimator, "disapearAnimation");
    }

    public void DisappearDone()
    {
        transform.position = m_vPositionTP;
        AnimationManager.PlayAnimation(playerAnimator, "apearAnimation");
    }

    public void AppearDone()
    {
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        m_bHitTheGroundEnabled = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        string colTag = collision.gameObject.tag;

        if (colTag == "LockBlock" && m_bHasKey)
        {
            m_bHasKey = false;

            LockBlockController script = collision.gameObject.GetComponent<LockBlockController>();

            script.UnlockBlock();

            //Destroy(collision.gameObject);

            GameControllerGO.PlayerLoseKey();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        string colTag = collision.gameObject.tag;

        if (colTag == "KeyObject")
        {
            m_bHasKey = true;

            Destroy(collision.gameObject);

            GameControllerGO.PlayerGetKey();
        }
    }

    public void ReverseGravity()
    {
        if (m_bReverseGravityAnim)
            return;

        m_bReverseGravityAnim = true;

        //Le player à un Child SpriteRenderer
        //Pour éviter les problèmes de collision pendant l'animation de rotation, c'est le child qui rotate, pas le Player
        GameObject go = this.transform.GetChild(0).gameObject;
        go.SetActive(true);

        //On désactive le sprite du Player pour afficher seulement le sprite du Child
        SpriteRenderer sprt = this.GetComponent<SpriteRenderer>();
        sprt.enabled = false;

        rb2d.gravityScale *= -1;
        if (rb2d.gravityScale < 0)
        {
            AnimationManager.PlayAnimation(playerAnimator, "ReverseGravity");
        }
        else
        {
            AnimationManager.PlayAnimation(playerAnimator, "ReverseGravity_Normal");
        }
    }

    public void ResetColliders()
    {
        SpriteRenderer sprt = this.GetComponent<SpriteRenderer>();
        sprt.enabled = true;

        GameObject go = this.transform.GetChild(0).gameObject;
        go.SetActive(false);

        go.transform.rotation = new Quaternion(0, 0, 0, 0);

        //On pense à faire la rotation sur le Sprite du Player
        if (rb2d.gravityScale < 0)
        {
            this.transform.rotation = new Quaternion(0, 0, 180, 0);
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        m_bReverseGravityAnim = false;
    }
}
