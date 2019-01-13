using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("Ray from mouse click: " + ray.ToString());
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            Debug.Log("Hit generated from ray: " + hit.ToString());
            bool hitCollider = hit.collider == true;
            Debug.Log("Hit a collider?: " + hitCollider);

            if (hit.collider != null)
            {
                Debug.Log("hit collider: " + hit.collider.name);
            }
        }
    }
}