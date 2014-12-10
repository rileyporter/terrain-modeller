using UnityEngine;
using System.Collections;
using Leap;

public class GrabControllerOVR : MonoBehaviour {
	
	
	public HandController handController;
	public OVRCameraRig cameraRig;	
	private ManipulatorControl manipulator;
	
	// Use this for initialization
	void Start () {
		manipulator = (ManipulatorControl) GetComponent("ManipulatorControl");
	}
	
	void OnGUI(){
		//GUI.Label (new Rect (300f, 300f, 100f, 100f), (isGrabbing) ? "grabbing" : "not grabbing");
	}


	
	
	private bool isGrabbing = false;
	
	int switchCounter = 0;
	
	Vector3 startingPosition;
	Vector3 tempGameObjectPosition;
	Vector3 previousDelta;

	Vector leapStartingPosition;

	
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
			Vector3 palmposition = h.GetPalmPosition ();
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
					startingPosition = palmposition;	
					tempGameObjectPosition = gameObject.transform.localPosition;	

					leapStartingPosition = leapHand.PalmPosition;


					// calculate Vcl
					//Vcl = handController.transform.position - cameraRig.transform.position;
					//Vcl.Normalize();

					previousDelta = new Vector3(0, 0, 0);
					
					
				}


				//Vector3 unityDelta = startingPosition - palmposition; // startingPosition - p
				//startingPosition = palmposition;

				Vector leapDelta = leapStartingPosition - leapHand.PalmPosition;

				Vector3 unityDelta = new Vector3(leapDelta.z,leapDelta.x,leapDelta.y*-1.0f);
				unityDelta = unityDelta * 0.1f;

				float scaledYDistance = leapDelta.y;
				float scaledXDistance = leapDelta.x * -1.0f;
				float scaledZDistance = leapDelta.z * -1.0f;

				Vector3 camForward = cameraRig.leftEyeAnchor.camera.transform.forward;
				camForward.Normalize();

				Vector3 camRight = cameraRig.leftEyeAnchor.camera.transform.right;
				camRight.Normalize();

				Vector3 camUp = cameraRig.leftEyeAnchor.camera.transform.up;
				camUp.Normalize();




				//Vector3 newGameObjectPosition =  (scaledYDistance *camForward) + tempGameObjectPosition;
				//Vector3 newGameObjectPosition =  (scaledXDistance *camRight) + tempGameObjectPosition;
				Vector3 newGameObjectPosition =  (scaledZDistance *camUp) +(scaledXDistance *camRight) + (scaledYDistance *camForward) + tempGameObjectPosition;
				
				gameObject.transform.localPosition = newGameObjectPosition;
				
				
			}
			
		}
		
	}
}
