using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreCount : MonoBehaviour
{
    private int _score;

    public Text scoretext;

    private void Start()
    {
        _score = 0;
    }

    private void Update()
    {
        if(_score >= 80)
        {
            SceneManager.LoadScene("ClearScene", LoadSceneMode.Single);
        }
    }

    public void ScoreAdd()
    {
        _score++;
        scoretext.text = _score.ToString();
    }
   
}
