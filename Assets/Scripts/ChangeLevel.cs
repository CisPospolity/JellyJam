using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevel : MonoBehaviour
{
    public void Change(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }
}
