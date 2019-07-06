using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InputDetector : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 beginPos;
    public Action<Direction> onInput;

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var delta = eventData.position - beginPos;
        if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x > 0)
                OnInput(Direction.Right);
            else
                OnInput(Direction.Left);
        }
        else
        {
            if (delta.y > 0)
                OnInput(Direction.Up);
            else
                OnInput(Direction.Down);
        }
    }

    private void OnInput(Direction dir)
    {
        onInput?.Invoke(dir);
    }
}
