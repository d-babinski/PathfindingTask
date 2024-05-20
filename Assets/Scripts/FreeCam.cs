using UnityEngine;
using UnityEngine.Serialization;

public class FreeCam : MonoBehaviour
{
    [SerializeField] private float minSize = 5f;
    [SerializeField] private float maxSize = 25f;
    [FormerlySerializedAs("camera")][SerializeField] private Camera mainCam = null;
    
    private void Update()
    {
        float _mouseScrollDelta = -Input.mouseScrollDelta.y;
        
        if (Mathf.Abs(_mouseScrollDelta) > Mathf.Epsilon)
        {
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize + _mouseScrollDelta, minSize, maxSize);
        }

        float _horizontalInput = Input.GetAxis("Horizontal");
        float _verticalInput = Input.GetAxis("Vertical");
        
        if (Mathf.Abs(_horizontalInput) > Mathf.Epsilon)
        {
            mainCam.transform.position += Vector3.right*(_horizontalInput*Time.deltaTime*mainCam.orthographicSize);
        }

        if (Mathf.Abs(_verticalInput) > Mathf.Epsilon)
        {
            mainCam.transform.position += Vector3.up*(_verticalInput*Time.deltaTime*mainCam.orthographicSize);
        }
    }
}
