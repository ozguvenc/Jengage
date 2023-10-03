using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public string[] ViewNames = { "6th", "7th", "8th", "Menu" };
    public string CurrentViewName = "6th";

    public int GetCurrentStackIndex()
    {
        int index = System.Array.IndexOf(ViewNames, CurrentViewName);
        if (index < 0)
        {
            // Log a warning if the provided view name does not exist
            Debug.LogWarning("The view name " + CurrentViewName + " does not exist in ViewNames.");
        }
        return index;
    }

    public void NextView()
    {
        int currentIndex = System.Array.IndexOf(ViewNames, CurrentViewName);
        if (currentIndex < ViewNames.Length - 1)
        {
            CurrentViewName = ViewNames[currentIndex + 1];
        }
        else
        {
            CurrentViewName = ViewNames[0];
        }
    }

    public void PreviousView()
    {
        int currentIndex = System.Array.IndexOf(ViewNames, CurrentViewName);
        if (currentIndex > 0)
        {
            CurrentViewName = ViewNames[currentIndex - 1];
        }
        else
        {
            CurrentViewName = ViewNames[ViewNames.Length - 1];
        }
    }

    public void GoToView(string viewName)
    {
        // Check if the provided view name exists in the ViewNames array
        if (System.Array.Exists(ViewNames, view => view == viewName))
        {
            // Update the current view name
            CurrentViewName = viewName;
        }
        else
        {
            // Log a warning if the provided view name does not exist
            Debug.LogWarning("The view name " + viewName + " does not exist in ViewNames.");
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
