using UnityEngine;
using System.Collections;

public class ballScript : MonoBehaviour {

	int count = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision col){

		if (col.gameObject.tag == "Enemy") {
			if (count == 0) {
				col.gameObject.GetComponent<EnemyScript> ().Damage ();

			}

		}
		count++;
	}
}
