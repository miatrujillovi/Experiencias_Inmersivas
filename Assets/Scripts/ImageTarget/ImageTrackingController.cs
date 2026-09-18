using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingController : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject prefab;

    Dictionary<TrackableId, GameObject> instances;

    private void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnImagensChanged);
    }

    private void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnImagensChanged);
    }

    void OnImagensChanged(ARTrackablesChangedEventArgs<ARTrackedImage> arg)
    {
        //Agregar y checar imagenes
        foreach (ARTrackedImage image in arg.added)
        {
            AddImage(image);
            UpdateImage(image);
        }

        //Checar imagenes agregadas
        foreach (ARTrackedImage image in arg.updated)
        {
            UpdateImage(image);
        }

        //Remover imagenes
        foreach (var image in arg.removed)
        {
            RemoveImage(image.Key);
        }

    }

    void AddImage(ARTrackedImage image)
    {
        //Si ya esta dentro del diccionario, no volver a agregar
        if (instances.ContainsKey(image.trackableId))
            return;

        GameObject instance = Instantiate(prefab, image.transform);

        //Agregar nuevo prefab al diccionario
        instances.Add(image.trackableId, instance);
    }

    void UpdateImage(ARTrackedImage image)
    {
        //Error al acceder al objeto
        if (!instances.TryGetValue(image.trackableId, out GameObject instanceGO))
            return;

        //Trackear el estado de la imagen
        bool isTracking = image.trackingState == TrackingState.Tracking;
        instanceGO.SetActive(isTracking);
    }

    void RemoveImage(TrackableId imageID)
    {
        if (!instances.TryGetValue(imageID, out GameObject instanceGO))
            return;

        //Destruyo el objeto y quito su ID de las instancias en el diccionario
        Destroy(instanceGO);
        instances.Remove(imageID);
    }
}
