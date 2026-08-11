using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    private Camera cam;
    private Vector3 offset = new Vector3(0f, -14f, 0f);
    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        transform.position = new Vector3(0, cam.transform.position.y, transform.position.z) + offset;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platforms"))
        {
            collision.gameObject.SetActive(false);
        }
        if (collision.CompareTag("Player") && playerMovement != null)
        {
            //Игрок сдох
            playerMovement.PlayerDead();
            EventBus.isPlayerDead?.Invoke();
        }

    }
}
