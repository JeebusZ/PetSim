using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootableUI : MonoBehaviour
{
    public long maxHealth;
    public long currentHealth;

    public float speed;
    public float speedEffect;

    public Slider fill;
    public Slider fillEffect;


    private void Update()
    {
        float target = Mathf.Lerp(fill.value, currentHealth, speed * Time.deltaTime);
        float targetEffect = Mathf.Lerp(fillEffect.value, currentHealth, speedEffect * Time.deltaTime);

        fill.value = target;
        fillEffect.value = targetEffect;
    }

    public void SetupUI(long maxHP)
    {
        maxHealth = maxHP;
        currentHealth = maxHP;
        fill.maxValue = maxHP;
        fillEffect.maxValue = maxHP;
    }

    public void TakeDamage(long Damage)
    {
        if(currentHealth > 0)
        {
            currentHealth -= Damage;
        }
    }
}
