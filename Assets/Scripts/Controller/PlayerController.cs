using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera thirdPersonCamera;
    public CharacterController characterController;
    public float moveSpeed = 10.0f;
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    private bool isFirstPerson = true;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    void Update()
    {
        // Switch between first and third person views
        if (Input.GetKeyDown(KeyCode.J))
        {
            isFirstPerson = !isFirstPerson;

            firstPersonCamera.gameObject.SetActive(isFirstPerson);
            thirdPersonCamera.gameObject.SetActive(!isFirstPerson);
        }

        // Look towards the mouse cursor
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

        // Move the character
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        characterController.Move(movement * Time.deltaTime);
    }
}
