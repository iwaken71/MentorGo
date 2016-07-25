using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetumeiUIScript : MonoBehaviour {

	public static SetumeiUIScript instance = null;

	public Text nameLabel;
	public RawImage image;
	public Text zokusei;
	public Text setumei;

	void Awake(){
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
	}


	public void Setsetumei(int id){
		Mentor mentor = GameManager.instance.mentorData [id];
		nameLabel.text = mentor.GetName ();
		image.texture = GameManager.instance.textureDic [id];
		zokusei.text = mentor.GetZokusei ();
		setumei.text = mentor.GetSetumei ();
	}
}
