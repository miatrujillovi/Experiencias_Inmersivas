using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementController : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject prefabToSpawn;
    public GameObject[] modelPrefabs;
    [Space]
    public Button changeBTN;
    public Button modeBTN;
    [Space]
    public float rotationSpeed = 10f;
    public float zoomSpeed = 1f;
    public float minScale = 0.5f;
    public float maxScale = 3f;

    private bool rotateMode = false;

    private GameObject objectSpawned;

    private bool cubeIsActive = false;

    private float distanceBetweenFingers;
    private float previousDistanceBetweenFingers;

    private int currentModelIndex = 0;

    private void Update()
    {
        VerifyObjectIsPlaced();

        HandleTouchInput();

        HandleMouseZoom();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        //Pinch/Zoom Object
        if (Input.touchCount == 2 && cubeIsActive)
        {
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);

            distanceBetweenFingers = Vector2.Distance(touch1.position, touch2.position);
            
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                previousDistanceBetweenFingers = distanceBetweenFingers;
                return;
            }

            float distanceDelta = distanceBetweenFingers - previousDistanceBetweenFingers;

            ZoomObject(distanceDelta);

            previousDistanceBetweenFingers = distanceBetweenFingers;

            return;
        }

        //Place, move or rotate Object
        var touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (!cubeIsActive)
            {
                TryPlaceObject(touch.position);
                return;
            }
        }

        if (touch.phase != TouchPhase.Moved || !cubeIsActive)
            return;

        if (rotateMode)
        {
            RotateObject(0, touch.deltaPosition.y);
        }
        else
        {
            MoveObject(touch.position);
        }
    }

    private void HandleMouseZoom()
    {
        if (!cubeIsActive || objectSpawned == null)
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            ZoomObject(scroll * 0.1f);
        }
    }

    #region Object Actions
    private void RotateObject(float horizontal, float vertical)
    {
        if (objectSpawned == null)
            return;

        float rotX = vertical * rotationSpeed * Time.deltaTime;
        float rotY = horizontal * rotationSpeed * Time.deltaTime;

        objectSpawned.transform.Rotate(Vector3.up, -rotY, Space.World);
        objectSpawned.transform.Rotate(Vector3.right, rotX, Space.Self);
    }

    public void ZoomObject(float distance)
    {
        //Calculate new scale for the object
        Vector3 currentScale = objectSpawned.transform.localScale;
        float zoomAmount = distance * zoomSpeed;
        Vector3 newScale = currentScale + Vector3.one * zoomAmount;

        //Change scale of the object
        float clampedScale = Mathf.Clamp(newScale.x, minScale, maxScale);
        objectSpawned.transform.localScale = new Vector3(clampedScale, clampedScale, clampedScale);
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

    private void MoveObject(Vector2 screenPosition)
    {
        if (objectSpawned == null)
            return;

        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

        bool hitPlane = raycastManager.Raycast(screenPosition, hitResults, TrackableType.PlaneWithinPolygon);
        if (!hitPlane)
            return;

        Pose hitPose = hitResults[0].pose;
        objectSpawned.transform.position = hitPose.position;
    }

    #endregion Object Actions

    #region Button Actions
    private void VerifyObjectIsPlaced()
    {
        if (cubeIsActive)
        {
            modeBTN.interactable = true;
            changeBTN.interactable = true;
        }
        else
        {
            modeBTN.interactable = false;
            changeBTN.interactable = false;
        }
    }

    public void ChangeObjectModel()
    {
        if (!cubeIsActive || objectSpawned == null)
            return;

        if (modelPrefabs == null || modelPrefabs.Length == 0)
            return;

        //Select next model on the list
        currentModelIndex = (currentModelIndex + 1) % modelPrefabs.Length;
        GameObject nextModel = modelPrefabs[currentModelIndex];
        if (nextModel == null)
            return;

        //Old models Transform
        Transform oldTransform = objectSpawned.transform;
        Vector3 position = oldTransform.position;
        Quaternion rotation = oldTransform.rotation;
        Vector3 scale = oldTransform.localScale;

        //Replace current model
        objectSpawned = Instantiate(nextModel, position, rotation);
        objectSpawned.transform.localScale = scale;

        cubeIsActive = true;
        Destroy(oldTransform.gameObject);
    }

    public void ActivateObjectRotation()
    {
        Text modeText = modeBTN.GetComponentInChildren<Text>();

        if (!rotateMode)
        {
            rotateMode = true;
            modeText.text = "Move";
        }
        else
        {
            rotateMode = false;
            modeText.text = "Rotate";

        }
    }

    #endregion Button Actions
}
