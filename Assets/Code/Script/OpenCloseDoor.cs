using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseDoor : MonoBehaviour
{
    public bool isClockWise;
    public bool isOpen;
    public float timeMultiplier;
    public float degrees;
    float initialDegrees;
    bool isMoving=false;
    int dir = 1;

    private void Start()
    {
        if (isOpen)
        {
            if (isClockWise)
            {
                gameObject.transform.Rotate(new Vector3(0, 105, 0), Space.Self);
            }
            else { gameObject.transform.Rotate(new Vector3(0, -105, 0), Space.Self); }
        }
        initialDegrees = degrees;
    }
    public void CloseOpenDoor()
    {
        if (!isMoving)
        {
            StartCoroutine(RotateDoor());
        }
    }

    private IEnumerator RotateDoor()
    {
        isMoving = true;
        float direction = isClockWise ? 1 : -1;
        float targetDegrees = isOpen ? -initialDegrees : initialDegrees;
        float rotationAmount = 0;

        while (Mathf.Abs(rotationAmount) < Mathf.Abs(targetDegrees))
        {
            float step = direction * timeMultiplier * Time.deltaTime * dir;
            gameObject.transform.Rotate(new Vector3(0, step, 0), Space.Self);
            rotationAmount += step;
            yield return null;
        }

        dir *= -1;
        isOpen = !isOpen;
        isMoving = false;
    }

}
