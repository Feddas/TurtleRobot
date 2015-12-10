using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Lays out the cards, fanning them out.
/// copied from C:\Job\Jetstream\BoschAr@Dx\BoschArUnity4_6\trunk\Assets\CUSTOM\Scripts\WheelPicker.cs
/// if AnchorMin.x and AnchorMax.x are the same Pos X is used. otherwise AnchorMin.x and AnchorMax.x are used.
/// </summary>
//[ExecuteInEditMode]
public class CardLayout : UIBehaviour, ILayoutGroup, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Default of how many cards should fit into the screen at a time. Cards will shrink if there are more than this number of cards childed.")]
    public int CardsVisible;

    [Tooltip("type of cards that can not be put into this layout")]
    public List<CardTypeEnum> CanNotContainCards;

    /// <summary> each card uses the full Height of this layout control </summary>
    private float cellHeightDelta { get { return Rect.sizeDelta.y; } }

    /// <summary> The width that will be allocated per card </summary>
    private float cellWidth
    {
        get
        {
            if (_cellWidth == -1)
            {
                if (CardsVisible == 0) CardsVisible = 10; // ensure no division by 0
            }

            _cellWidth = getCellWidth();
            return _cellWidth;
        }
    }
    private float _cellWidth = -1;

    private float getCellWidth()
    {
        if (Rect.childCount > CardsVisible) // shrink width so all cards fit
        {
            return Rect.rect.width / Rect.childCount;
        }
        else
        {
            return Rect.rect.width / CardsVisible;
        }
    }

    private RectTransform rect;
    private RectTransform Rect
    {
        get
        {
            if (rect == null)
            {
                rect = this.GetComponent<RectTransform>();
            }
            return rect;
        }
    }

    public void SetLayoutHorizontal()
    {
        if (cellWidth <= 0)
            return;

        // calculate Y position to place first child
        var startY = Rect.anchorMin.y == Rect.anchorMax.y
            ? Rect.anchoredPosition.y  // use the anchor position
            : 0; // this assumes the pickers are anchored in the middle.

        // calculate X position to place first child
        var minX = -Rect.rect.width / 2f + (cellWidth / 2);
        var maxX = Rect.rect.width / 2f + (cellWidth / 2);
        var startX = minX;

        // place children relative to first child
        var currentColumn = 0;
        float currentX;
        for (int i = 0; i < Rect.childCount; i++)
        {
            var child = Rect.GetChild(i) as RectTransform;
            if (child == null)
                continue;

            // set the childs size
            child.sizeDelta = new Vector2(cellWidth, cellHeightDelta);

            // set the childs position
            currentX = startX + currentColumn * cellWidth;
            currentX = wrap(currentX, minX, maxX);
            child.anchoredPosition = new Vector2(currentX, startY);
            child.anchorMin = new Vector2(0.5f, 0);
            child.anchorMax = new Vector2(0.5f, 1);

            currentColumn++;
        }
    }

    public void SetLayoutVertical()
    {
    }

    private float wrap(float input, float min, float max)
    {
        float offsetToZero = input - min;
        float range = max - min;
        return ((offsetToZero % range) + range) % range + min;
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        SetDirty();
    }
#endif

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        SetDirty();
    }

    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();

        SetDirty();
    }

    protected override void OnDidApplyAnimationProperties()
    {
        base.OnDidApplyAnimationProperties();

        SetDirty();
    }

    protected virtual void OnTransformChildrenChanged()
    {
        SetDirty();
    }

    public void SetDirty()
    {
        if (CanvasUpdateRegistry.IsRebuildingLayout())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(Rect);
    }

    private Draggable getValidDraggable(GameObject droppedCard)
    {
        if (droppedCard == null) return null;
        //Debug.Log(droppedCard.name + " OnDropped ontop of " + this.name);

        // make sure the droppedCard type is allowed in this container
        var card = droppedCard.GetComponent<Card>();
        if (CanNotContainCards.Contains(card.Type))
            return null;
        else
            return droppedCard.GetComponent<Draggable>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // moved the placeholder to this container
        var draggable = getValidDraggable(eventData.pointerDrag);
        if (draggable != null) draggable.DragDestination = this.transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // add the droppedCard to this container
        var draggable = getValidDraggable(eventData.pointerDrag);
        if (draggable != null && draggable.DragDestination == this.transform)
            draggable.DragDestination = draggable.DragBeginIn;
    }
}
