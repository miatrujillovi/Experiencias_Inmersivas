using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class AddressablesUpdateCatalog : MonoBehaviour
{
    private async void Start()
    {
        var checkHandle = Addressables.CheckForCatalogUpdates(false);

        await checkHandle.Task;

        List<string> catalogsToUpdate = checkHandle.Result;

        if (catalogsToUpdate != null && catalogsToUpdate.Count > 0)
        {
            Debug.Log("Actualizando catalogo de Addressables...");

            var updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, false);

            await updateHandle.Task;

            Addressables.Release(updateHandle);

            Debug.Log("Catalogo actualizado");
        }
        else
        {
            Debug.Log("El catálogo ya está actualizado");
        }

        Addressables.Release(checkHandle);

        SceneManager.LoadScene(1);
    }
}
