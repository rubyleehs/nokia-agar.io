using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    private Vector3 originalPos;
    private static CameraShake instance;

    void Awake()
    {
        originalPos = transform.localPosition;
        instance = this;
    }

    public static void Shake(float duration, float amount)
    {
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.CamShake(duration, amount));
    }

    public IEnumerator CamShake(float duration, float amount)
    {
        float endTime = Time.unscaledTime + duration;
        int i = 0;
        while (Time.unscaledTime < endTime && i < 500)
        {
            i++;
            transform.localPosition = originalPos + Random.insideUnitSphere * amount;

            //duration -= Time.unscaledDeltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
