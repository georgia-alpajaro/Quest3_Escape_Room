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

    void Update()
    {
        if (_grabbable.isGrabbed)
        {
            Debug.Log("is grabbed!");
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                FireGun();
            }
            /*
            if (_grabbable.currentGrabber.gameObject.name == "LeftHandAnchor")
            {
                if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger))
                {
                    FireGun();
                }
            } else
            {
                if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
                {
                    FireGun();
                }
            }
            */
        }
        /*
        if (_grabInteractable.UseProgress > 0.3f)
        {
            FireGun();
        }
        */
    }

    private void FireGun()
    {
        Debug.Log("Pew Pew");
        // play audio, have some sort of visual, and fire ray at ghost which makes it take damage
        // maybe make this a coroutine in the future so if button is held down it shoots at constant rate?
    }
}
