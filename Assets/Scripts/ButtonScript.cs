using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool isLeft;
    private PlayerMovement pm;

    private void Start()
    {
        pm = FindAnyObjectByType<PlayerMovement>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (isLeft) pm.LeftButton();
        else pm.RightButton();
    }

    public void OnPointerUp(PointerEventData data)
    {
        pm.NoButton();
    }
}
