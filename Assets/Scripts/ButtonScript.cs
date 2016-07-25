using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

	public int id;
	public void SetID(int id){
		this.id = id;
	}

	public void SetSetumei(){
		SetumeiUIScript.instance.Setsetumei (id);

	}		

	public void AnimTrigger(){
		GameObject.Find ("Canvas").GetComponent<Animator> ().SetTrigger ("Setumei");
	}
}
