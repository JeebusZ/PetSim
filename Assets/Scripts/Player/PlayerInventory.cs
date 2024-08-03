using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private long copper;
    [SerializeField] private long silver;
    [SerializeField] private long gold;

    public long _copper { get { return copper; } set { copper = value; } }
    public long _silver { get { return silver; } set { silver = value; } }
    public long _gold { get { return gold; } set { gold = value; } }


    private const long CopperToSilver = 10000;
    private const long SilverToGold = 1000;

    public void AddCopper(long amount)
    {
        copper += amount;
        ConvertCurrency();
    }
    public void AddSilver(long amount)
    {
        silver += amount;
        ConvertCurrency();
    }
    public void AddGold(long amount)
    {
        gold += amount;
        ConvertCurrency();
    }

    private void ConvertCurrency()
    {
        if(copper >= CopperToSilver)
        {
            silver += copper / CopperToSilver;
            copper %= CopperToSilver;
        }

        if(silver >= SilverToGold)
        {
            gold += silver / SilverToGold;
            silver %= SilverToGold;
        }
        GetComponent<PlayerUI>().UpdateCoinsUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            long randomCopper = UnityEngine.Random.Range(1000, 100000); // Random copper between 1,000 and 100,000
            AddCopper(randomCopper);
        }
    }

    private void ResetMoney()
    {
        copper = 0;
        silver = 0;
        gold = 0;
    }
}
