using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// based off of Quill18's Drag & Drop Tutorial https://youtu.be/bMuYUOIAdnc
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary> where the draggable will drop when dragging is ended </summary>
    private static RectTransform dropDestination;

    /// <summary> What transform this draggable object was initially dragged from </summary>
    public Transform DragBeginIn { get; set; }

    /// <summary> What transform this draggable object will be parented to whan it is dropped </summary>
    public Transform DragDestination
    {
        get { return dropDestination.parent; }
        set
        {
            if (dropDestination.parent == value)
                return;

            // update the placeholders container
            dropDestination.SetParent(value);

            // clean up the old parent Layout
            SetParentDirty();

            // reference the new layout for future redraws
            destinationLayout = value.GetComponent<CardLayout>();
        }
    }

    private Vector2 dragOffset;
    private CanvasGroup canvasGroup;
    private Transform storesObjectDuringDrag;
    private CardLayout destinationLayout; // used to redraw the destination layout when the dropDestination moves within that layout

    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        if (dropDestination == null)
        {
            dropDestination = new GameObject("WhereDraggedCardWillDrop", typeof(RectTransform)).GetComponent<RectTransform>();
        }
        storesObjectDuringDrag = this.transform.parent.parent;
        destinationLayout = this.transform.parent.GetComponent<CardLayout>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag " + this.name);
        dropDestination.SetParent(this.transform.parent);
        dropDestination.SetSiblingIndex(this.transform.GetSiblingIndex());

        DragDestination = DragBeginIn = this.transform.parent; // return draggable to it's original parent if it gets dropped in an illegal spot
        this.transform.SetParent(storesObjectDuringDrag); // ensure the layout is handled if this card is dropped in the same zone it is picked up in.
        dragOffset = (Vector2)this.transform.position - eventData.position;

        canvasGroup.blocksRaycasts = false; // allow PointerEventData to go through the card into whatever container the draggable goes over
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag " + this.name);

        // update the cards location to match the position of the mouse
        this.transform.position = eventData.position + dragOffset;

        // update the dropDestination location
        int newSiblingIndex = DragDestination.childCount;  // default to rightmost spot since condition below checks "<"
        for (int i = 0; i < DragDestination.childCount; i++)
        {
            if (this.transform.position.x < DragDestination.GetChild(i).position.x)
            {
                newSiblingIndex = i;

                // ignore dropDestination current position when finding a new dropDestination position
                if (dropDestination.GetSiblingIndex() < newSiblingIndex)
                {
                    //Debug.Log("not placing at index " + newSiblingIndex + " because dropDestination is at " + dropDestination.GetSiblingIndex());
                    newSiblingIndex--;
                }

                break;
            }
        }

        // apply any updates to dropDestination
        dropDestination.SetSiblingIndex(newSiblingIndex);
        SetParentDirty();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag " + this.name);

        // add the draggable to the last container it entered
        this.transform.SetParent(DragDestination);
        this.transform.SetSiblingIndex(dropDestination.GetSiblingIndex());

        // reset values used while dragging
        dropDestination.SetParent(storesObjectDuringDrag);
        canvasGroup.blocksRaycasts = true;

        SetParentDirty();
    }

    private void SetParentDirty()
    {
        destinationLayout.SetDirty();
    }
}
