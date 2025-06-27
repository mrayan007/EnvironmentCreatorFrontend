using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;

    // Lijst met objecten die geplaatst zijn in de wereld
    private List<GameObject> placedObjects = new List<GameObject>();

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        // Verberg het zijmenu
        UISideMenu.SetActive(false);
        // Instantieer het prefab object op de positie (0,0,0) met geen rotatie
        GameObject prefab = prefabObjects[index];
        GameObject instanceOfPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        // Haal het Object2D component op van het nieuw geplaatste object
        Object2D object2D = instanceOfPrefab.GetComponent<Object2D>();
        // Stel de objectManager van het object in op deze instantie van ObjectManager
        object2D.objectManager = this;
        // Zet de isDragging eigenschap van het object op true zodat het gesleept kan worden
        object2D.isDragging = true;

        object2D.prefabId = prefab.name;

        placedObjects.Add(instanceOfPrefab);
    }

    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

    // Methode om de huidige scène te resetten
    public void Reset()
    {
        // Laad de huidige scène opnieuw
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public async void Save()
    {
        var environmentId = CurrentEnvironment.currentEnvironment.id;

        foreach (var obj in placedObjects)
        {
            ObjectDto dto = new ObjectDto();

            Object2D object2D = obj.GetComponent<Object2D>();

            // Use the tracked prefab name
            dto.prefabId = object2D.prefabId;

            // Transform data
            Vector3 pos = obj.transform.position;
            Vector3 scale = obj.transform.localScale;
            float rotZ = obj.transform.eulerAngles.z;

            dto.positionX = pos.x;
            dto.positionY = pos.y;
            dto.scaleX = scale.x;
            dto.scaleY = scale.y;
            dto.rotationZ = rotZ;

            // Sorting layer
            var renderer = obj.GetComponent<SpriteRenderer>();
            dto.sortingLayer = renderer != null ? renderer.sortingOrder : 0;

            // Environment ID
            dto.environmentId = environmentId;

            // Send to API
            await ApiClient.instance.AddObject(dto);
        }

        Debug.Log("All objects saved.");
    }
}
