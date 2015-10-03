using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Will eventually lays out the cards, fanning them out. Currently just a copy from C:\Job\Jetstream\BoschAr@Dx\BoschArUnity4_6\trunk\Assets\CUSTOM\Scripts\WheelPicker.cs
/// if AnchorMin.x and AnchorMax.x are the same Pos X is used. otherwise AnchorMin.x and AnchorMax.x are used.
/// </summary>
[ExecuteInEditMode ]
public class CardLayout : UIBehaviour, ILayoutGroup
{
    // each child is set to this height
    public float CellHeight;

    [Range(0, 1)]
    public float ScrollPosition;

    // each child uses the full width of this control
    private float cellWidth { get { return Rect.sizeDelta.x; } }
    private float WheelHeight { get { return Rect.childCount * CellHeight; } }

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
    }

    public void SetLayoutVertical()
    {
        if (CellHeight <= 0)
            return;

        var startX = Rect.anchorMin.x == Rect.anchorMax.x ? Rect.anchoredPosition.x : 0; // this assumes the pickers are anchored in the middle.
        var minY = -WheelHeight / 2f + (CellHeight / 2);
        var maxY = WheelHeight / 2f + (CellHeight / 2);
        var startY = Mathf.Lerp(minY, maxY, ScrollPosition);

        var currentRow = 0;
        float currentY;
        for (int i = 0; i < Rect.childCount; i++)
        {
            var child = Rect.GetChild(i) as RectTransform;
            if (child == null)
                continue;

            child.sizeDelta = new Vector2(cellWidth, CellHeight);
            currentY = startY + currentRow * CellHeight;
            currentY = wrap(currentY, minY, maxY);
            child.anchoredPosition = new Vector2(startX, currentY);
            child.anchorMin = new Vector2(Rect.anchorMin.x, 0.5f);
            child.anchorMax = new Vector2(Rect.anchorMax.x, 0.5f);

            currentRow++;
        }

        Rect.sizeDelta = new Vector2(cellWidth, WheelHeight);
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

        if (CellHeight < 1)
            CellHeight = 1;

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
}
