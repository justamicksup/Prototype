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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.powerPickup(power);
            aud.Play();
            gameObject.SetActive(false);

            Invoke(nameof(ResetStats), power.effectDuration);

        }
    }
    private void ResetStats()
    {
        gameManager.instance.playerScript.playerSpeed -= power.speedBonus;
        gameManager.instance.playerScript.stamina -= power.staminaBonus;
        gameManager.instance.playerScript.shootDamage -= power.shootDmgBonus;
        gameManager.instance.playerScript.meleeDamage -= power.meleeDmgBonus;

        Destroy(gameObject);
    }
}
