using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    // Lijst met objecten die geplaatst kunnen worden (matchen met prefab namen)
    public List<GameObject> prefabObjects;

    // Alle geplaatste objecten (oud + nieuw)
    private List<GameObject> placedObjects = new List<GameObject>();
    // Alleen nieuw toegevoegde objecten
    private List<GameObject> newObjects = new List<GameObject>();

    private void Start()
    {
        var currentEnvironment = CurrentEnvironment.currentEnvironment;
        var objects = currentEnvironment.objects;

        foreach (var objDto in objects)
        {
            GameObject prefab = prefabObjects.FirstOrDefault(p => p.name == objDto.prefabId);
            if (prefab == null) continue;

            Vector3 position = new Vector3(objDto.positionX, objDto.positionY, 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, objDto.rotationZ);
            GameObject instance = Instantiate(prefab, position, rotation);
            instance.transform.localScale = new Vector3(objDto.scaleX, objDto.scaleY, 1f);

            var renderer = instance.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.sortingOrder = objDto.sortingLayer;

            Object2D object2D = instance.GetComponent<Object2D>();
            object2D.objectManager = this;
            object2D.prefabId = prefab.name;

            placedObjects.Add(instance); // Alleen in de algemene lijst, want dit zijn oude objecten
        }
    }

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        UISideMenu.SetActive(false);

        GameObject prefab = prefabObjects[index];
        GameObject instanceOfPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        Object2D object2D = instanceOfPrefab.GetComponent<Object2D>();
        object2D.objectManager = this;
        object2D.isDragging = true;
        object2D.prefabId = prefab.name;

        placedObjects.Add(instanceOfPrefab);
        newObjects.Add(instanceOfPrefab); // Alleen nieuwe objecten opslaan later
    }

    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }

    public void Reset()
    {
        foreach (var obj in placedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        placedObjects.Clear();
        newObjects.Clear();
    }

    public async void Save()
    {
        var environmentId = CurrentEnvironment.currentEnvironment.id;

        foreach (var obj in newObjects) // Alleen nieuwe objecten opslaan
        {
            ObjectDto dto = new ObjectDto();
            Object2D object2D = obj.GetComponent<Object2D>();

            dto.prefabId = object2D.prefabId;

            Vector3 pos = obj.transform.position;
            Vector3 scale = obj.transform.localScale;
            float rotZ = obj.transform.eulerAngles.z;

            dto.positionX = pos.x;
            dto.positionY = pos.y;
            dto.scaleX = scale.x;
            dto.scaleY = scale.y;
            dto.rotationZ = rotZ;

            var renderer = obj.GetComponent<SpriteRenderer>();
            dto.sortingLayer = renderer != null ? renderer.sortingOrder : 0;

            dto.environmentId = environmentId;

            await ApiClient.instance.AddObject(dto);
        }

        Debug.Log("New objects saved.");
    }

    public void Exit()
    {
        SceneManager.LoadScene("EnvironmentScreen");
    }
}
