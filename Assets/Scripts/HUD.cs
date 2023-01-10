using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    [SerializeField] Text HP;
    [SerializeField] Text ammo;
    [SerializeField] Text stamina;
    [SerializeField] Text coins;
    [SerializeField] Image overlay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HP.text = gameManager.instance.playerScript.getHP().ToString();
        ammo.text = gameManager.instance.playerScript.getAmmo().ToString();
        coins.text = gameManager.instance.playerScript.GetCoins().ToString();

    }

    public void damageFeedback()
    {

    }
}
