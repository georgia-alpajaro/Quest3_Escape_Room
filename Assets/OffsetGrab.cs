using Fusion.XR.Host.Grabbing;
using UnityEngine;

public class OffsetGrab : MonoBehaviour
{
    [SerializeField] private PhysicsGrabbable grabbable;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    private void Start()
    {
        grabbable = GetComponent<PhysicsGrabbable>();
    }

    public void MoveToGrabPosition()
    {
        grabbable.localPositionOffset = positionOffset;
        grabbable.localRotationOffset = Quaternion.Euler(rotationOffset);
    }
}
