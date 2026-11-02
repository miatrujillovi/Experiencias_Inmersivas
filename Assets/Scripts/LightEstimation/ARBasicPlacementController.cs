using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARBasicPlacementController : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public GameObject prefabToSpawn;

    private int objectsSpawned = 0;

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceObject(Input.mousePosition);
        }
    }

    private void TryPlaceObject(Vector2 _screenPos)
    {
        List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

        bool hitPlane = raycastManager.Raycast(_screenPos, hitResults, TrackableType.PlaneWithinPolygon);

        if (!hitPlane) return;

        Pose hitPose = hitResults[0].pose;

        GameObject objectSpawned = Instantiate(prefabToSpawn, hitPose.position, hitPose.rotation);
        objectSpawned.transform.position = hitPose.position;
        objectSpawned.transform.rotation = hitPose.rotation;

        objectsSpawned++;
    }

    public int GetObjectsSpawned()
    {
        return objectsSpawned;
    }
}
