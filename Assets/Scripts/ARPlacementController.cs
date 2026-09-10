using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementController : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject prefabToSpawn;
    public bool touchScreen;
    [Space]
    public Button deleteBTN;
    public Button colorBTN;
    [Space]
    public float rotationSpeed = 10f;

    private Renderer objectRenderer;

    private GameObject objectSpawned;

    private bool cubeIsActive = false;

    private void Update()
    {
        VerifyObjectIsPlaced();

        if (touchScreen)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            TryPlaceObject(touch.position);
        }

        if (touch.phase == TouchPhase.Moved && cubeIsActive)
        {
            RotateObject(touch.deltaPosition.x, touch.deltaPosition.y);
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceObject(Input.mousePosition);
        }

        if (Input.GetMouseButton(0) && cubeIsActive)
        {
            RotateObject(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    private void RotateObject(float horizontal, float vertical)
    {
        if (objectSpawned == null)
            return;

        float rotX = vertical * rotationSpeed * Time.deltaTime;
        float rotY = horizontal * rotationSpeed * Time.deltaTime;

        objectSpawned.transform.Rotate(Vector3.up, -rotY, Space.World);
        objectSpawned.transform.Rotate(Vector3.right, rotX, Space.Self);
    }

    private void TryPlaceObject(Vector2 _screenPos)
    {
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

        bool hitPlane = raycastManager.Raycast(_screenPos, hitResults, TrackableType.PlaneWithinPolygon);

        if (!hitPlane) return;

        Pose hitPose = hitResults[0].pose;

        if (!cubeIsActive)
        {
            objectSpawned = Instantiate(prefabToSpawn, hitPose.position, hitPose.rotation);
            cubeIsActive = true;
        }

        objectSpawned.transform.position = hitPose.position;
        objectSpawned.transform.rotation = hitPose.rotation;
    }

    private void VerifyObjectIsPlaced()
    {
        if (cubeIsActive)
        {
            deleteBTN.interactable = true;
            colorBTN.interactable = true;
        }
        else
        {
            deleteBTN.interactable = false;
            colorBTN.interactable = false;
        }
    }

    public void DeleteObject()
    {
        cubeIsActive = false;
        Destroy(objectSpawned);
    }

    public void ChangeObjectColor()
    {
        Transform visualsObject = objectSpawned.transform.GetChild(1);
        objectRenderer = visualsObject.GetComponent<Renderer>();

        Color randomColor = Random.ColorHSV();
        objectRenderer.material.SetColor("_BaseColor", randomColor);
    }
}
