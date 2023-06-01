using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnHover : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler
{
    [SerializeField] bool clickSound;
    private Selectable selectable;
    void Start()
    {
        selectable = GetComponent<Selectable>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentSelectedGameObject != selectable.gameObject)
        {
            selectable.Select();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(clickSound)
            SoundManager.Instance.PlaySound("ButtonClick", 1f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SoundManager.Instance.PlaySound("ButtonHover", 1f);
    }
}
