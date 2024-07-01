using UnityEngine;
using UnityEngine.UIElements;

public class CameraInteraction : MonoBehaviour
{
    public GameObject decalPrefab; // Prefab for the decal or wire sphere
    public LayerMask terrainLayer; // Layer mask for the terrain
    public RaycastHit hit;
    GameObject go;
    Vector3 position;
    private void OnEnable()
    {
        go = Instantiate(decalPrefab, transform.up * 100, Quaternion.identity);
        position = transform.up * 100;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
        {
            // Get the position where the ray hits the terrain
            position = hit.point;
           // Debug.Log(hit.transform.gameObject.name + " " + hit.transform.tag);

            // Place the decal or wire sphere at the hit position
            go.transform.position = position;


        }
    }
}
