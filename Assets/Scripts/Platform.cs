using System;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private int platformType = 0;
    [SerializeField] private float minPlatformSpeed;
    [SerializeField] private float maxPlatformSpeed;
    [SerializeField] private float speedLimit = 200f;
    [SerializeField] private float scoreMultiplier;
    //0 - Обычная
    //1 - Двигающаяся
    //2 - Ломающаяся
    private Camera cam;
    private GameObject player;
    private Scoring scoring;
    private Rigidbody2D rb;
    private Collider2D col;
    bool isTouched;

    private float leftBorder;
    private float rightBorder;
    private float platformWidth;
    private float currentSpeed;
    private float direction = 1f;
    private int score;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
        scoring = FindAnyObjectByType<Scoring>();
        score = scoring.GetScore();
        col = GetComponent<Collider2D>();
        isTouched = false;
        if (platformType == 1)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerAbove();
    }
    private void FixedUpdate()
    {
        CurrentPlatform(platformType);
    }
    private float GetPlayerY()
    {
        float y = player.transform.position.y;
        return y;
    }

    private void isPlayerAbove()
    {
        if (transform.position.y < GetPlayerY() && isTouched == false)
        {
            isTouched = true;
            scoring.UpdateScore();
        }
    }

    public void TouchedReset()
    {
        isTouched = false;
    }

    //Для двигающихся платформ
    private void MovingPlatform()
    {
        float offsetX = platformWidth / 2;
        float posX = transform.position.x;
        currentSpeed = RandomPlatformSpeed();
        rb.linearVelocity = new Vector2(currentSpeed * direction * Time.deltaTime, 0f);
        if (posX + offsetX >= rightBorder)
        {
            direction = -1f;
        }
        else if(posX - offsetX <= leftBorder)
        {
            direction = 1f;
        }
    }
    private float RandomPlatformSpeed()
    {
        float speed = UnityEngine.Random.Range(minPlatformSpeed + SpeedMultiplier(), maxPlatformSpeed + SpeedMultiplier());
        if (speed >= speedLimit)
        {
            speed = speedLimit;
            return speed;
        }
        return speed;
    }
    private float SpeedMultiplier()
    {
        float speed = score * scoreMultiplier;
        return speed;
    }
    private void CurrentPlatform(int id)
    {
        switch (id)
        {
            case 0:
                break;
            case 1:
                MovingPlatform();
                break;
            case 2:
                BreakablePlatform();
                break;
            default:
                Debug.Log("Неведомый тип платформы!");
                break;
        }
    }
    private void GetScreenBorders(float right, float left)
    {
        rightBorder = right;
        leftBorder = left;
    }

    //Для хрупких платформ
    private void BreakablePlatform()
    {
        col.isTrigger = true;
    }
    private void Break()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Break();
        }
    }
    private void GetPlatformWidth(float width)
    {
        platformWidth = width;
    }


    private void OnEnable()
    {
        EventBus.isPlatformWidth += GetPlatformWidth;
        EventBus.isGetScreenBorders += GetScreenBorders;
    }
    private void OnDisable()
    {
        EventBus.isPlatformWidth -= GetPlatformWidth;
        EventBus.isGetScreenBorders -= GetScreenBorders;
    }
}
