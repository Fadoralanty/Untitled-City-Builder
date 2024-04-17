using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform CameraTransform;
    
    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 cameraBounds;
    public Vector3 zoomAmount;
    public Vector3 minZoom;
    public Vector3 maxZoom;
    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    private void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = CameraTransform.localPosition;
    }

    private void LateUpdate()
    {
        HandleMovementInput();
    }
    

    private void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed);
        }        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed);
        }        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetAxis("Mouse ScrollWheel")>0f)
        {
            Vector3 zoom = newZoom + zoomAmount;
            if (zoom.y < minZoom.y && zoom.z > minZoom.z)
            {
                newZoom = new Vector3(0, minZoom.y, minZoom.z);
            }
            else
            {
                newZoom += zoomAmount;
            }
        }        
        if (Input.GetAxis("Mouse ScrollWheel")<0f)
        {
            Vector3 zoom = newZoom - zoomAmount;
            if (zoom.y > minZoom.y && zoom.z < minZoom.z)
            {
                newZoom = new Vector3(0, maxZoom.y, maxZoom.z);
            }
            else
            {
                newZoom += zoomAmount;
            }
            newZoom -= zoomAmount;
            
        }

        if (newPosition.x > cameraBounds.x)
        {
            newPosition.x = cameraBounds.x;
        }

        if (newPosition.x < -cameraBounds.x)
        {
            newPosition.x = -cameraBounds.x;
        }        
        if (newPosition.z > cameraBounds.z)
        {
            newPosition.z = cameraBounds.z;
        }

        if (newPosition.z < -cameraBounds.z)
        {
            newPosition.z = -cameraBounds.z;
        }
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
