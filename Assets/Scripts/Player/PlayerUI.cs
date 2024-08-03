using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Currency")]
    public TextMeshProUGUI copperText;
    public TextMeshProUGUI SilverText;
    public TextMeshProUGUI goldText;
    private PlayerInventory inventory;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    public void UpdateCoinsUI()
    {
        copperText.text = FormatNumber(inventory._copper);
        SilverText.text = FormatNumber(inventory._silver);
        goldText.text = FormatNumber(inventory._gold);
    }

    public static string FormatNumber(long number)
    {
        if (number >= 1000000000)
            return (number / 1000000000f).ToString("0.##") + "b";
        if (number >= 1000000)
            return (number / 1000000f).ToString("0.##") + "m";
        if (number >= 1000)
            return (number / 1000f).ToString("0.##") + "k";
        return number.ToString();
    }
}
