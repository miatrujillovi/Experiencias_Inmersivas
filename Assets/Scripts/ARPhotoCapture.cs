using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ARPhotoCapture : MonoBehaviour
{
    [Header("Camera Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shutterSound;
    [SerializeField] private Image flashImage;
    [Space]
    [Header("Flash Settings")]
    [SerializeField, Range(0f, 1f)]
    private float flashOpacity = 0.85f;
    [Space]
    [Header("Screenshot Settings")]
    [SerializeField] private Canvas uiCanvas;
    [Space]
    [SerializeField] private float flashDuration = 0.2f;

    private bool isCapturing = false;
    private Coroutine flashCoroutine;

    public void TakePhoto()
    {
        if (isCapturing)
            return;

        StartCoroutine(CapturePhotoRoutine());
    }


    private IEnumerator CapturePhotoRoutine()
    {
        isCapturing = true;

        // Remember whether the Canvas was active
        bool canvasWasEnabled = uiCanvas != null && uiCanvas.enabled;

        // Hide the UI before capturing the screen.
        if (uiCanvas != null)
            uiCanvas.enabled = false;

        // Wait for Unity to render a frame without the UI
        yield return new WaitForEndOfFrame();

        Texture2D photo = null;

        try
        {
            photo = ScreenCapture.CaptureScreenshotAsTexture();

            if (photo == null)
            {
                Debug.LogError("Photo capture failed.");
                yield break;
            }

            string fileName =
                $"ARPhoto_{System.DateTime.Now:yyyyMMdd_HHmmss_fff}.png";

            string filePath = System.IO.Path.Combine(
                Application.persistentDataPath,
                fileName
            );

            System.IO.File.WriteAllBytes(filePath, photo.EncodeToPNG());

            Debug.Log($"Photo saved to: {filePath}");
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"Failed to save photo: {exception.Message}");
        }
        finally
        {
            if (photo != null)
                Destroy(photo);

            // Restore the UI after capturing
            if (uiCanvas != null)
                uiCanvas.enabled = canvasWasEnabled;
        }

        // Camera feedback happens after the UI is restored
        if (audioSource != null && shutterSound != null)
            audioSource.PlayOneShot(shutterSound);

        if (flashImage != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);

            flashCoroutine = StartCoroutine(FlashEffect());
        }

        isCapturing = false;
    }


    private IEnumerator FlashEffect()
    {
        flashImage.gameObject.SetActive(true);

        Color color = flashImage.color;
        color.r = 1f;
        color.g = 1f;
        color.b = 1f;
        color.a = flashOpacity;
        flashImage.color = color;

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            color.a = Mathf.Lerp(flashOpacity, 0f, elapsed / flashDuration);

            flashImage.color = color;
            yield return null;
        }

        color.a = 0f;
        flashImage.color = color;

        flashImage.gameObject.SetActive(false);
        flashCoroutine = null;
    }
}
