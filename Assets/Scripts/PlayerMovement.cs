using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Параметры игрока")]
    [SerializeField] private float playerSideSpeed = 5f;
    [SerializeField] private float playerJumpForce = 5f;
    [SerializeField] private float offsetX = 0.3f;
    [Header("Точки")]
    [SerializeField] private Transform startPoint;
    private Vector2 lastPlayerPos;
    [Header("Анимации")]
    private SpriteRenderer sprite;
    //
    //
    //
    private float moveDirect = 0f;
    private Camera cam;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isOnGround;
    private bool isGameStopped;
    private float maxSpeed = 8f;
    private float leftBorder, rightBorder;
    private float savedVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        cam = Camera.main;
        sprite = GetComponent<SpriteRenderer>();
        GetScreenBorders();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        if (isGameStopped == false)
        {
            Movement();
        }
    }

    public void LeftButton() => moveDirect = -1;
    public void RightButton() => moveDirect = 1;
    public void NoButton() => moveDirect = 0;

    public void Movement()
    {
        if (isOnGround == true && rb.linearVelocityY <= 0)
        {
            rb.linearVelocityY = playerJumpForce;
        }
        if (moveDirect != 0 && rb != null)
        {
            rb.linearVelocityX = moveDirect * playerSideSpeed * Time.deltaTime;
        }
        if (moveDirect == 0 && rb != null)
        {
            rb.linearVelocityX = 0f;
        }

        if (rb.linearVelocityY > maxSpeed)
        {
            rb.linearVelocityY = 0;
        }
        WorldBorders();
        CharacterLookToMove();
    }

    private void WorldBorders()
    {
        float playerX = transform.position.x;
        if (playerX >= rightBorder)
        {
            transform.position = new Vector2(leftBorder + offsetX, transform.position.y);
        }
        if (playerX <= leftBorder)
        {
            transform.position = new Vector2(rightBorder - offsetX, transform.position.y);
        }
    }
    private void GetScreenBorders()
    {
        Vector3 leftEdge = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 rightEdge = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        leftBorder = leftEdge.x;
        rightBorder = rightEdge.x;
    }

    public void PlayerDead()
    {
        rb.gravityScale = 0f;
    }
    public void PlayerSurrender()
    {
        rb.gravityScale = 1f;
        transform.position = startPoint.position;
        ResetCamera();
        EventBus.isPlayerReady?.Invoke();
    }
    public void PlayerContinue()
    {
        rb.gravityScale = 1f;
        transform.position = lastPlayerPos;
        ResetCamera();
    }
    private void Pause()
    {
        savedVelocity = rb.linearVelocityY;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;      
    }
    private void Resume()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.gravityScale = 1f;
        rb.linearVelocityY = savedVelocity;
    }
    private void ResetCamera()
    {
        cam.GetComponent<CameraBlock>().ResetCamPos(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platforms") || collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            lastPlayerPos = transform.position;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platforms") || collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            lastPlayerPos = transform.position;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isOnGround = false;
    }

    //Анимации
    private void CharacterLookToMove()
    {
        if (moveDirect == -1)
        {
            sprite.flipX = true;
        }
        if (moveDirect == 1)
        {
            sprite.flipX = false;
        }
    }

    





    private void OnEnable()
    {
        EventBus.isPlayerContinue += PlayerContinue;
        EventBus.isPlayerSurrender += PlayerSurrender;
        EventBus.isPause += Pause;
        EventBus.isResume += Resume;
    }
    private void OnDisable()
    {
        EventBus.isPlayerContinue -= PlayerContinue;
        EventBus.isPlayerSurrender -= PlayerSurrender;
        EventBus.isPause -= Pause;
        EventBus.isResume -= Resume;
    }
}
