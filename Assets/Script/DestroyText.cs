using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyText : MonoBehaviour
{
    public GameObject text;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("SampleCoroutine");
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator SampleCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(text);

    }
}
