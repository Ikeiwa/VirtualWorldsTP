using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBound
{
    public Vector2 start;
    public Vector2 end;
    public Vector2 center;
    public Vector2 size;
    public Vector2 extend;

    public BuildingBound(Vector2 start, Vector2 end) { BuildBounds(start,end); }

    private void BuildBounds(Vector2 start, Vector2 end)
    {
        size = new Vector2(Mathf.Abs(end.x-start.x),Mathf.Abs(end.y-start.y));
        extend = size / 2;
        center = (start + end)/2;
        this.start = center - extend;
        this.end = center + extend;
    }

    public static BuildingBound ForceCombine(BuildingBound A, BuildingBound B)
    {
        Vector2 newSize = B.center - A.center;
        newSize = new Vector2(Mathf.Abs(newSize.x), Mathf.Abs(newSize.y)) + A.extend + B.extend;
        newSize /= 2;

        Vector2 newCenter = (A.center + B.center) / 2;

        return new BuildingBound(newCenter-newSize,newCenter+newSize);
    }

    public bool SnapTo(BuildingBound other)
    {
        float distX = other.center.x - center.x;
        float distY = other.center.y - center.y;

        if (Mathf.Abs(distX) >= -other.extend.x && Mathf.Abs(distX) <= other.extend.x)
        {
            if (distY > 0)
            {
                BuildBounds(start, new Vector2(end.x, center.y + distY - other.extend.y));
            }
            else
            {
                BuildBounds(new Vector2(start.x, center.y + distY + other.extend.y), end);
            }

            return true;
        }
        else if (Mathf.Abs(distY) >= -other.extend.y && Mathf.Abs(distY) <= other.extend.y)
        {
            if (distX > 0)
            {
                BuildBounds(start, new Vector2(center.x + distX - other.extend.x, end.y));
            }
            else
            {
                BuildBounds(new Vector2(center.x + distX + other.extend.x, start.y), end);
            }

            return true;
        }

        return false;
    }
}
