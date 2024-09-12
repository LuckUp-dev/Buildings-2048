using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCubes : MonoBehaviour
{
    Vector3 _startMousePos;
    Vector3 _finishMousePos;

    public float speed = 10f;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(MoveFunction());
        }
    }

    IEnumerator MoveFunction()
    {
        float timeSinceStarted = 0f;
        _startMousePos = transform.position;
        _finishMousePos = _startMousePos + new Vector3(0f, 0f, 1.05f);
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            float percentageComplete = timeSinceStarted / speed;
            transform.position = Vector3.Lerp(_startMousePos, _finishMousePos, percentageComplete);

            // If the object has arrived, stop the coroutine
            if (transform.position == _finishMousePos)
            {
                yield break;
            }

            // Otherwise, continue next frame
            yield return null;
        }
    }
}
