using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;
using System;

public class StackManager : MonoBehaviour
{
    [Header("Stacks Configuration")]
    public StackConfig[] StackConfigs; // Store the settings for each stack here

    [Header("Block Settings")]
    public Vector3 BlockSize = new Vector3(0.25f, 0.15f, 0.85f);
    public float HorizontalSpacing = 0.01f;
    public float VerticalSpacing = 0.01f;
    public Material[] Materials;
    public PhysicMaterial BlockPhysicsMaterial;
    public AudioClip FallSound;

    public LevelManager levelManager; // Reference to the LevelManager

    private List<RawDataModel>[] blockDataLists; // A list for each grade's data
    private int[] currentBlockCounts; // A counter for each grade's block count
    private bool isKinematic = true;

    private void Start()
    {
        int numberOfStacks = StackConfigs.Length;
        blockDataLists = new List<RawDataModel>[numberOfStacks];
        currentBlockCounts = new int[numberOfStacks];

        for (int i = 0; i < numberOfStacks; i++)
        {
            // Store initial transform values
            StackConfigs[i].InitialPosition = StackConfigs[i].BaseAnchor.position;
            StackConfigs[i].InitialRotation = StackConfigs[i].BaseAnchor.rotation;
            StackConfigs[i].InitialScale = StackConfigs[i].BaseAnchor.localScale;

            // Parse JSON and fill blockDataLists[i]
            var jsonData = JSON.Parse(StackConfigs[i].ProcessedData.text);
            if (jsonData == null || !jsonData.IsArray)
            {
                Debug.LogError("Invalid JSON data for stack " + i);
                continue;
            }

            blockDataLists[i] = new List<RawDataModel>();
            foreach (JSONNode node in jsonData.AsArray)
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
                blockDataLists[i].Add(data);
            }

            // Ensure the stack is built with original positioning
            StackConfigs[i].BaseAnchor.position = StackConfigs[i].InitialPosition;
            StackConfigs[i].BaseAnchor.rotation = StackConfigs[i].InitialRotation;
            StackConfigs[i].BaseAnchor.localScale = StackConfigs[i].InitialScale;

            BuildStack(i); // Build each stack
        }
    }

    public StackConfig GetCurrentStackConfig()
    {
        return Array.Find(StackConfigs, config => config.GradeName == levelManager.CurrentViewName);
    }

    public void BuildStack(int stackIndex)
    {
        if (!StackConfigs[stackIndex].LayersParent)
        {
            Debug.LogError($"LayersParent GameObject is not assigned for stack {stackIndex}!");
            return;
        }

        int layerNumber = 1;
        while (currentBlockCounts[stackIndex] < blockDataLists[stackIndex].Count)
        {
            BuildLayer(layerNumber, stackIndex);
            layerNumber++;
        }
    }

    private void BuildLayer(int layerNumber, int stackIndex)
    {
        GameObject layer = new GameObject($"Layer Parent ({layerNumber})");
        layer.transform.SetParent(StackConfigs[stackIndex].LayersParent.transform);

        int blocksThisLayer = Mathf.Min(
            3,
            blockDataLists[stackIndex].Count - currentBlockCounts[stackIndex]
        );
        for (int i = 0; i < blocksThisLayer; i++)
        {
            BuildBlock(layer.transform, stackIndex);
        }

        StackConfigs[stackIndex].BaseAnchor.position += new Vector3(
            0,
            BlockSize.y + VerticalSpacing,
            0
        );

        if (layerNumber % 2 == 0)
        {
            layer.transform.RotateAround(
                StackConfigs[stackIndex].BaseAnchor.position,
                Vector3.up,
                90
            );
        }
    }

    private void BuildBlock(Transform parent, int stackIndex)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = $"Block ({++currentBlockCounts[stackIndex]})";
        block.transform.SetParent(parent);
        block.transform.localScale = BlockSize;

        block.AddComponent<HighlightObject>();

        if (
            blockDataLists[stackIndex] != null
            && currentBlockCounts[stackIndex] <= blockDataLists[stackIndex].Count
        )
        {
            Renderer renderer = block.GetComponent<Renderer>();
            int mastery = blockDataLists[stackIndex][currentBlockCounts[stackIndex] - 1].mastery;
            if (renderer && mastery >= 0 && mastery < Materials.Length)
            {
                renderer.material = Materials[mastery];

                switch (Materials[mastery].name)
                {
                    case "Glass":
                        block.tag = "Glass";
                        break;
                    case "Wood":
                        block.tag = "Wood";
                        break;
                    case "Stone":
                        block.tag = "Stone";
                        break;
                }
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

        int blockNumberInLayer = (currentBlockCounts[stackIndex] - 1) % 3;
        float totalBlockWidth = BlockSize.x + HorizontalSpacing;
        float startPosition = -totalBlockWidth;
        float offset = startPosition + blockNumberInLayer * totalBlockWidth;
        block.transform.position =
            StackConfigs[stackIndex].BaseAnchor.position + new Vector3(offset, 0, 0);

        Rigidbody rb = block.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        AudioSource audioSource = block.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.clip = FallSound;
        audioSource.playOnAwake = false;

        BlockCollisionHandler collisionHandler = block.AddComponent<BlockCollisionHandler>();
        collisionHandler.audioSource = audioSource;

        if (
            blockDataLists[stackIndex] != null
            && currentBlockCounts[stackIndex] <= blockDataLists[stackIndex].Count
        )
        {
            BlockInfo blockInfo = block.AddComponent<BlockInfo>();
            RawDataModel data = blockDataLists[stackIndex][currentBlockCounts[stackIndex] - 1];
            blockInfo.GradeLevel = data.grade;
            blockInfo.Domain = data.domain;
            blockInfo.Cluster = data.cluster;
            blockInfo.StandardID = data.standardid;
            blockInfo.StandardDescription = data.standarddescription;
        }
    }

    public void ToggleKinematic()
    {
        StackConfig currentConfig = GetCurrentStackConfig();

        if (currentConfig == null)
        {
            Debug.LogError("No StackConfig found for current view!");
            return;
        }

        isKinematic = !isKinematic;
        Debug.Log("Blocks are kinematic = " + isKinematic);

        foreach (Transform layer in currentConfig.LayersParent.transform)
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

    public void RebuildStack()
    {
        StackConfig currentConfig = GetCurrentStackConfig();

        if (currentConfig == null)
        {
            Debug.LogError("No StackConfig found for current view!");
            return;
        }

        // Restore the BaseAnchor's initial transform
        currentConfig.BaseAnchor.position = currentConfig.InitialPosition;
        currentConfig.BaseAnchor.rotation = currentConfig.InitialRotation;
        currentConfig.BaseAnchor.localScale = currentConfig.InitialScale;

        // Destroy all existing blocks
        foreach (Transform child in currentConfig.LayersParent.transform)
        {
            Destroy(child.gameObject);
        }

        // Find the index of currentConfig in StackConfigs
        int indexOfCurrentConfig = Array.IndexOf(StackConfigs, currentConfig);
        if (indexOfCurrentConfig == -1)
        {
            Debug.LogError("Current StackConfig is not found in StackConfigs array!");
            return;
        }

        // Update the corresponding index in currentBlockCounts.
        currentBlockCounts[indexOfCurrentConfig] = 0;

        // Ensure blocks are kinematic after rebuilding
        isKinematic = true;

        // Call BuildStack to rebuild the blocks
        BuildStack(indexOfCurrentConfig);
    }
}
