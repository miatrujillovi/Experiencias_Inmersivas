using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlacementController : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject prefabToSpawn;
    public bool touchScreen;

    private GameObject objectSpawned;

    private bool cubeIsActive = false;

    private void Update()
    {
        if (touchScreen)
        {
            if (Input.touchCount == 0) return;

            var touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began) return;

            TryPlaceObject(touch.position);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceObject(Input.mousePosition);
            }
        }
    }

    public void TryPlaceObject(Vector2 _screenPos)
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
}
