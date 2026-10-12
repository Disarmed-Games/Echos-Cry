using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject _selectImage;

    private void OnEnable()
    {
        _selectImage.SetActive(EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject);
    }

    public void OnSelect(BaseEventData eventData) => _selectImage.SetActive(true);
    public void OnDeselect(BaseEventData eventData) => _selectImage.SetActive(false);

    public void OnPointerEnter(PointerEventData eventData)
    {
        _selectImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selectImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _selectImage.SetActive(false);
    }
}
