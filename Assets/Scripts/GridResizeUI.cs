using Astar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridResizeUI : MonoBehaviour
{
    private const int MIN_SIZE = 1;
    private const int MAX_SIZE = 10000;
    
    [SerializeField] private TMP_InputField widthField = null;
    [SerializeField] private TMP_InputField heightField = null;
    [SerializeField] private Planner planner = null;

    public void OnResizeGrid()
    {
        int _newWidth = int.Parse(widthField.text);
        int _newHeight = int.Parse(heightField.text);

        if (_newHeight > MAX_SIZE || _newHeight < MIN_SIZE || _newWidth > MAX_SIZE || _newWidth < MIN_SIZE)
        {
            return;
        }

        planner.GenerateNewGrid(_newWidth, _newHeight);
    }
    
}
