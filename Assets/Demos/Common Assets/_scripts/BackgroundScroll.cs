using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class BackgroundScroll : MonoBehaviour
{
    public float speed = -0.5f;
    Renderer r;
    // Use this for initialization
    void Start()
    {
        r = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 offset = new Vector2((Time.time * speed), 0);
        //r.material.mainTextureOffset = offset;
        r.sharedMaterial.mainTextureOffset = offset;
    }
}