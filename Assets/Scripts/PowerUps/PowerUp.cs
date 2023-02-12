using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerStat power;
    public AudioSource aud;

    private void Start()
    {
        aud.clip = power.powerAudio;
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
            Invoke(nameof(ResetStats), power.effectDuration);
        }
    }
    private void ResetStats()
    {
        gameManager.instance.playerScript.playerBaseSpeed -= power.speedBonus;
        gameManager.instance.playerScript.currentStamina -= power.staminaBonus;
        gameManager.instance.playerScript.shootDamage -= power.shootDmgBonus;
        gameManager.instance.playerScript.meleeDamage -= power.meleeDmgBonus;

        gameManager.instance.instaKillIcon.SetActive(false);
        gameManager.instance.speedBoostIcon.SetActive(false);
        gameManager.instance.healingIcon.SetActive(false);

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
