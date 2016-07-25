using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class GameManager: MonoBehaviour {

	public static GameManager instance = null; 

	public float z = 6;
	public float mult = 4;
	public int score = 0;
	public Texture2D[] textures;
	public Dictionary<int,Texture> textureDic = new Dictionary<int,Texture>();
	ScrollController2 scController;
	public Mentor[] mentorData;
	public float sumDistance = 0;

	GameObject enemy;
	GameObject ballSozai;
	GameObject ball;
	Vector2 startPos,endPos;
	float slideTime;
	GameObject enemySozai;
	Positions nowPos;
	Positions prePos;
	float timer = 0;
	float tmpDistance = 0;
	bool zukan = false;

	void Awake(){
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
		ballSozai = Resources.Load ("MonsterBall") as GameObject;
		enemySozai = Resources.Load ("Enemy") as GameObject;
		textures =  Resources.LoadAll <Texture2D>("Image");
		foreach (Texture2D t in textures) {
			textureDic.Add (int.Parse (t.name), (Texture)t);
		}
		//Instantiate(Resources.Load ("Canvas"));
	}

	// Use this for initialization
	void Start () {
		sumDistance = 0;
		StartCoroutine("GetLocation");
		enemy = Instantiate (enemySozai,Vector3.zero,Quaternion.identity) as GameObject;
		enemy.SetActive (false);
		ReadCSVData ();
	}

	public GameObject GetEnemy(){
		return enemy;
	}

	void CreateEnemy(){
		if (enemy == null) {
			enemy = Instantiate (enemySozai,Vector3.zero,Quaternion.identity) as GameObject;
			enemy.GetComponent<EnemyScript> ().SetMat ();
		}
		if (!enemy.activeSelf) {
			RandomRotateRoot ();
			enemy.SetActive (true);
			enemy.GetComponent<EnemyScript> ().SetStartPos ();
			enemy.GetComponent<EnemyScript> ().SetMat ();
		}
	}

	void RandomRotateRoot(){
		#if !UNITY_EDITOR
		GameObject pointsRoot = GameObject.FindGameObjectWithTag ("Points");
		float y = Random.Range (0, 360);
		pointsRoot.transform.rotation = Quaternion.Euler (0,y,0);
		#endif
	}

	// Update is called once per frame
	void Update () {
		//scoreLabel.text = score.ToString ();
		#if UNITY_IOS
		if (sumDistance - tmpDistance > 30) {
			CreateEnemy ();
			tmpDistance = sumDistance;
		}
		#endif


		if (!enemy.activeSelf) {
			timer += Time.deltaTime;
		}

		if (timer > 3) {
			timer = 0;
			#if UNITY_IOS
			//CreateEnemy ();
			StartCoroutine("GetLocation");
			#endif
			#if UNITY_EDITOR

			CreateEnemy ();
			#endif
		}

		if (!zukan) {
			#if UNITY_IOS
			bool uiTouce = false;
			if(Input.GetMouseButtonDown(0)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray,out hit,100)){
					if(hit.collider.gameObject.layer == LayerMask.NameToLayer("UI")){
						uiTouce = true;
						return;
					}
				}
			}
			if(!uiTouce){
				if (Input.touchCount > 0) {
					if (Input.GetTouch (0).phase == TouchPhase.Began) {


						Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch (0).position);
						RaycastHit hit;
						if(Physics.Raycast(ray,out hit,100)){
							if(hit.collider.gameObject.layer == LayerMask.NameToLayer("UI")){
								uiTouce = true;
								return;
							}

						}
						if(!uiTouce){
							CreateBall ();
							startPos = Input.GetTouch (0).position;
							slideTime = Time.time;
							Transform cameraTrans = Camera.main.transform;
							ball.transform.position = cameraTrans.position + cameraTrans.forward*13 + cameraTrans.up*-4;
							ball.transform.LookAt(cameraTrans.position);
							ball.transform.SetParent(cameraTrans);
							ball.GetComponent<Rigidbody> ().velocity = Vector3.zero;
							ball.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
							ball.GetComponent<Rigidbody> ().useGravity = false;
						}
					} else if (Input.GetTouch (0).phase == TouchPhase.Ended) {
						Transform cameraTrans = Camera.main.transform;
						endPos = Input.GetTouch (0).position;
						slideTime = Time.time - slideTime;
						Vector2 dir = (endPos - startPos).normalized;
						ball.GetComponent<Rigidbody> ().velocity = (cameraTrans.transform.right*dir.x + cameraTrans.transform.up*dir.y + cameraTrans.transform.forward*dir.y*z) *( mult / (slideTime + 1.0f));
						//ball.GetComponent<Rigidbody> ().velocity = new Vector3 (dir.x, dir.y, dir.y * z) *( mult / (slideTime + 1.0f));
						ball.GetComponent<Rigidbody> ().useGravity = true;
					}
				}
			}
			#endif

			#if UNITY_EDITOR
			if(Input.GetMouseButtonDown(0)){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray,out hit,100)){

					if(hit.collider.tag == "Enemy"){
						hit.collider.GetComponent<EnemyScript>().Damage();
					}
				}
			}
			#endif
		}
	}
	public void ToZukan(){
		zukan = true;
	}
	public void ToGame(){
		zukan = false;
	}

	void CreateBall(){
		ball = Instantiate (ballSozai, new Vector3 (0,-4,-17), Quaternion.identity) as GameObject;
		ball.GetComponent<Rigidbody> ().useGravity = false;
	}

	public void SetLocation(double h,double v){
		nowPos = new Positions (h, v);
		if (nowPos != null && prePos != null) {
			sumDistance += CalculateDistance (nowPos, prePos);
		}
		prePos = nowPos;
	}

	double deg2rad(double deg)
	{
		return (deg / 180) * Mathf.PI;
	}



	/// <summary>
	/// 2点間の位置情報から距離を求める
	/// </summary>
	/// <param name="posA"></param>
	/// <param name="posB"></param>
	/// <returns></returns>
	public float CalculateDistance(Positions posA, Positions posB)
	{
		// 2点の緯度の平均
		double latAvg = deg2rad(posA.Latitude + ((posB.Latitude - posA.Latitude) / 2));
		// 2点の緯度差
		double latDifference = deg2rad(posA.Latitude - posB.Latitude);
		// 2点の経度差
		double lonDifference = deg2rad(posA.Longitude - posB.Longitude);

		double curRadiusTemp = 1 - 0.00669438 * Mathf.Pow(Mathf.Sin((float)latAvg), 2);
		// 子午線曲率半径
		double meridianCurvatureRadius = 6335439.327 / Mathf.Sqrt(Mathf.Pow((float)curRadiusTemp, 3));
		// 卯酉線曲率半径
		double primeVerticalCircleCurvatureRadius = 6378137 / Mathf.Sqrt((float)curRadiusTemp);

		// 2点間の距離
		double distance = Mathf.Pow((float)(meridianCurvatureRadius * latDifference), 2) 
			+ Mathf.Pow((float)(primeVerticalCircleCurvatureRadius
				* Mathf.Cos((float)latAvg) * lonDifference), 2);
		distance = Mathf.Sqrt((float)distance);

		return (float)Mathf.Round((float)distance);
	}

	IEnumerator GetLocation() {
		if (!Input.location.isEnabledByUser) {
			Debug.Log("Error");
			yield break;
		}
		Input.location.Start();
		int maxWait =  120;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1) {
			Debug.Log( "Timed out");
			yield break;
		}
		if (Input.location.status == LocationServiceStatus.Failed) {
			Debug.Log("Unable to determine device location");
			yield break;
		} else {
			SetLocation (Input.location.lastData.latitude,Input.location.lastData.longitude);
			string s = "Location: " +
				Input.location.lastData.latitude + " " +
				Input.location.lastData.longitude + " " +
				Input.location.lastData.altitude + " " +
				Input.location.lastData.horizontalAccuracy + " " +
				Input.location.lastData.timestamp;
			Debug.Log (s);

		}
		Input.location.Stop();
	}

	void ReadCSVData(){
		StreamReader sr = new StreamReader (Application.dataPath+"/Resources/csv/data.csv");

		string strStream = sr.ReadToEnd ();

		System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;

		string[] lines = strStream.Split (new char[]{ '\r', '\n' }, option);

		char[] spliter = new char[1]{','};

		int heightLength = lines.Length;
		int widthLength = lines [0].Split (spliter, option).Length;
		mentorData = new Mentor[heightLength];
		for (int i = 0; i < heightLength; i++) {
			string[] readStrData = lines [i].Split (spliter, option);
			int id = int.Parse (readStrData [0]);
			string name = readStrData [1];
			string zokusei = readStrData [2];
			string setumei = readStrData [3];
			//Debug.Log (id);
			Mentor mentor = new Mentor (id, name,zokusei,setumei);
			mentorData [i] = mentor;
			//Debug.Log (i);
		}
	}

	public void AddMentor(int index){
		//GameManager.instance.score++;
		//mentorList.Add (mentorData [index]);
		//GameManager.instance.score++;mentorData[index]
		//Mentor mentor = mentorData[index];
		if (scController == null) {
			scController = GameObject.Find("Canvas").transform.FindChild("Panel").FindChild("ScrollView").FindChild("Content").GetComponent<ScrollController2>();
		}
		scController.SetNode(index); //図鑑にメンターを追加する

		//ScrollController.instance.SetNode (mentorData [index]);
	}
}

// Positionsクラス
public class Positions{
	public double Latitude;
	public double Longitude;
	public Positions(double input1,double input2){
		Latitude = input1;
		Longitude = input2;
	}
}
//メンタークラス
public class Mentor{
	int id;
	string name;
	string zokusei;
	string setumei;

	public Mentor(int id,string name,string zokusei,string setumei){
		this.id = id;
		this.name = name;
		this.zokusei = zokusei;
		this.setumei = setumei;
	}

	public string GetName(){
		return this.name;
	}
	public int GetId(){
		return this.id;
	}
	public string GetZokusei(){
		return this.zokusei;
	}
	public string GetSetumei(){
		return this.setumei;
	}
}
 



