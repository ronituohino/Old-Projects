using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : Singleton<MoneyManager>
{
    [SerializeField] GameObject coin;
    [SerializeField] Transform coinParent;

    [SerializeField] TextMeshProUGUI balanceText;

    int coinAmount;

    private void Start()
    {
        GlobalReferencesAndSettings.Instance.moneyManager = this;
    }

    public void AddCoins(int amount)
    {
        coinAmount += amount;
        UpdateText();
    }

    public void RemoveCoins(int amount)
    {
        coinAmount -= amount;
        UpdateText();
    }

    void UpdateText()
    {
        balanceText.text = $"{coinAmount} $";
    }

    public IEnumerator SpawnCoins(int amount, Vector2 world)
    {
        for(int i = 0; i < amount; i++)
        {
            Instantiate(coin, world, Quaternion.identity, coinParent);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        }
    }
}
