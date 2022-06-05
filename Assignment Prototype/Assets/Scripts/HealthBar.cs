using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image HealthBar1;
    public float CurrentHealth;
    private float MaxHealth = 100f;
    PlayerHealth Player;
    // Start is called before the first frame update
    private void Start()
    {
        HealthBar1 = GetComponent<Image>();
        Player = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentHealth = Player.currentHealth;
        HealthBar1.fillAmount = CurrentHealth / MaxHealth;
    }
}
