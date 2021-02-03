using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    [Header("Body Parts")]
    [SerializeField] private GameObject head = null;

    [Header("Aim Parameters")]
    [SerializeField] [Range(0.0f, 90.0f)] private float maxHeadAngle = 45.0f;
    [SerializeField] private float rotateSpeed = 90.0f;
    [SerializeField] [Range(1.0f, 90.0f)] private float minBodyRotate = 45.0f;

    private GameObject body;
    private Vector3 mouseInWorld;
    private Quaternion bodyTargetRotation;
    private bool isRotating;
    private bool isOnTarget;

    private void OnDrawGizmos()
    {
        Gizmos.color = isOnTarget ? Color.green : Color.red;

        if (Application.isPlaying)
        {
            Gizmos.DrawSphere(mouseInWorld, 0.5f);
        }

        Gizmos.DrawRay(head.transform.position, head.transform.rotation * Quaternion.Euler(0, maxHeadAngle, 0) * Vector3.forward * 5.0f);
        Gizmos.DrawRay(head.transform.position, head.transform.rotation * Quaternion.Euler(0, -maxHeadAngle, 0) * Vector3.forward * 5.0f);

    }

    private void OnValidate()
    {
        if (minBodyRotate > maxHeadAngle * 2.0f)
            minBodyRotate = maxHeadAngle * 2.0f;
    }

    private void Awake()
    {
        body = gameObject;
        bodyTargetRotation = body.transform.rotation;
        isRotating = false;
        isOnTarget = false;
    }

    private void Update()
    {
        UpdateBodyRotation();

        mouseInWorld = GetMouseLocation();

        Quaternion rotationToMouse = Quaternion.LookRotation(mouseInWorld - transform.position, Vector3.up);
        float angleFromBody = rotationToMouse.eulerAngles.y - body.transform.eulerAngles.y;
        angleFromBody = Mathf.Repeat(angleFromBody + 180.0f, 360.0f) - 180.0f;

        if (Mathf.Abs(angleFromBody) <= maxHeadAngle)
        {
            head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, rotationToMouse, rotateSpeed * Time.deltaTime);
            isOnTarget = true;
        }
        else
        {
            Quaternion maxRotation = Quaternion.Euler(0.0f, maxHeadAngle * Mathf.Sign(angleFromBody), 0.0f);
            head.transform.localRotation = Quaternion.RotateTowards(head.transform.localRotation, maxRotation, rotateSpeed * Time.deltaTime);

            bodyTargetRotation = Quaternion.Euler(0.0f, body.transform.eulerAngles.y + minBodyRotate * Mathf.Sign(angleFromBody), 0.0f);
            isRotating = true;
            isOnTarget = false;
        }
    }

    private void UpdateBodyRotation()
    {
        if (isRotating)
        {
            if (Mathf.Abs(bodyTargetRotation.eulerAngles.y - body.transform.eulerAngles.y) < Mathf.Epsilon)
            {
                isRotating = false;
            }
            else
            {
                body.transform.rotation = Quaternion.RotateTowards(body.transform.rotation, bodyTargetRotation, rotateSpeed * Time.deltaTime);
            }
        }
    }

    private Vector3 GetMouseLocation()
    {
        Camera cam = Camera.main;
        Vector2 mousePos = Input.mousePosition;
        Vector3 point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.transform.position.y));
        return point;
    }

}
