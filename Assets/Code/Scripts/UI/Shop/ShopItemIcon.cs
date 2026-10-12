using UnityEngine.EventSystems;
using UnityEngine.UI;

// Give the item icon its own focus target alongside the quantity buttons.
public class ShopItemIcon : Selectable
{
    private ShopItem shopItem;

    protected override void Awake()
    {
        base.Awake();
        shopItem = GetComponentInParent<ShopItem>();
        transition = Transition.None;
        var links = navigation;
        links.mode = Navigation.Mode.Automatic;
        navigation = links;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (shopItem != null) shopItem.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (shopItem != null) shopItem.OnDeselect(eventData);
    }
}
