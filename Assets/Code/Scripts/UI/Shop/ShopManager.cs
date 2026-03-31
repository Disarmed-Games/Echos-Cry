using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// Original Author: Abby
/// All Contributors Since Creation: Abby
/// Last Modified By:
public class ShopManager : MonoBehaviour
{
    [SerializeField] private InputTranslator _translator;
    [SerializeField] private GameObject[] ShopItemArray;
    [SerializeField] private TextMeshProUGUI totalCostText;
    [SerializeField] private TextMeshProUGUI currentGoldText;
    private int currentItemIndex = 0;
    private float totalCost = 0f;

    private void Update()
    {
        float tempCost = 0f;
        foreach (GameObject item in ShopItemArray)
        {
            tempCost += item.GetComponent<ShopItem>().GetCost();
        }
        totalCost = tempCost;
        totalCostText.text = $"Total Cost: ${totalCost.ToString()}";

        if(totalCost <= PlayerCurrencySystem.Instance.GetGoldCurrency())
        {
            totalCostText.color = Color.white;
        }
        else
        {
            totalCostText.color = Color.red;
        }

        currentGoldText.text = $"Gold Balance: ${PlayerCurrencySystem.Instance.GetGoldCurrency().ToString()}";
    }

    public void Purchase()
    {
        if (totalCost <= 0) return;
        if(PlayerCurrencySystem.Instance.GetGoldCurrency() >= totalCost)
        {
            foreach (GameObject item in ShopItemArray)
            {
                ShopItem shopItemScript = item.GetComponent<ShopItem>();
                for (int i = 0; i < shopItemScript.GetAmount(); i++)
                {
                    InventoryManager.Instance.Add(shopItemScript.GetItemData());
                }
                shopItemScript.ResetAmount();
            }

            PlayerCurrencySystem.Instance.DecrementGoldCurrency((int)totalCost);
        }
    }
}
