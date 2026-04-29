using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] GameObject highlightImage;
    [SerializeField] InventoryItemData itemData;
    [SerializeField] private float initCost = 0f;
    private int buyAmount;
    private float actualCost = 0f;

    public void ToggleHighlight(bool state)
    {
        highlightImage.GetComponent<Image>().enabled = state;
    }

    public float GetCost()
    {
        UpdateCost();
        return actualCost;
    }
    public float GetAmount()
    {
        return buyAmount;
    }
    public InventoryItemData GetItemData()
    {
        return itemData;
    }
    public void ResetAmount()
    {
        buyAmount = 0;
        amountText.text = buyAmount.ToString();
        UpdateCost();
    }

    private void Start()
    {
        costText.text = $"${initCost.ToString()}";
    }

    public void IncreaseAmount()
    {
        buyAmount++;
        if (buyAmount > 99) { buyAmount = 0; }
        amountText.text = buyAmount.ToString();
        UpdateCost();
    }
    public void DecreaseAmount()
    {
        buyAmount--;
        if (buyAmount < 0 ) { buyAmount = 99; }
        amountText.text = buyAmount.ToString();
        UpdateCost();
    }

    private void UpdateCost()
    {
        actualCost = initCost * buyAmount;
        if (actualCost == 0)
        {
            costText.text = $"${initCost.ToString()}";
        }
        else
        {
            costText.text = $"${actualCost.ToString()}";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null && eventData.pointerEnter.GetComponent<Image>() != null)
        {
            ToggleHighlight(true);
            UITip.Instance.StartMessage(itemData.description, this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToggleHighlight(false);
        UITip.Instance.StopMessage(this);
    }
}
