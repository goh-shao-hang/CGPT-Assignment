using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Image StaminaBar1;
    public float CurrentStamina;
    private float MaxStamina = 100f;
    PlayerMovement Player;
    // Start is called before the first frame update
    private void Start()
    {
        StaminaBar1 = GetComponent<Image>();
        Player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentStamina = Player.currentStamina;
        StaminaBar1.fillAmount = CurrentStamina / MaxStamina;
    }
}
