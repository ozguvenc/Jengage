using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    [Tooltip("Event that gets triggered when the object is clicked.")]
    public UnityEvent OnClick;

    private MeshCollider meshCollider;

    private void Start()
    {
        meshCollider = GetComponent<MeshCollider>();

        // Ensure there's a collider
        if (meshCollider == null)
        {
            Debug.LogError("No MeshCollider found on the clickable object. Adding one.");
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
    }

    private void OnMouseDown()
    {
        // Trigger the assigned UnityEvent
        OnClick.Invoke();
    }
}
