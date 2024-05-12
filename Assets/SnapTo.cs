using Fusion.XR.Host.Utils;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapTo : MonoBehaviour
{
    //can provide position object should snap to, or can make snap to axis
    public bool snapToAxis = true;
    public bool canRotate = true;

    public List<Vector3> positions = new List<Vector3>();

    public Vector3 localAxis;
    public float axisLength;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Quaternion _initialRotation;
    private Vector3 _worldAxis;
    
    public float sliderValue = 0;

    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (!_rb)
        {
            Debug.LogError("Could not find Rigidbody component");
            return;
        }

        if (snapToAxis)
        {
            _startPosition = transform.position;
            _endPosition = transform.position + (transform.TransformDirection(localAxis) * axisLength);
            _worldAxis = _endPosition - _startPosition;
        }

        if (!canRotate)
        {
            _initialRotation = transform.rotation;
            _rb.freezeRotation = true;
            _rb.drag = 10;
            _rb.angularDrag = 40;
        }
    }

    public void OnUngrab()
    {
        StartCoroutine(waitForDoneGrabbing());
    }

    private IEnumerator waitForDoneGrabbing()
    {
        yield return new WaitForSeconds(0.4f);
        Snap();
    }

    private void Snap()
    {
        if (snapToAxis)
        {
            Debug.Log("Snapping to axis");
            SnapToAxis();
        } else
        {
            Debug.Log("Snapping to position");
            SnapToPosition();
        }
    }

    //find closest position on axis to snap to and update slider value
    private void SnapToAxis()
    {
        //project position onto defined axis
        Vector3 projectedPoint = Vector3.Project((transform.position - _startPosition), _worldAxis) + _startPosition;
        sliderValue = Vector3.Distance(_startPosition, projectedPoint) / axisLength;
        if (!canRotate)
        {
            transform.rotation = _initialRotation;
        }
        if (sliderValue < 0)
        {
            sliderValue = 0;
            transform.SetPositionAndRotation(_startPosition, transform.rotation);
            //_rb.VelocityFollow(_startPosition, transform.rotation, Time.deltaTime);
        } else if (sliderValue > 1)
        {
            sliderValue = 1;
            transform.SetPositionAndRotation(_endPosition, transform.rotation);
            //_rb.VelocityFollow(_endPosition, transform.rotation, Time.deltaTime);
        } else
        {
            //_rb.VelocityFollow(projectedPoint, transform.rotation, Time.deltaTime);
            transform.SetPositionAndRotation(projectedPoint, transform.rotation);
        }
        Debug.Log($"Snapping onto axis, slider value is now {sliderValue}");
    }
    //find closest position from list to snap tos
    private void SnapToPosition()
    {
        int closestIndex = 0;
        float minDist = Vector3.Distance(transform.position, positions[0]);
        for (int i = 1; i < positions.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, positions[i]);
            if (dist < minDist)
            {
                closestIndex = i;
                minDist = dist;
            }
        }

        Debug.Log($"Snapping to position {positions[closestIndex]}");
        if (canRotate)
        {
            //_rb.VelocityFollow(positions[closestIndex], transform.rotation, Time.deltaTime);
            transform.SetPositionAndRotation(positions[closestIndex], transform.rotation);
        } else
        {
            //_rb.VelocityFollow(positions[closestIndex], _initialRotation, Time.deltaTime);
            transform.SetPositionAndRotation(positions[closestIndex], _initialRotation);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine( _startPosition, _endPosition );
        Gizmos.DrawSphere(_startPosition, 0.04f);
        Gizmos.DrawSphere(_endPosition, 0.0f);
    }
}
