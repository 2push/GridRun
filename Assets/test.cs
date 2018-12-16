using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    Coroutine coroutine;

    private void Start()
    {
        coroutine = StartCoroutine(Counter());
    }

    private void Update()
    {
        print(coroutine == null);
        if (Input.GetKeyDown(KeyCode.Space)) StopCoroutine(coroutine);
    }
    IEnumerator Counter()
    {
            print("well done!");
            yield return new WaitForSeconds(3);
    }
}
