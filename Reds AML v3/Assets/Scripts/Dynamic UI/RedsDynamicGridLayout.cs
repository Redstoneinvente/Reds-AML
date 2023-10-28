using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedsDynamicGridLayout : LayoutGroup
{
    [SerializeField] int rowCount;
    [SerializeField] int columnCount;

    [Min(0)]
    [SerializeField] Vector2 spacing;
    [Space(20)]

    [Min(0)]
    [SerializeField] Vector2 cellSize;

    [Space(20)]
    [SerializeField] bool useCustomSize;

    [SerializeField] FitType fit;
    [SerializeField] AdjustmentType adjustment;

    [SerializeField] Vector2 minCellSize;
    [SerializeField] Vector2Int rowOrColumnCount;

    [SerializeField] RedsDynamicContentUILayout redsDynamicContentUILayout;

    public override void CalculateLayoutInputVertical()
    {
        Vector2 spacing = this.spacing;

        base.CalculateLayoutInputHorizontal();

        float srqRt = Mathf.Sqrt(rectTransform.rect.width * transform.childCount);
        float srqRtH = Mathf.Sqrt(rectTransform.rect.height * transform.childCount);

        columnCount = Mathf.CeilToInt(srqRt / minCellSize.x);
        rowCount = Mathf.CeilToInt(srqRtH / minCellSize.y);

        columnCount = transform.childCount <= columnCount ? transform.childCount : columnCount;
        columnCount = columnCount <= 0 ? 1 : columnCount;

        rowCount = transform.childCount <= columnCount ? 1 : rowCount;
        rowCount = rowCount <= 0 ? (transform.childCount / columnCount) : rowCount;

        //if (fit == FitType.Rows)
        //{
        //    rowCount = Mathf.CeilToInt(transform.childCount / (float)columnCount);
        //}
        //else if(fit == FitType.Columns)
        //{
        //    columnCount = Mathf.CeilToInt(transform.childCount / (float)rowCount);
        //}

        switch (fit)
        {
            case FitType.Auto:

                break;

            case FitType.Rows:

                rowCount = Mathf.CeilToInt(transform.childCount / (float)columnCount);

                break;

            case FitType.Columns:

                columnCount = Mathf.CeilToInt(transform.childCount / (float)rowCount);

                break;

            case FitType.Exact:

                rowCount = rowOrColumnCount.x;
                columnCount = rowOrColumnCount.y;

                break;

            default:

                break;
        }

        Vector2 pointer = new Vector2(0, 0);

        if (!useCustomSize)
        {
            float parentSizeX = rectTransform.rect.width;
            float parentSizeY = rectTransform.rect.height;

            float sizeX = parentSizeX / (float)columnCount;
            float sizeY = parentSizeY / (float)rowCount; 

            sizeY = sizeY < minCellSize.y ? minCellSize.y : sizeY;

            cellSize.x = sizeX - ((padding.left / (float)columnCount)) - ((padding.right / (float)columnCount));
            cellSize.y = sizeY - ((padding.top / (float)columnCount)) - ((padding.bottom / (float)columnCount));

            if (redsDynamicContentUILayout != default)
            {
                cellSize = redsDynamicContentUILayout.GetSize(cellSize.x, cellSize.y);
            }
        }

        switch (adjustment)
        {
            case AdjustmentType.None:
                break;

            case AdjustmentType.Spacing:

                if (fit == FitType.Rows)
                {
                    //Change Spacing Y
                    
                    float totalSizeCurrent = cellSize.y * rowCount;
                    float sizeRemaining = rectTransform.rect.height - totalSizeCurrent - padding.top - padding.bottom;

                    float newSpacing = sizeRemaining / (float)rowCount;

                    spacing.y = newSpacing;
                } 
                else if (fit == FitType.Columns)
                {
                    //Change Spacing X

                    float totalSizeCurrent = cellSize.x * columnCount;
                    float sizeRemaining = rectTransform.rect.width - totalSizeCurrent - padding.right - padding.left;

                    float newSpacing = sizeRemaining / (float)columnCount;

                    spacing.x = newSpacing;
                }
                else if (fit == FitType.Exact)
                {
                    //Change spacing of X or Y depending if they are toucing screen borders or not

                    float totalSizeCurrentX = cellSize.y * rowCount;
                    float sizeRemainingY = rectTransform.rect.height - totalSizeCurrentX - padding.top - padding.bottom;

                    float newSpacingY = sizeRemainingY / (float)rowCount;

                    float totalSizeCurrentY = cellSize.x * columnCount;
                    float sizeRemainingX = rectTransform.rect.width - totalSizeCurrentY - padding.right - padding.left;

                    float newSpacingX = sizeRemainingX / (float)columnCount;

                    if (sizeRemainingX <= 0)
                    {
                        if (sizeRemainingY <= 0)
                        {
                            //Modify None
                            break;
                        }
                        else
                        {
                            //Modify Y
                            newSpacingX = spacing.x;
                        }
                    }
                    else
                    {
                        if (sizeRemainingY <= 0)
                        {
                            //Modify None
                            break;
                        }
                        else
                        {
                            //Modify Y & X
                        }
                    }

                    spacing.x = newSpacingX;
                    spacing.y = newSpacingY;
                }
                else
                {
                    //To think about
                }

                break;

            case AdjustmentType.Padding:



                break;

            default:
                break;
        }

        for (int i = 0; i < rectChildren.Count; i++)
        {
            //pointer.x = i % columnCount;
            //pointer.y = i / columnCount;

            float posX = ((cellSize.x) * pointer.x) + (spacing.x * pointer.x) + padding.left;
            float posY = (cellSize.y * pointer.y) + (spacing.y * pointer.y) + padding.top;

            SetChildAlongAxis(rectChildren[i], 0, posX, cellSize.x - ((spacing.x / (float)columnCount) * 2));
            SetChildAlongAxis(rectChildren[i], 1, posY, cellSize.y - ((spacing.y / (float)columnCount) * 2));

            pointer.x++;
             
            if (pointer.x == columnCount)
            {
                pointer.y++;
                pointer.x = 0;
            }
        }
    }

    enum FitType 
    {
        Auto,
        Rows,
        Columns,
        Exact
    }

    enum AdjustmentType
    {
        None,
        Spacing,
        Padding
    }

    public override void SetLayoutHorizontal()
    {
       
    }

    public override void SetLayoutVertical()
    {
        
    }
}
