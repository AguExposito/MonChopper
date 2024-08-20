using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler , IDragHandler
{
    
    [HideInInspector] public Transform parenAfterDrag;
    [HideInInspector] public Image image;
    [HideInInspector] FPSController player;
    private void Start()
    {
        image=transform.GetComponent<Image>();
        player=transform.root.GetComponent<FPSController>();
    }
    public void OnBeginDrag(PointerEventData eventData) 
    {
        player.canOpenUI = true;
        parenAfterDrag = transform.parent;
        transform.SetParent(transform.root.GetChild(transform.root.childCount-1).GetChild(0));//reparents outside grid hierarchy, canvasui is always last sibling
        transform.SetAsLastSibling(); //makes it always visible
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; //Makes item follow mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        player.canOpenUI = false;
        transform.SetParent(parenAfterDrag); //Sets new parent
        image.raycastTarget = true;
    }

}
