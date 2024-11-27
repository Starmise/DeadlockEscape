using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadMovement : MonoBehaviour
{
    Plane clickPlane = new Plane(Vector3.up, Vector3.zero);
    float planeDistance = 0f;
    Vector3 clickOffset = Vector3.zero;

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
            transform.position = (ray.GetPoint(planeDistance)) + clickOffset;
        }
    }

    Ray GetScreenPointToRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
