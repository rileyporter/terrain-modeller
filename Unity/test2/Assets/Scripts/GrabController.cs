using UnityEngine;
using System.Collections;
using Leap;

public class GrabController : MonoBehaviour {


	public HandController handController;

	public GameObject cameraRig;
	public Camera camera;

	private ManipulatorControl manipulator;

	public GameObject debugIndicator1;
	public GameObject debugIndicator2;


	// Use this for initialization
	void Start () {
		manipulator = (ManipulatorControl) GetComponent("ManipulatorControl");
	}

	void OnGUI(){
		GUI.Label (new Rect (300f, 300f, 100f, 100f), (isGrabbing) ? "grabbing" : "not grabbing");
	}


	private bool isGrabbing = false;

	int switchCounter = 0;

	Vector startingPositionXZ;
	Vector startingPositionYZ;
	Vector3 tempGameObjectPosition;
	Vector3 tempGameObjectRotation;

	Vector Lb;
	Vector LsXZ;
	Vector LsYZ;
	Vector LbsXZ;
	Vector LbsYZ;

	Vector3 Vcl; // camera to leap in world coordinates

	float dist_z = 0;

	/*

	private Vector3 getZXPlane (Vector3 v){
		Vector3 vn = new Vector3 (v.x, 0, v.z);
	}
	
	private Vector getZXPlane (Vector v){
		Vector vn = new Vector (v.x, 0, v.z);
	}
	
	private Vector3 getZYPlane (Vector3 v){
		Vector3 vn = new Vector3 (0, v.y, v.z);
	}
	
	private Vector getZYPlane (Vector v){
		Vector vn = new Vector (0, v.y, v.z);
	}
	*/


	
	// Update is called once per frame
	void Update () {

		if (manipulator.pointSelected ()) {
			return;
		}


		if (handController.GetAllGraphicsHands ().Length == 0)
						return;



		HandModel hand = manipulator.getDominantHand ();

		if (hand != null) {

			HandModel h = hand;

			Vector leapPalmPosition =  hand.GetLeapHand().PalmPosition;

			Vector leapPalmPositionXZPlane =  hand.GetLeapHand().PalmPosition;
			leapPalmPositionXZPlane.y = 0;

			Vector leapPalmPositionYZPlane = hand.GetLeapHand().PalmPosition;
			leapPalmPositionYZPlane.x = 0;

		
			Vector3 p = h.GetPalmPosition ();

			
			Hand leapHand = h.GetLeapHand ();
			
			
			bool justSwitched = false;
			
			bool beforeCheck = isGrabbing;
			
			isGrabbing = leapHand.GrabStrength > 0.9;
			
			justSwitched = beforeCheck != isGrabbing;
			
			if(justSwitched){
				Debug.Log ("switched " + switchCounter++);
			}
			

			
			
			if (isGrabbing) {
				

				if (justSwitched) {
					startingPositionXZ = leapPalmPositionXZPlane;	
					startingPositionYZ = leapPalmPositionYZPlane;	

					tempGameObjectPosition = gameObject.transform.localPosition;	
					tempGameObjectRotation = gameObject.transform.localEulerAngles;

					// get distance from Ls to Lb
					dist_z = -800;


					// calculate Lb
					Lb = -(new Vector(0, 0, dist_z));




					LbsXZ = leapPalmPositionXZPlane - Lb;
					LbsYZ = leapPalmPositionYZPlane - Lb;

				}




				Vector LbiXZ = leapPalmPositionXZPlane - Lb;
				Vector LbiYZ = leapPalmPositionYZPlane - Lb;

				float dotPXZ = LbiXZ.Normalized.Dot (LbsXZ.Normalized);
				float dotPYZ = LbiYZ.Normalized.Dot (LbsYZ.Normalized);

				//Debug.Log (dotP );
				float angleXZ = Mathf.Acos(dotPXZ) *  57.29f;
				float angleYZ = Mathf.Acos(dotPYZ) *  57.29f;

				//get the rotation of the angle
				if(leapPalmPositionXZPlane.x > startingPositionXZ.x){
					// turn to right
					angleXZ*=-1f;
				}else{

				}

				//get the rotation of the angle
				if(leapPalmPositionYZPlane.y > startingPositionYZ.y){
					// turn to right
					angleYZ*=-1f;
				}else{
					
				}
				//angleYZ = 0;

				Vector3 newRotation = tempGameObjectRotation + new Vector3(angleYZ*-2f,angleXZ*6.0f, 0);
				newRotation.x = (newRotation.x+360);
				newRotation.y = (newRotation.y+360);
				newRotation.z = (newRotation.z+360);
				gameObject.transform.localEulerAngles = newRotation;


				Debug.Log(angleXZ + ", " + angleYZ);

				Vector leapDelta = startingPositionXZ - leapPalmPosition;
					
				// Convert leap Delta to unity delta
				leapDelta = leapDelta;
				leapDelta.x = 0;
				leapDelta.y = 0;

				Vector3 unityDelta = new Vector3(leapDelta.x, leapDelta.y, -leapDelta.z);

				//Debug.Log(tempPosition + " " + p + " " + new_delta + " " + delta);

				Vector3 newGameObjectPosition =  (	unityDelta.z * camera.transform.forward.normalized) + tempGameObjectPosition;
				
				gameObject.transform.localPosition = newGameObjectPosition;
				
				
			}
		
		}

	}
}
