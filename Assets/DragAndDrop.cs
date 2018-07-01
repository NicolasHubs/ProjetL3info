using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private float OffsetX;
    private float OffsetY;

    public void BeginDrag()
    {
        OffsetX = transform.parent.parent.position.x - Input.mousePosition.x;
        OffsetY = transform.parent.parent.position.y - Input.mousePosition.y;
    }

    public void onDrag()
    {
        transform.parent.parent.position = new Vector3(OffsetX + Input.mousePosition.x, OffsetY + Input.mousePosition.y);
    }
}

