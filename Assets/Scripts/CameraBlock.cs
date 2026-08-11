using UnityEngine;

public class CameraBlock : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, 0f);
    private float topPosY;

    private void Start()
    {
        topPosY = target.position.y;
    }
    private void LateUpdate()
    {
        PosUpdate();
    }
    private void PosUpdate()
    {
        if (target.position.y > topPosY)
        {
            topPosY = target.position.y;
        }
        transform.position = new Vector3(0, topPosY, transform.position.z) + offset;
    }

    public void ResetCamPos(Vector2 position)
    {
        topPosY = position.y;
    }
}
