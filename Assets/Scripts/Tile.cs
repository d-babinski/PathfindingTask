using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    public static event Action<int, int> OnTileClicked = null;
    public static event Action<int, int> OnTileMiddleClicked = null;
    public static event Action<int, int> OnTileRightClicked = null;
    
    public int X, Y = 0;
    
    public void OnPointerDown(PointerEventData _eventData)
    {
        if (_eventData.button == PointerEventData.InputButton.Left)
        {
            OnTileClicked?.Invoke(X, Y);
        }

        if (_eventData.button == PointerEventData.InputButton.Middle)
        {
            OnTileMiddleClicked?.Invoke(X, Y);
        }

        if (_eventData.button == PointerEventData.InputButton.Right)
        {
            OnTileRightClicked?.Invoke(X, Y);
        }
    }
}
