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

    Vector3 targetPosition;

    void Start()
    {
        SetOrientation();
        SetGridOffset();
    }

    void OnMouseDown()
    {
        Ray ray = GetScreenPointToRay();

        if (clickPlane.Raycast(ray, out planeDistance))
        {
            clickOffset = transform.position - ray.GetPoint(planeDistance);
        }

    }

    void OnMouseDrag()
    {
        Ray ray = GetScreenPointToRay();

        if (clickPlane.Raycast(ray, out planeDistance))
        {
           MoveThread(ray.GetPoint(planeDistance));
        }
    }

    void MoveThread(Vector3 screenPosition)
    {
        Vector3 offsetPosition = screenPosition + clickOffset;
        Vector3 lockedPosition = LockPositionToLocalForwardAxis(offsetPosition);
        Vector3 gridPosition = SnapToGrid(lockedPosition);
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

        if (angle == 0)
            orientation = Orientation.EAST;
        else if (angle == 90)
            orientation = Orientation.NORTH;
        else if (angle == 180)
            orientation = Orientation.WEST;
        else
            orientation = Orientation.SOUTH;
    }

    void SetGridOffset()
    {
        gridOffset = new Vector3(
          Mathf.Abs(transform.position.x % 1f),
          0f,
          Mathf.Abs(transform.position.z % 1f)
        );
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

    enum Orientation
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }
}
