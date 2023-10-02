using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

public class StackManager : MonoBehaviour
{
    [Header("Stack Settings")]
    public Transform BaseAnchor;
    public GameObject LayersParent;

    [Header("Block Settings")]
    public Vector3 BlockSize = new Vector3(0.25f, 0.15f, 0.85f);
    public float HorizontalSpacing = 0.01f;
    public float VerticalSpacing = 0.01f;
    public int BlockCount = 16;
    public Material[] Materials; // Assumes you have assigned 3 materials in the order: Glass, Wood, Stone
    public PhysicMaterial BlockPhysicsMaterial;
    public TextAsset ProcessedData; // This is where you'd assign your JSON file in the inspector

    private int currentBlockCount = 0;

    private List<RawDataModel> blockDataList;

    [System.Serializable]
    public class RawDataArrayWrapper
    {
        public RawDataModel[] items;
    }

    private void Start()
    {
        if (ProcessedData != null)
        {
            // Log the JSON content to Unity console
            var jsonData = SimpleJSON.JSON.Parse(ProcessedData.text);
            if (jsonData != null && jsonData.IsArray)
            {
                blockDataList = new List<RawDataModel>();

                foreach (SimpleJSON.JSONNode node in jsonData.AsArray)
                {
                    RawDataModel data = new RawDataModel
                    {
                        id = node["id"].AsInt,
                        subject = node["subject"],
                        grade = node["grade"],
                        mastery = node["mastery"].AsInt,
                        domainid = node["domainid"],
                        domain = node["domain"],
                        cluster = node["cluster"],
                        standardid = node["standardid"],
                        standarddescription = node["standarddescription"]
                    };
                    blockDataList.Add(data);
                }
            }
            else
            {
                Debug.LogError("Error parsing JSON data or JSON data is not an array.");
            }
        }
        BuildStack();
    }

    public void BuildStack()
    {
        if (!LayersParent)
        {
            Debug.LogError("LayersParent GameObject is not assigned!");
            return;
        }

        int layerNumber = 1;
        while (currentBlockCount < BlockCount)
        {
            BuildLayer(layerNumber);
            layerNumber++;
        }
    }

    private void BuildLayer(int layerNumber)
    {
        // Create a parent for each layer
        GameObject layer = new GameObject($"Layer Parent ({layerNumber})");
        layer.transform.SetParent(LayersParent.transform);

        // Determine how many blocks to create in this layer
        int blocksThisLayer = Mathf.Min(3, BlockCount - currentBlockCount);
        for (int i = 0; i < blocksThisLayer; i++)
        {
            BuildBlock(layer.transform);
        }

        // Adjust position for the next layer
        BaseAnchor.position += new Vector3(0, BlockSize.y + VerticalSpacing, 0);

        // Rotate layers as required by Jenga rules
        if (layerNumber % 2 == 0)
        {
            layer.transform.RotateAround(BaseAnchor.position, Vector3.up, 90);
        }
    }

    private void BuildBlock(Transform parent)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = $"Block ({++currentBlockCount})";
        block.transform.SetParent(parent);

        block.transform.localScale = BlockSize;

        if (blockDataList != null && currentBlockCount <= blockDataList.Count)
        {
            Renderer renderer = block.GetComponent<Renderer>();
            int mastery = blockDataList[currentBlockCount - 1].mastery;
            if (renderer && mastery >= 0 && mastery < Materials.Length)
            {
                renderer.material = Materials[mastery];
            }
        }

        if (BlockPhysicsMaterial)
        {
            Collider collider = block.GetComponent<Collider>();
            if (collider)
            {
                collider.material = BlockPhysicsMaterial;
            }
        }

        // Calculate the block's number in the current layer (0, 1, or 2)
        int blockNumberInLayer = (currentBlockCount - 1) % 3;

        // We calculate the total width for a block (including the space that comes after it)
        float totalBlockWidth = BlockSize.x + HorizontalSpacing;

        // Initial position is set to be the negative total width of one block. This sets the leftmost block's position.
        float startPosition = -totalBlockWidth;

        // Now we adjust this start position based on the block's number in the layer.
        float offset = startPosition + blockNumberInLayer * totalBlockWidth;

        // Set the block's position
        block.transform.position = BaseAnchor.position + new Vector3(offset, 0, 0);

        Rigidbody rb = block.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void ToggleKinematic()
    {
        foreach (Transform layer in LayersParent.transform)
        {
            foreach (Transform block in layer)
            {
                Rigidbody rb = block.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.isKinematic = !rb.isKinematic;
                }
            }
        }
    }
}
