using UnityEngine;

public class RotateManager : MonoBehaviour
{
    // Reference to the LevelManager to get the current view
    public LevelManager levelManager;

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
            float difference = lastMousePosition.x - Input.mousePosition.x;

            // Get the current StackConfig based on LevelManager's current view
            StackConfig currentStackConfig = GetCurrentStackConfig();
            if (currentStackConfig == null || currentStackConfig.LayersParent == null)
            {
                Debug.LogError("No valid LayersParent to rotate for the current view.");
                return;
            }

            // Rotate the LayersParent of the current stack based on the horizontal mouse movement
            currentStackConfig.LayersParent.transform.Rotate(Vector3.up, difference * 0.5f);

            lastMousePosition = Input.mousePosition;
        }
    }

    private StackConfig GetCurrentStackConfig()
    {
        // If there is a StackManager in the scene, attempt to retrieve the current StackConfig
        StackManager stackManager = FindObjectOfType<StackManager>();
        if (stackManager != null)
        {
            return stackManager.GetCurrentStackConfig();
        }

        Debug.LogError("No StackManager found in the scene.");
        return null;
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
