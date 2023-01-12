using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light spotLight;
    public float secondsToFlicker = 3;
    private float secondsPassed = 0;
    public Vector2 flickerIntensityRange = new Vector2(20, 100);
    private Coroutine flicker;

    // Update is called once per frame
    private void Start()
    {
        flicker = StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        float flickerIntensity = flickerIntensityRange.x;
        while (true)
        {
            secondsPassed = 0;
            float startIntensity = spotLight.intensity;
            //float secondsPassed = 0;
            while (secondsPassed <= secondsToFlicker)
            {
                spotLight.intensity = Mathf.Lerp(startIntensity, flickerIntensity, secondsPassed / secondsToFlicker);
                secondsPassed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (flickerIntensity < flickerIntensityRange.y) flickerIntensity = flickerIntensityRange.y;
            else flickerIntensity = flickerIntensityRange.x;
            yield return null;

        }
    }
}
