using UnityEngine;
using System.Collections;
using Leap;

public class ManipulatorControl : MonoBehaviour {

	public HandController handController;
	public GameObject positionIndicator;	
	public Terrain terrain;
	private TerrainCollider tc;
	private TerrainData td;

	public Material nonSelectedMaterial;
	public Material selectedMaterial;

	public bool rightIsDominant = true;

	public Vector3 indiciatorOffset = new Vector3(0.0f, 0.0f, 0.0f);

	// Use this for initialization
	public void Start () {
		Controller c = handController.GetLeapController ();
		//c.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
		c.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		//c.EnableGesture(Gesture.GestureType.TYPESWIPE);
		c.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		//c.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		c.Config.SetFloat ("Gesture.KeyTap.MinDistance", 10.0f);

		
		
		

		tc = (TerrainCollider) terrain.GetComponent("TerrainCollider");
		td = tc.terrainData;

		//print ("parent");
	}

	private bool selected = false;


	private int lastTapGesture_id = -1;

	
	// Update is called once per frame
	public void Update () {
		// see if we have a hand


		HandModel dominantHand;
		HandModel nonDominantHand = null;

		if (handController.GetAllGraphicsHands ().Length == 0)
			return;


		if (handController.GetAllGraphicsHands ().Length == 1) {
		 	dominantHand = handController.GetAllGraphicsHands () [0];
		} else {
			// decide which hand is in
			HandModel firstHand = handController.GetAllGraphicsHands()[0];
			HandModel secondHand = handController.GetAllGraphicsHands()[1];

			HandModel leftHand;
			HandModel rightHand;

			if(firstHand.GetLeapHand().IsRight){
				rightHand = firstHand;
				leftHand = secondHand;
			} else {
				rightHand = secondHand;
				leftHand = firstHand;
			}

			if(rightIsDominant){
				dominantHand = rightHand;
				nonDominantHand = leftHand;
			}else{
				dominantHand = leftHand;
				nonDominantHand = rightHand;
			}
			
		}

		Frame frame = handController.GetFrame ();
		
	
		
		// pointing		
		HandModel pointingHand = dominantHand;

		if (!selected) {
			// this is the absolute position in unity coordinates
			Vector3 castOrigin = pointingHand.fingers[1].GetBoneCenter(3);
			Vector3 castDirection = pointingHand.fingers[1].GetBoneDirection(3);
			
			// we only want the latyer with the terrain
			int mask = 1 << 8;
			
			RaycastHit hit;
			if(Physics.Raycast(castOrigin, castDirection,  out hit, Mathf.Infinity,  mask)){
				//print ("hit");
				positionIndicator.transform.position = hit.point;
				//print(hit.collider.gameObject.name);
			}



		}
		


		// selecting


		foreach (Gesture g in handController.GetFrame ().Gestures()) {

			if(g.Type == Gesture.GestureType.TYPEKEYTAP){

				if(lastTapGesture_id != -1 && lastTapGesture_id != g.Id){

					if(g.Hands[0].Id == nonDominantHand.GetLeapHand().Id){
						print("keytap! " + lastTapGesture_id);
						selected = !selected;
					}


				}


				lastTapGesture_id = g.Id;
			}


		}





		
	}
}
