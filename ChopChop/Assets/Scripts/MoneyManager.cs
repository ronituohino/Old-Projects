using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

public class MoneyManager : Singleton<MoneyManager>
{
    protected float currentBalance;
    public TextMeshProUGUI text;
    CultureInfo culture;

    private void Start()
    {
        culture = CultureInfo.CurrentCulture;
        UpdateBalance(currentBalance);
    }

    //Sets the balance to amount
    protected void UpdateBalance(float amount)
    {
        currentBalance = amount;
        text.text = amount.ToString("C", culture);
    }

    //Adds amount to current balance
    public void AddMoney(float amount)
    {
        UpdateBalance(amount + currentBalance);
    }
}
