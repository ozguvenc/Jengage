using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public LevelManager levelManager; // Reference to the LevelManager to know the current view/grade.
    public TextMeshPro[] tooltipTextBoxes; // An array of TextMeshPro objects for the 6th, 7th, and 8th grades.

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                BlockInfo blockInfo = hit.transform.GetComponent<BlockInfo>();
                if (blockInfo)
                {
                    ShowTooltip(blockInfo);
                }
            }
        }
    }

    private void ShowTooltip(BlockInfo data)
    {
        // Get the current view index using LevelManager.
        int currentIndex = levelManager.GetCurrentStackIndex();

        // Ensure the current index is valid for our tooltip text boxes array.
        if (currentIndex < 0 || currentIndex >= tooltipTextBoxes.Length)
        {
            Debug.LogError("Invalid index for tooltip text boxes: " + currentIndex);
            return;
        }

        // Create the tooltip text.
        string tooltipText =
            $"{data.GradeLevel}: {data.Domain}\n{data.Cluster}\n{data.StandardID}: {data.StandardDescription}";

        // Assign the tooltip text to the corresponding TextMeshPro object.
        tooltipTextBoxes[currentIndex].text = tooltipText;
    }
}
