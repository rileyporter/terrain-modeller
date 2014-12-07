using UnityEngine;
using System.Collections;
using Leap;

public class GrabControllerOVR : MonoBehaviour {
	
	
	public HandController handController;
	public OVRCameraRig cameraRig;	
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnGUI(){
		GUI.Label (new Rect (300f, 300f, 100f, 100f), (isGrabbing) ? "grabbing" : "not grabbing");
	}
	
	
	private bool isGrabbing = false;
	
	int switchCounter = 0;
	
	Vector3 startingPosition;
	Vector3 tempGameObjectPosition;
	Vector3 previousDelta;
	
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
		
		if (handController.GetAllGraphicsHands ().Length == 0)
			return;
		
		HandModel hand = handController.GetAllGraphicsHands()[0];
		
		if (hand != null) {
			
			HandModel h = hand;
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
					startingPosition = p;	
					tempGameObjectPosition = gameObject.transform.localPosition;	

					// calculate Vcl
					//Vcl = handController.transform.position - cameraRig.transform.position;
					//Vcl.Normalize();

					previousDelta = new Vector3(0, 0, 0);
					
					
				}

				startingPosition += previousDelta;
				Vector3 unityDelta = p - startingPosition; // startingPosition - p

				Debug.Log(unityDelta);

				previousDelta = unityDelta;
				
				// scale unity delta
				//unityDelta = unityDelta * -0.1f;
				unityDelta.x = 0;
				unityDelta.y = 0;

				Vector3 newGameObjectPosition =  ((unityDelta.z) * new Vector3(0f, 0f, 1f)) + tempGameObjectPosition;
				
				gameObject.transform.localPosition = newGameObjectPosition;
				
				
			}
			
		}
		
	}
}
