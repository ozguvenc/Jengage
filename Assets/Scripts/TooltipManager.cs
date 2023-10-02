using UnityEngine;
using TMPro; // Import the TextMeshPro namespace

public class TooltipManager : MonoBehaviour
{
    public TextMeshPro tooltipTextBox; // This is the TextMeshPro 3D variant reference

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
        string tooltipText =
            $"{data.GradeLevel}: {data.Domain}\n{data.Cluster}\n{data.StandardID}: {data.StandardDescription}";
        tooltipTextBox.text = tooltipText;
    }
}
