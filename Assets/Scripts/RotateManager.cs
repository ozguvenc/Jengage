using UnityEngine;

public class RotateManager : MonoBehaviour
{
    public GameObject LayersParent; // Assign your LayerParent GameObject here

    private Vector3 lastMousePosition;
    private bool isMouseDragEnabled = true;

    private void Update()
    {
        if (!isMouseDragEnabled)
            return;

        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0)) // Check if the left mouse button is being held
        {
            // Calculate the difference in horizontal position
            float difference = lastMousePosition.x - Input.mousePosition.x; // Swapped the positions for reverse rotation

            // Rotate the LayersParent based on the horizontal mouse movement
            LayersParent.transform.Rotate(Vector3.up, difference * 0.5f); // You can adjust the multiplier (0.5f here) to make rotation faster/slower

            lastMousePosition = Input.mousePosition;
        }
    }

    public void DisableMouseDrag()
    {
        isMouseDragEnabled = false;
    }

    public void EnableMouseDrag()
    {
        isMouseDragEnabled = true;
    }
}
