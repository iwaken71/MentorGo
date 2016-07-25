using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	Vector3 target;
	Renderer rend;
	//public Texture2D[] textures;
	int id;
	public Mentor mentor;

	public GameObject particle;


	void Awake(){

		rend = GetComponent<Renderer> ();

	}

	public void SetMat(){
		Texture2D[] textures = GameManager.instance.textures;
		int index = Random.Range (0, textures.Length);
		id = int.Parse(textures [index].name);
		rend.material.mainTexture  = GameManager.instance.textureDic[id];
		mentor = GameManager.instance.mentorData [id];
	}

	// Use this for initialization
	void Start () {
		target = transform.position;
		int index = Random.Range (0, GameObject.FindGameObjectsWithTag ("Point").Length);
		target = GameObject.FindGameObjectsWithTag ("Point") [index].transform.position;

	}

	public void SetStartPos(){
		int index = Random.Range (0, GameObject.FindGameObjectsWithTag ("Point").Length);
		target  = GameObject.FindGameObjectsWithTag ("Point") [index].transform.position;
		transform.position = target;
	}


	// Update is called once per frame
	void Update () {

		transform.LookAt (Camera.main.transform.position);

		Vector3 direction = (target - transform.position).normalized;


		if (Vector3.Distance (transform.position, target) < 1.0f) {
			transform.position += direction * Time.deltaTime;
		}else{
			transform.position += direction * Time.deltaTime*7;

		}
		if (Vector3.Distance (transform.position, target) < 0.1f) {
			int index = Random.Range (0, GameObject.FindGameObjectsWithTag ("Point").Length);
			target = GameObject.FindGameObjectsWithTag ("Point") [index].transform.position;
		}


	}

	public void Damage(){
		GameObject obj = Instantiate (particle, transform.position, Quaternion.identity) as GameObject;
		Destroy (obj,0.8f);
		gameObject.SetActive (false);
		GameManager.instance.score++;
		GameManager.instance.AddMentor (id);
	}
}
