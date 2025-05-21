using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColor : MonoBehaviour
{
  [SerializeField] private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.color = Color.HSVToRGB(Time.time % 1, 1, 1);
    }
}
