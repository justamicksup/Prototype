using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerStat power;
    public AudioSource aud;
    float time;

    private void Start()
    {
        aud.clip = power.powerAudio;
        time = power.effectDuration;
    }

    public void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aud.Play();
            StartCoroutine(playAud());
            try
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            catch
            {
            }
            Invoke(nameof(ResetStats), time);
        }
    }
    private void ResetStats()
    {
        //Debug.Log("RESET STATS INVOKED " + time);
        gameManager.instance.playerScript.playerBaseSpeed -= power.speedBonus;
        gameManager.instance.playerScript.currentStamina -= power.staminaBonus;
        gameManager.instance.playerScript.shootDamage -= power.shootDmgBonus;        

        gameManager.instance.playerScript.speedPart.Stop();
        //gameManager.instance.playerScript.healthPart.Stop();
        gameManager.instance.playerScript.speedPart.gameObject.SetActive(false);

        Destroy(gameObject);
    }

    private IEnumerator playAud()
    {
        yield return new WaitForSeconds(aud.clip.length);
        gameManager.instance.playerScript.powerPickup(power);
        gameObject.SetActive(false);
    }
}
