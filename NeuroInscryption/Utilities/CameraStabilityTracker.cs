using UnityEngine;

namespace NeuroInscryption.Utilities;

public sealed class CameraStabilityTracker : Singlyboi<CameraStabilityTracker>
{
    private Vector3 _lastCamPos;
    private Vector3 _lastCamAngle;

    private Camera? _cam;
    public bool IsStable { get; private set; }

    private void FixedUpdate()
    {
        if (_cam == null)
        {
            _cam = Camera.main;
            if (_cam == null) return;
        }

        Vector3 pos = _cam.transform.position;
        Vector3 angle = _cam.transform.rotation.eulerAngles;

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
