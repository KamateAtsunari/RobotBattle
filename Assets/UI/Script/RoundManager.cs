using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    //private int roundCount;
    [SerializeField] private vThirdPersonCamera tCamera = null;
    [SerializeField] private Text roundText = null;
    private int roundCount;
    public static string winText; 
    // Start is called before the first frame update
    public void Initialize()
    {
    }
    public void ChangeRound()
    {
        roundCount++;
        roundText.text = roundCount.ToString();
        tCamera.Init();
    }
    public void ChangeWinerScene(string wText)
    {
        winText = wText;
        SceneManager.LoadScene("WinerScene");
    }
}
