using UnityEngine;

namespace NeuroInscryption.Utilities;

public class CameraStabilityTracker : Singlyboi<CameraStabilityTracker>
{
    private Vector3 _lastCamPos;
    private Vector3 _lastCamAngle;
    private readonly UniLazy<Camera> _cam = new(() => Camera.main);

    private Camera Cam => _cam.Value;
    public bool IsStable { get; private set; }

    private void FixedUpdate()
    {
        Camera cam = Cam;
        if (!cam) return;

        Vector3 pos = cam.transform.position;
        Vector3 angle = cam.transform.rotation.eulerAngles;

        bool isStable = pos == _lastCamPos && angle == _lastCamAngle;
        // if (isStable != IsStable) Plugin.Logger.LogDebug(isStable ? "stable" : "unstable");
        IsStable = isStable;

        _lastCamAngle = angle;
        _lastCamPos = pos;
    }

    private void OnDisable()
    {
        IsStable = true;
    }
}
