using UnityEngine;
using System.Collections;
using TMPro;

public class TestManager : MonoBehaviour
{
    public int StartTimeInSeconds = 10;
    public AudioClip CountdownEndSound;
    public AudioClip CountdownTickSound;
    public StackManager stackManagerReference; // Reference to StackManager
    public LevelManager levelManager; // Reference to LevelManager to know the current view
    public TextMeshPro[] countdownTexts; // Array to hold TextMeshPro for 6th, 7th, 8th grades
    public GameObject[] ResetButtons; // Array to hold Reset Buttons for 6th, 7th, 8th grades
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ValidateReferences();
    }

    private void ValidateReferences()
    {
        if (countdownTexts == null || countdownTexts.Length != 3)
        {
            Debug.LogError(
                "Countdown TextMeshPro references are not set correctly in TestManager."
            );
        }

        if (ResetButtons == null || ResetButtons.Length != 3)
        {
            Debug.LogError("ResetButton references are not set correctly in TestManager.");
        }

        if (levelManager == null)
        {
            Debug.LogError("LevelManager reference is not set in TestManager.");
        }

        if (stackManagerReference == null)
        {
            Debug.LogError("StackManager reference is not set in TestManager.");
        }
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        int currentIndex = levelManager.GetCurrentStackIndex();
        if (
            currentIndex < 0
            || currentIndex >= countdownTexts.Length
            || currentIndex >= ResetButtons.Length
        )
        {
            Debug.LogError("Invalid index for countdown texts or reset buttons: " + currentIndex);
            yield break; // Exit coroutine if invalid index
        }

        TextMeshPro currentCountdownText = countdownTexts[currentIndex];
        GameObject currentResetButton = ResetButtons[currentIndex];

        int currentTime = StartTimeInSeconds;

        while (currentTime > 0)
        {
            currentCountdownText.text = currentTime.ToString();

            if (CountdownTickSound != null)
            {
                audioSource.PlayOneShot(CountdownTickSound);
            }

            yield return new WaitForSeconds(1);
            currentTime--;
        }

        currentCountdownText.text = "0";

        if (CountdownEndSound != null)
        {
            audioSource.PlayOneShot(CountdownEndSound);
        }

        RemoveGlass();
        stackManagerReference.ToggleKinematic(); // Toggle kinematic state of the blocks

        currentResetButton.GetComponent<ClickableObject>().MakeObjectClickable();
    }

    public void ResetTest()
    {
        int currentIndex = levelManager.GetCurrentStackIndex();
        if (currentIndex < 0 || currentIndex >= ResetButtons.Length)
        {
            Debug.LogError("Invalid index for reset buttons: " + currentIndex);
            return; // Exit if invalid index
        }

        GameObject currentResetButton = ResetButtons[currentIndex];

        stackManagerReference.RebuildStack(); // Rebuild the stack
        stackManagerReference.ToggleKinematic(); // Toggle kinematic state back

        currentResetButton.GetComponent<ClickableObject>().MakeObjectNotClickable();
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
