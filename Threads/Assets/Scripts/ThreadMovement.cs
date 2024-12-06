using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class ThreadMovement : MonoBehaviour
{
    Plane clickPlane = new Plane(Vector3.up, Vector3.zero);
    float planeDistance = 0f;

    Vector3 clickOffset = Vector3.zero;
    Vector3 gridOffset = Vector3.zero;
    Orientation orientation;

    BoxCollider col;
    float stoneHalfSize = 0f;
    Vector3 validBackPos = Vector3.negativeInfinity;
    Vector3 validFrontPos = Vector3.positiveInfinity;

    Vector3 targetPosition;
    Vector3 velocity = Vector3.zero;
    float smoothTime = .1f;

    //bool inputEnabled = true;

    enum Orientation
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    void Start()
    {
        col = GetComponent<BoxCollider>();
        targetPosition = transform.position;

        SetOrientation();
        SetGridOffset();
    }

    void Update()
    {
        //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void OnMouseDown()
    {
        //if (!inputEnabled) return;

        Ray ray = GetScreenPointToRay();

        if (clickPlane.Raycast(ray, out planeDistance))
        {
            clickOffset = transform.position - ray.GetPoint(planeDistance);
        }

        GetValidMoveRange();
    }

    void OnMouseDrag()
    {
        //if (!inputEnabled) return;

        Ray ray = GetScreenPointToRay();

        if (clickPlane.Raycast(ray, out planeDistance))
        {
           MoveThread(ray.GetPoint(planeDistance));
        }
    }

    void OnMouseUp()
    {
        //if (!inputEnabled) return;

        validBackPos = Vector3.negativeInfinity;
        validFrontPos = Vector3.positiveInfinity;

        targetPosition = transform.position;
    }

    void GetValidMoveRange()
    {
        Ray ray = new Ray();
        ray.origin = transform.position;
        RaycastHit hit;
        LayerMask wallMask = LayerMask.GetMask("Wall");

        col.enabled = false;

        ray.direction = transform.right;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, wallMask))
        {
            validFrontPos = RoundVector(hit.point - transform.right * stoneHalfSize);
        }

        ray.direction = -transform.right;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, wallMask))
        {
            validBackPos = RoundVector(hit.point + transform.right * stoneHalfSize);
        }

        col.enabled = true;
    }

    void MoveThread(Vector3 screenPosition)
    {
        Vector3 offsetPosition = screenPosition + clickOffset;
        Vector3 lockedPosition = LockPositionToLocalForwardAxis(offsetPosition);
        Vector3 clampedPosition = ClampToValidPosition(lockedPosition);
        Vector3 gridPosition = SnapToGrid(clampedPosition);
        transform.position = gridPosition;
    }

    Vector3 LockPositionToLocalForwardAxis(Vector3 pos)
    {
        switch (orientation)
        {
            case Orientation.NORTH:
            case Orientation.SOUTH:
                pos.x = transform.position.x;
                break;
            case Orientation.EAST:
            case Orientation.WEST:
                pos.z = transform.position.z;
                break;
        }

        return RoundVector(pos);
    }

    Vector3 ClampToValidPosition(Vector3 pos)
    {
        switch(orientation)
        {
            case Orientation.NORTH:
                pos.z = Mathf.Clamp(pos.z, Mathf.Min(validBackPos.z, validFrontPos.z), Mathf.Max(validBackPos.z, validFrontPos.z));
                break;
            case Orientation.SOUTH:
                pos.z = Mathf.Clamp(pos.z, Mathf.Min(validFrontPos.z, validBackPos.z), Mathf.Max(validFrontPos.z, validBackPos.z));
                break;
            case Orientation.EAST:
                pos.x = Mathf.Clamp(pos.x, validBackPos.x, validFrontPos.x);
                break;
            case Orientation.WEST:
                pos.x = Mathf.Clamp(pos.x, validFrontPos.x, validBackPos.x);
                break;
        }

        return RoundVector(pos);
    }


        
    void SetGridOffset()
    {
        gridOffset = new Vector3(
          Mathf.Abs(transform.position.x % 1f),
          0f,
          Mathf.Abs(transform.position.z % 1f)
        );
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        Vector3 gridPosition = new Vector3(
          Mathf.Floor(pos.x) + gridOffset.x, pos.y,
          Mathf.Floor(pos.z) + gridOffset.z
        );

        return RoundVector(gridPosition);
    }

    void SetOrientation()
    {
        Vector3 localFwd = RoundVector(transform.forward);
        Vector3 worldFwd = RoundVector(Vector3.forward);

        float angle = Vector3.SignedAngle(worldFwd, localFwd, Vector3.up);

        if ((Mathf.Abs(angle) < 45) && Mathf.Abs(angle) >= 0)
            orientation = Orientation.EAST;
        else if (angle > 45 && angle <= 135)
            orientation = Orientation.NORTH;
        else if (Mathf.Abs(angle) > 135)
            orientation = Orientation.WEST;
        else
            orientation = Orientation.SOUTH;

        switch (orientation)
        {
            case Orientation.NORTH:
            case Orientation.SOUTH:
                stoneHalfSize = col.bounds.extents.z;
                break;
            case Orientation.EAST:
            case Orientation.WEST:
                stoneHalfSize = col.bounds.extents.x;
                break;
        }
    }

    Ray GetScreenPointToRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    static Vector3 RoundVector(Vector3 v)
    {
        return new Vector3(
          Mathf.Round(v.x * 2) / 2,
          Mathf.Round(v.y * 2) / 2,
          Mathf.Round(v.z * 2) / 2
        );
    }

    void ShowValidMoveRange()
    {
        if (validBackPos != Vector3.negativeInfinity)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(validBackPos, .25f);
        }

        if (validFrontPos != Vector3.positiveInfinity)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(validFrontPos, .25f);
        }

        if (validBackPos != Vector3.negativeInfinity && validFrontPos != Vector3.positiveInfinity)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(validBackPos, validFrontPos);
        }
    }

    //public void DisableInput()
    //{
    //    inputEnabled = false;
    //}

    void OnDrawGizmos()
    {
        ShowValidMoveRange();

        //Gizmos.color = Color.yellow;
    }
}
