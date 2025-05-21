using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public static class Tags
    {
        public const string TitleScene = "TitleScene";
    }
    public void SwitchScene()
    {
        SceneManager.LoadScene(Tags.TitleScene, LoadSceneMode.Single);
    }
}
