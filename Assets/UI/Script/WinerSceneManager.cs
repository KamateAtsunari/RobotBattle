using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WinerSceneManager : MonoBehaviour
{
    [SerializeField] private Text winText = null;
    // Start is called before the first frame update
    void Start()
    {
        winText.text = RoundManager.winText;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("r"))
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
