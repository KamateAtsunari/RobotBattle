using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle = null;
    private ParticleSystem.MinMaxGradient color;
    private ParticleSystem.MainModule main;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        color = new ParticleSystem.MinMaxGradient();
        color.mode = ParticleSystemGradientMode.Color;
        main = particle.main;
    }

    // Update is called once per frame
    void Update()
    {
        //時間経過によってエフェクトの色を変える
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 2 && elapsedTime < 2.1f)
        {
            //Debug.Log(2);
            Color color = new Color(0.65f, 0.25f, 1f);
            main.startColor = color;
        }
        else if(elapsedTime > 4 && elapsedTime < 4.1f)
        {
            //Debug.Log(4);
            Color color = new Color(1f, 0.25f, 0.35f);
            main.startColor = color;
        }
    }
}
