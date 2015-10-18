﻿using UnityEngine;
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
public class CardLayout : UIBehaviour, ILayoutGroup, IDropHandler
{
    /// <summary> The width that will be allocated per card </summary>
    public float CellWidth;

    [Range(0, 1)]
    public float ScrollPosition;

    [Tooltip("type of cards that can not be put into this layout")]
    public List<CardTypeEnum> CanNotContainCards;

    /// <summary> each card uses the full Height of this control </summary>
    private float cellHeight { get { return Rect.sizeDelta.x; } }
    /// <summary> The total width of all cards side by side </summary>
    private float LayoutWidth { get { return Rect.childCount * CellWidth; } }

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
        if (CellWidth <= 0)
            return;

        // calculate Y position to place first child
        var startY = Rect.anchorMin.y == Rect.anchorMax.y
            ? Rect.anchoredPosition.y  // use the anchor position
            : 0; // this assumes the pickers are anchored in the middle.
        
        // calculate X position to place first child
        var minX = -LayoutWidth / 2f + (CellWidth / 2);
        var maxX = LayoutWidth / 2f + (CellWidth / 2);
        var startX = Mathf.Lerp(minX, maxX, ScrollPosition);

        // place children relative to first child
        var currentColumn = 0;
        float currentX;
        for (int i = 0; i < Rect.childCount; i++)
        {
            var child = Rect.GetChild(i) as RectTransform;
            if (child == null)
                continue;

            // set the childs size
            child.sizeDelta = new Vector2(CellWidth, cellHeight);

            // set the childs position
            currentX = startX + currentColumn * CellWidth;
            currentX = wrap(currentX, minX, maxX);
            child.anchoredPosition = new Vector2(currentX, startY);
            child.anchorMin = new Vector2(0.5f, 0);
            child.anchorMax = new Vector2(0.5f, 1);

            currentColumn++;
        }

        //Rect.sizeDelta = new Vector2(cellHeight, LayoutWidth);
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

    protected override void OnValidate()
    {
        base.OnValidate();

        if (CellWidth < 1)
            CellWidth = 1;

        SetDirty();
    }

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

    private void SetDirty()
    {
        if (CanvasUpdateRegistry.IsRebuildingLayout())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(Rect);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var droppedCard = eventData.pointerDrag;
        Debug.Log(droppedCard.name + " OnDropped ontop of " + this.name);

        // make sure the droppedCard type is allowed in this container
        var card = droppedCard.GetComponent<Card>();
        if (CanNotContainCards.Contains(card.Type))
            return;

        // add the droppedCard to this container
        var draggable = droppedCard.GetComponent<Draggable>();
        draggable.DropToParent = this.transform;
    }
}
