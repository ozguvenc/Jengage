using UnityEngine;

public class StackManager : MonoBehaviour
{
    [Header("Stack Settings")]
    public Transform BaseAnchor;
    public GameObject LayersParent;

    [Header("Block Settings")]
    public Vector3 BlockSize = new Vector3(0.25f, 0.15f, 0.85f);
    public float SpaceBetweenBlocks = 0.01f; // Space between blocks horizontally
    public float CollisionBuffer = 0.01f; // Space between layers vertically
    public int BlockCount = 16;
    public Material BlockMaterial;
    public PhysicMaterial BlockPhysicsMaterial;

    private int currentBlockCount = 0;

    private void Start()
    {
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
            BaseAnchor.position += new Vector3(0, BlockSize.y + CollisionBuffer, 0);

            // Rotate layers as required by Jenga rules
            if (layerNumber % 2 == 0)
            {
                layer.transform.RotateAround(BaseAnchor.position, Vector3.up, 90);
            }

            layerNumber++;
        }
    }

    private void BuildBlock(Transform parent)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = $"Block ({++currentBlockCount})";
        block.transform.SetParent(parent);

        block.transform.localScale = BlockSize;

        Renderer renderer = block.GetComponent<Renderer>();
        if (BlockMaterial && renderer)
        {
            renderer.material = BlockMaterial;
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
        float totalBlockWidth = BlockSize.x + SpaceBetweenBlocks;

        // Initial position is set to be the negative total width of one block. This sets the leftmost block's position.
        float startPosition = -totalBlockWidth;

        // Now we adjust this start position based on the block's number in the layer.
        // This will result in 0 adjustment for the leftmost block, 1x adjustment for the middle, and 2x adjustment for the rightmost block.
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
