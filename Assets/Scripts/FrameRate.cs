using UnityEngine;
using UnityEngine.UI;

public class FrameRate : MonoBehaviour
{
    private Text fpsText;

    private void Awake()
    {
        fpsText = GetComponent<Text>();
    }

    private void Update()
    {
        float fps = 1.0f / Time.deltaTime;

        fpsText.text = "FPS: " + (int)fps;
    }
}
