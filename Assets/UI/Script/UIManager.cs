using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager = null;
    [SerializeField] private SimpleHealthBar healthBar = null;
    //ガードバー
    [SerializeField] private SimpleHealthBar guradBar = null;

    [SerializeField] private Image firstWin = null;
    [SerializeField] private Image secontWin = null;

    [SerializeField] private string winText = null;

    private int winCount = 1;

    public void HpBarUpdate(float hp,float maxHp)
    {
        healthBar.UpdateBar(hp,maxHp);
    }
    public void GuradBarUpdate(float gurad,float maxGurad)
    {
        guradBar.UpdateBar(gurad, maxGurad);
    }
    public void RoundWin()
    {
        switch (winCount)
        {
            case 1:
                firstWin.color = new Color(1, 0, 0);
                roundManager.ChangeRound();
                break;
            case 2:
                secontWin.color = new Color(1, 0, 0);
                roundManager.ChangeRound();
                break;
            case 3:
                roundManager.ChangeWinerScene(winText);
                //secontWin.color = new Color(1, 0, 0);
                break;
        }
        winCount++;
        
    }
}
