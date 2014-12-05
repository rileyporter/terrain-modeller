using UnityEngine;
using System.Collections;
using Leap;

public class GrabController : MonoBehaviour {


	public HandController handController;
	public OVRCameraRig cameraRig;	

	// Use this for initialization
	void Start () {
	
	}


	private bool isGrabbing = false;

	int switchCounter = 0;

	Vector3 tempPosition;
	Vector3 tempGameObjectPosition;

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
					tempPosition = p;
	
					tempGameObjectPosition = gameObject.transform.localPosition;

				}
				
				Vector3 delta = tempPosition - p;
				Debug.Log(delta);
				//delta = delta * 1.2f;

				Vector3 newGameObjectPosition =  delta + tempGameObjectPosition;
				
				gameObject.transform.localPosition = newGameObjectPosition;
				
				
			}
		
		}

	}
}
