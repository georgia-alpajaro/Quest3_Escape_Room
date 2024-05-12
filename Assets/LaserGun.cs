using Fusion.XR.Host.Grabbing;
using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    //[SerializeField] private HandGrabUseInteractable _grabInteractable;
    //above is for checking trigger press with hand tracking
    //will have to check if using controllers or hands, then set the grab interactable and check trigger

    [SerializeField] private PhysicsGrabbable _grabbable;
    [SerializeField] private Vector3 _position;

    public void MoveToGrabPosition()
    {
        _grabbable.localPositionOffset = _position;
        _grabbable.localRotationOffset = Quaternion.identity;
    }
}
