using UnityEngine;
using UnityEngine.SceneManagement;
public class RetryScene : MonoBehaviour
{
    public void SwitchScene()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
