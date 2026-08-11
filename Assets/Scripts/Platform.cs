using UnityEngine;

public class Platform : MonoBehaviour
{
    private GameObject player;
    private Scoring scoring;
    bool isTouched;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        scoring = FindAnyObjectByType<Scoring>();
        isTouched = false;
    }

    // Update is called once per frame
    void Update()
    {
        isPlayerAbove();
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
}
