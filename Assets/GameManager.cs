using System.Collections; // Required for Coroutines (IEnumerator)
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    [Tooltip("Time in seconds to wait before loading the next level.")]
    public float sceneLoadDelay = 2.0f; // Adjust this value in the Unity Inspector

    private EndingNest[] EndingNests;
    private bool isLevelChanging = false;

    void Start()
    {
        EndingNests = GameObject.FindGameObjectsWithTag("Nest")
                                .Select(go => go.GetComponent<EndingNest>())
                                .Where(component => component != null)
                                .ToArray();
    }

    void Update()
    {
        Debug.LogWarning(EndingNests.Length);
        if (!isLevelChanging && EndingNests.Length > 0 && EndingNests.All(nest => nest.isFilled))
        {
            isLevelChanging = true;
            // Start the coroutine instead of calling a method directly
            StartCoroutine(LoadNextLevelWithDelay());
        }
    }

    // Changed from a regular method to an IEnumerator coroutine
    IEnumerator LoadNextLevelWithDelay()
    {
        // 1. Wait for the specified number of seconds
        yield return new WaitForSeconds(sceneLoadDelay);

        // 2. Get the current scene name
        string currentSceneName = SceneManager.GetActiveScene().name;
        string numberPart = currentSceneName.Replace("Level", "");

        if (int.TryParse(numberPart, out int currentLevelNumber))
        {
            int nextLevelNumber = currentLevelNumber + 1;
            string nextSceneName = "Level" + nextLevelNumber;

            // 3. Verify and load
            if (Application.CanStreamedLevelBeLoaded(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning($"[GameManager] Cannot load {nextSceneName}. It either doesn't exist or isn't added to Build Settings!");
                isLevelChanging = false;
                SceneManager.LoadScene("Level1");
            }
        }
        else
        {
            Debug.LogError($"[GameManager] Current scene name '{currentSceneName}' does not follow the 'Level{{number}}' format!");
            isLevelChanging = false;
        }
    }
}