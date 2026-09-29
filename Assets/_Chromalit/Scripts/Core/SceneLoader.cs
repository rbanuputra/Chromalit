using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chromalit.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1f; // reset kalau dari pause
            SceneManager.LoadScene(sceneName);
        }

        public void QuitGame()
        {
            Debug.Log("Quit game!");
            Application.Quit();
        }
    }
}