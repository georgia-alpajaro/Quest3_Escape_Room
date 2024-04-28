using Fusion.XR.Host.Grabbing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AxisDrag : MonoBehaviour
{
    public Vector3 localAxis;
    public float axisLength;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    [SerializeField]
    private Quaternion _initialRotation;
    private Vector3 _worldAxis;

    //-1 if float range [0, 1]
    public int numberOfSteps = 0;
    public float sliderValue = -1;
    public List<float> _steps = new List<float>(); //steps are distance from [0, 1] along local axis
    public List<string> _stepNames = new List<string>();
    public int currentStep = -1;

    private PhysicsGrabbable _grabbable; //to check isGrabbed for event
    [SerializeField]
    private bool previouslyGrabbed = false;

    private Rigidbody _rb;
    
    void Start()
    {
        if (numberOfSteps != _steps.Count || numberOfSteps != _stepNames.Count)
        {
            Debug.LogError($"Incorrect numberOfSteps: {numberOfSteps}, since _steps has {_steps.Count} elements and there are {_stepNames.Count} names");
            return;
        }

        if (numberOfSteps > 0)
        {
            currentStep = 0;
        } else
        {
            sliderValue = 0;
        }
        
        _grabbable = GetComponent<PhysicsGrabbable>();
        if (!_grabbable)
        {
            Debug.LogError("Could not find PhysicsGrabbable component");
            return;
        }
        _rb = GetComponent<Rigidbody>();
        if (!_rb)
        {
            Debug.LogError("Could not find Rigidbody component");
            return;
        }
        _startPosition = transform.position;
        _endPosition = transform.position + (transform.TransformDirection(localAxis) * axisLength);
        _initialRotation = transform.rotation;
        _worldAxis = _endPosition - _startPosition;

        _rb.freezeRotation = true;
        _rb.drag = 10;
        _rb.angularDrag = 40;
    }

    private void Update()
    {
        if (previouslyGrabbed && !_grabbable.IsGrabbed)
        {
            SnapToStep();
        }
        previouslyGrabbed = _grabbable.IsGrabbed;
    }

    public void SnapToStep()
    {
        Debug.Log("Calling SnapToStep()");
        if (numberOfSteps > 0)
        {
            //find minimum distance step
            Vector3 closestStep = (_steps[currentStep] * _worldAxis) + _startPosition; //in world space
            float minDistance = Vector3.Distance(transform.position, closestStep);
            //Debug.LogWarning($"Starting out with closest step as 0: {_stepNames[0]}");
            for (int i = 0; i < _steps.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, (_steps[i] * _worldAxis) + _startPosition);
                Debug.LogWarning($"Step {i} is {dist} away from current position at {transform.position}");
                if (dist < minDistance)
                {
                    //Debug.LogWarning($"Found new closest step {i}: {_stepNames[i]}");
                    minDistance = dist;
                    currentStep = i;
                }
            }
            //snap to position
            transform.SetPositionAndRotation((_steps[currentStep] * _worldAxis) + _startPosition, _initialRotation);

            Debug.LogWarning($"Snapping to step {currentStep}: {_stepNames[currentStep]}, set position to {(_steps[currentStep] * _worldAxis) + _startPosition}");
        }
        else
        {
            //project position onto defined axis
            Vector3 projectedPoint = Vector3.Project((transform.position - _startPosition), _worldAxis) + _startPosition;
            transform.SetPositionAndRotation(projectedPoint, _initialRotation);
            sliderValue = Vector3.Distance(_startPosition, projectedPoint) / axisLength;
            Debug.LogWarning($"Snapping onto axis, slider value is now {sliderValue}");
        }
    }
}
