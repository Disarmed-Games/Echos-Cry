using AudioSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private soundEffect clickSFX;
    [SerializeField] private soundEffect hoverSFX;

    public void OnPointerClick(PointerEventData eventData)
    {
        EchosCry.Sound.PlaySFX(clickSFX, this.transform, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EchosCry.Sound.PlaySFX(hoverSFX, this.transform, 0);
    }
}
