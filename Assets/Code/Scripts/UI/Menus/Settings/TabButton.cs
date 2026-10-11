using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TabGroup _tabGroup;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _tabText;
    public Image TabImage => _backgroundImage;
    public TextMeshProUGUI TabText => _tabText;

    public void OnPointerClick(PointerEventData eventData)
    {
        _tabGroup.OnTabSelected(this);
    }

    public void OnSubmit(BaseEventData eventData) => _tabGroup.OnTabSelected(this);
    public void OnSelect(BaseEventData eventData) => _tabGroup.OnTabEnter(this);
    public void OnDeselect(BaseEventData eventData) => _tabGroup.OnTabExit(this);

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tabGroup.OnTabExit(this);
    }

    void Awake()
    {
        if (GetComponent<Selectable>() == null) gameObject.AddComponent<Selectable>();
        _tabGroup.Subscribe(this);
    }
    void Update()
    {
        
    }
}
