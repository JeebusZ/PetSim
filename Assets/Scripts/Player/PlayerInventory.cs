using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public long copper;
    public long silver;
    public long gold;

    private const long CopperToSilver = 10000;
    private const long SilverToGold = 1000;

    public void AddCopper(long amount)
    {
        copper += amount;
        GetComponent<PlayerUI>().UpdateCoinsUI(amount, ResourceType.COPPER);
        ConvertCurrency();
    }
    public void AddSilver(long amount)
    {
        silver += amount;
        GetComponent<PlayerUI>().UpdateCoinsUI(amount, ResourceType.SILVER);
        ConvertCurrency();
    }
    public void AddGold(long amount)
    {
        gold += amount;
        GetComponent<PlayerUI>().UpdateCoinsUI(amount, ResourceType.GOLD);
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
