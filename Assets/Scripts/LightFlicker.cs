using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light spotLight;
    public Vector2 flickerIntensityRange = new Vector2(20, 100);
    private Coroutine flicker;

    // Update is called once per frame
    private void Start()
    {
        flicker = StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            float waitSeconds = Random.Range(0.5f, 1);
            float transitionSeconds = Random.Range(0.5f, 1);
            float flickerIntensity = Random.Range(flickerIntensityRange.x, flickerIntensityRange.y);
            float secondsPassed = 0;
            while (secondsPassed <= transitionSeconds)
            {
                spotLight.intensity = Mathf.Lerp(spotLight.intensity, flickerIntensity, secondsPassed / transitionSeconds);
                secondsPassed += Time.deltaTime;
                yield return null;
                    ;
            }

            yield return new WaitForSeconds(waitSeconds);
        }
    }
}
