using UnityEngine;
using UnityEngine.UI;

public class ObjectCounter : MonoBehaviour
{
    public ARBasicPlacementController placementController;
    private Text objectsText;

    private void Awake()
    {
        objectsText = GetComponent<Text>();
    }

    private void Update()
    {
        objectsText.text = "Objects: " + placementController.GetObjectsSpawned();
    }
}
