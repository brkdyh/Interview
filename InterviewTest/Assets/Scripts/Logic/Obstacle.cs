using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float FallDownSpeed = 1f;

    IEnumerator DelayDestory()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    public void FallDown(Vector3 StartPosition)
    {
        isFalling = true;
        transform.position = StartPosition;
        StartCoroutine(DelayDestory());
    }

    bool isFalling = false;
    void UpdateFallDown()
    {
        if (!isFalling)
            return;

        transform.position += Vector3.down * FallDownSpeed * Time.deltaTime;
    }

    private void Update()
    {
        UpdateFallDown();
    }
}
