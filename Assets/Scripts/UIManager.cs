using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Text scoreLabel;
	public Text message;
	public GameObject messageImage;
	//public Scro



	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		scoreLabel.text = GameManager.instance.sumDistance.ToString ("f1");
		if (GameManager.instance.GetEnemy().activeSelf) {
			messageImage.SetActive(false);
			string n = GameManager.instance.GetEnemy().GetComponent<EnemyScript> ().mentor.GetName ();
			message.text = n + "が現れた！";
		} else {
			messageImage.SetActive(false);
		}
	}
}
