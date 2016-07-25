using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ScrollController2 : MonoBehaviour {


	public RectTransform prefab;

	public void SetNode(int index){
		Mentor mentor = GameManager.instance.mentorData [index];
		//GameManager.instance.score++;
		RectTransform item = Instantiate (prefab) as RectTransform;
		item.GetComponent<ButtonScript> ().SetID (mentor.GetId());
		item.SetParent (this.transform,false);
		Text textLabel = item.GetComponentInChildren<Text> ();
		RawImage imageLabel = item.GetComponentInChildren<RawImage> ();
		textLabel.text =  mentor.GetName();
		imageLabel.texture = (Texture)GameManager.instance.textureDic[mentor.GetId()];
		//GameManager.instance.score++;
	}

}
