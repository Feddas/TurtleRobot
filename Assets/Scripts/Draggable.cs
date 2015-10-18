using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// based off of Quill18's Drag & Drop Tutorial https://youtu.be/bMuYUOIAdnc
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// What transform this draggable object will be parented to whan it is dropped
    /// </summary>
    public Transform DropToParent { get; set; }

    private Vector2 dragOffset;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag " + this.name);
        DropToParent = this.transform.parent; // return draggable to it's original parent if it gets dropped in an illegal spot
        this.transform.SetParent(this.transform.parent.parent); // ensure the layout is handled if this card is dropped in the same zone it is picked up in.
        dragOffset = (Vector2)this.transform.position - eventData.position;
        canvasGroup.blocksRaycasts = false; // allow PointerEventData to go through the card into whatever container the draggable goes over
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag " + this.name);
        this.transform.position = eventData.position + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag " + this.name);
        this.transform.SetParent(DropToParent);
        canvasGroup.blocksRaycasts = true;
    }
}
