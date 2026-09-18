using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImagePrefabSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ImagePrefabPair
    {
        public string imageName; 
        public GameObject prefab;
    }

    [Header("References")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    [Header("Image | Prefab")]
    [SerializeField] private List<ImagePrefabPair> imagePrefabs = new();

    private readonly Dictionary<TrackableId, GameObject> spawnedObjects = new();

    private void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> changes)
    {
        foreach(ARTrackedImage trackedImage in changes.added)
        {
            SpawnPrefab(trackedImage);
        }

        foreach(ARTrackedImage trackedImage in changes.updated)
        {
            UpdatePrefab(trackedImage);
        }

        foreach(var trackedImage in changes.removed)
        {
            RemovePrefab(trackedImage.Key);
        }
    }

    private void SpawnPrefab(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        GameObject prefab = GetPrefabForImage(imageName);

        if (prefab == null)
        {
            Debug.LogWarning("AR Image " + imageName + " was detected, but no prefab is assigned");
            return;
        }

        GameObject spawnedObject = Instantiate(prefab, trackedImage.transform);

        spawnedObject.transform.localPosition = Vector3.zero;
        spawnedObject.transform.localRotation = Quaternion.identity;

        spawnedObjects.Add(trackedImage.trackableId, spawnedObject);
    }

    private void UpdatePrefab(ARTrackedImage trackedImage)
    {
        if (!spawnedObjects.TryGetValue(trackedImage.trackableId, out GameObject spawnedObject))
            return;

        bool isTracking = trackedImage.trackingState == TrackingState.Tracking;

        spawnedObject.SetActive(isTracking);
    }

    private void RemovePrefab(TrackableId trackedImageId)
    {
        if (!spawnedObjects.TryGetValue(trackedImageId, out GameObject spawnedObject))
            return;

        Destroy(spawnedObject);

        spawnedObjects.Remove(trackedImageId);
    }

    private GameObject GetPrefabForImage(string imageName)
    {
        foreach (ImagePrefabPair pair in imagePrefabs)
        {
            if (pair.imageName == imageName)
            {
                return pair.prefab;
            }
        }

        return null;
    }
}
