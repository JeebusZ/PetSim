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

    private void Start()
    {
        copperText.text = "0";
        SilverText.text = "0";
        goldText.text = "0";
    }

    public void UpdateCoinsUI(long amount, ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.COPPER:
                copperText.text = FormatNumber(amount);
                break;
            case ResourceType.SILVER:
                SilverText.text = FormatNumber(amount);
                break;
            case ResourceType.GOLD:
                goldText.text = FormatNumber(amount);
                break;
            case ResourceType.PLATINUM:
                break;
            case ResourceType.DIAMOND:
                break;
            default:
                break;
        }
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
