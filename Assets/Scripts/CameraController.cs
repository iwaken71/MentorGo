using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private GUIStyle labelStyle;
	Quaternion start_gyro;
	Quaternion gyro;

	// Use this for initialization
	void Start () {
		start_gyro = Input.gyro.attitude;
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_IOS
		Input.gyro.enabled = true;
		if (Input.gyro.enabled) {
			gyro = Input.gyro.attitude;
			gyro = Quaternion.Euler (90, 0, 0) * (new Quaternion (-gyro.x, -gyro.y, gyro.z, gyro.w));
			this.transform.localRotation = Quaternion.Inverse (start_gyro)*gyro;
		}
		#endif

		#if UNITY_EDITOR
		#endif
	
	}
}
