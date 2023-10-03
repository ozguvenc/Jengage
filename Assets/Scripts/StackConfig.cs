using UnityEngine;

[System.Serializable]
public class StackConfig
{
    public string GradeName;
    public Transform BaseAnchor;
    public GameObject LayersParent;
    public TextAsset ProcessedData;

    [HideInInspector] // Hide in Inspector as we'll set these programmatically
    public Vector3 InitialPosition;

    [HideInInspector]
    public Quaternion InitialRotation;

    [HideInInspector]
    public Vector3 InitialScale;
}
