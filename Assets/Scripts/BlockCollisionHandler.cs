using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BlockCollisionHandler : MonoBehaviour
{
    public AudioSource audioSource;
    private bool hasPlayed = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasPlayed && collision.gameObject.CompareTag("Table"))
        {
            audioSource.Play();
            hasPlayed = true;
        }
    }
}
