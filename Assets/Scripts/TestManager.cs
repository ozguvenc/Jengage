using UnityEngine;
using System.Collections;
using TMPro;

public class TestManager : MonoBehaviour
{
    public int StartTimeInSeconds = 10;
    public AudioClip CountdownEndSound;
    public AudioClip CountdownTickSound;
    public StackManager stackManagerReference;
    public TextMeshProUGUI countdownText;
    public GameObject ResetButton;

    private AudioSource audioSource;

    private void Start()
    {
        if (stackManagerReference == null)
        {
            Debug.LogError("StackManager reference is not set in TestManager.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (countdownText == null)
        {
            Debug.LogError("Countdown TextMeshProUGUI reference is not set in TestManager.");
        }

        if (ResetButton == null)
        {
            Debug.LogError("ResetButton reference is not set in TestManager.");
        }
        else
        {
            ResetButton.SetActive(false); // Initially set the button to be inactive
        }
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int currentTime = StartTimeInSeconds;

        while (currentTime > 0)
        {
            countdownText.text = currentTime.ToString();

            // Play tick sound for every second change except the last one
            if (CountdownTickSound != null)
            {
                audioSource.PlayOneShot(CountdownTickSound);
            }

            yield return new WaitForSeconds(1);
            currentTime--;
        }

        countdownText.text = "0";

        // Play end sound when the countdown reaches zero
        if (CountdownEndSound != null)
        {
            audioSource.PlayOneShot(CountdownEndSound);
        }

        RemoveGlass();
        stackManagerReference.ToggleKinematic();

        ResetButton.SetActive(true); // Enable the ResetButton after the countdown is over
    }

    public void ResetTest()
    {
        stackManagerReference.RebuildStack();
        stackManagerReference.ToggleKinematic();
        ResetButton.SetActive(false);
    }

    public void RemoveGlass()
    {
        GameObject[] glassBlocks = GameObject.FindGameObjectsWithTag("Glass");
        foreach (GameObject glassBlock in glassBlocks)
        {
            Destroy(glassBlock);
        }
    }
}