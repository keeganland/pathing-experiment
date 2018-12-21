using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour {

    public float moveSpeedf = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveCharacter(float x, float y)
    {
        Vector3 dirVector = new Vector3(x, y, 0).normalized * moveSpeedf;
        GetComponent<Rigidbody2D>().MovePosition(transform.position + dirVector * Time.deltaTime);
    }
}
