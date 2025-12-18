using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [Inject] private HybridHandler hybridHandler;
    
    private void LateUpdate()
    {
        if (!hybridHandler.TryGetCameraPosition(out var cameraPosition)) return;
        transform.position = cameraPosition;
    }
}