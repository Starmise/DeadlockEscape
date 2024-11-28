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

    Vector3 targetPosition;

    void Start()
    {
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
           //Vector3 transformPosition = (ray.GetPoint(planeDistance)) + clickOffset;
           //transform.position = SnapToGrid(targetPosition);
           MoveThread(ray.GetPoint(planeDistance));
        }
    }

    void MoveThread(Vector3 screenPosition)
    {
        Vector3 offsetPosition = screenPosition + clickOffset;
        Vector3 gridPosition = SnapToGrid(offsetPosition);
        targetPosition = gridPosition;
    }

    Vector3 SnapToGrid(Vector3 pos)
    {
        Vector3 gridPosition = new Vector3(
          Mathf.Floor(pos.x) + gridOffset.x, pos.y,
          Mathf.Floor(pos.z) + gridOffset.z
        );

        return gridPosition;
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
}
