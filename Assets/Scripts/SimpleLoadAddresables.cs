using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SimpleLoadAddresables : MonoBehaviour
{
    public string addressableName;

    private GameObject instance;

    private async void Start()
    {
        var handle = Addressables.UpdateCatalogs(null, false);
        await handle.Task;
        Addressables.Release(handle);

        Load();
    }

    private void OnDestroy()
    {
        Release();
    }

    void Load()
    {
        if (instance != null)
            return;

        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(addressableName, transform, false);
        handle.Completed += operation =>
        {
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                instance = operation.Result;
            }
            else
            {
                Addressables.Release(operation);
            }
        };
    }

    void Release()
    {
        if (!instance)
            return;

        Addressables.ReleaseInstance(instance);
        instance = null;
    }
}
