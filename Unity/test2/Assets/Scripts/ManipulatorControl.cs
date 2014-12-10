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

	private Vector3 posInTerrainCoordinates;
	private float[,] old_heights;

	// Use this for initialization
	public void Start () {
		Controller c = handController.GetLeapController ();
		//c.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
		//c.EnableGesture (Gesture.GestureType.TYPECIRCLE);
		//c.EnableGesture(Gesture.GestureType.TYPESWIPE);
		c.EnableGesture (Gesture.GestureType.TYPEKEYTAP);
		//c.Config.SetFloat ("Gesture.KeyTap.HistorySeconds", 0.2f);
		c.Config.SetFloat ("Gesture.KeyTap.MinDistance", 10.0f);

		
		
		

		tc = (TerrainCollider) terrain.GetComponent("TerrainCollider");
		td = tc.terrainData;

		old_heights = td.GetHeights(0,0,td.heightmapWidth, td.heightmapHeight);

		//print ("parent");
	}

	private bool selected = false;

	public bool pointSelected(){
		return selected;
	}


	private int lastTapGesture_id = -1;
	private int lastCircleGesture_id = -1;



	HandModel dominantHand;
	HandModel nonDominantHand = null;

	public HandModel getDominantHand(){
		return dominantHand;
	}

	public HandModel getNonDominantHand(){
		return nonDominantHand;
	}

	private bool isGrabbing = false;


	Vector previousHandPosition;

	public int kernelDimension = 101;

	int modification_pos_x;
	int modification_pos_y;

	float[,] height_buffer;
	// Update is called once per frame
	public void Update () {
		// see if we have a hand


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
						Vector3 castOrigin = pointingHand.fingers [1].GetBoneCenter (3);
						Vector3 castDirection = pointingHand.fingers [1].GetBoneDirection (3);
			
						// we only want the latyer with the terrain
						int mask = 1 << 8;
			
						RaycastHit hit;
						if (Physics.Raycast (castOrigin, castDirection, out hit, Mathf.Infinity, mask)) {
								//print ("hit");
								positionIndicator.transform.position = hit.point;
								//print(hit.collider.gameObject.name);
						}


			positionIndicator.renderer.material = nonSelectedMaterial;
				} else {
			positionIndicator.renderer.material = selectedMaterial;		
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






		// manipulating the scene
		if (selected) {

				HandModel hand = getDominantHand ();
				
				HandModel h = hand;
				
				Hand leapHand = hand.GetLeapHand();	

				Vector3 p = h.GetPalmPosition ();
				
				
				
				bool justSwitched = false;
				
				bool beforeCheck = isGrabbing;
				
				isGrabbing = leapHand.GrabStrength > 0.9;
				
				justSwitched = beforeCheck != isGrabbing;
				
				
				
				
				if (isGrabbing) {
					
					
					if (justSwitched) {
						previousHandPosition = leapHand.PalmPosition;

						// load the terrain at the specific position
						Vector3 posInTerrainCoordinates = positionIndicator.transform.position - terrain.transform.position;
						
						posInTerrainCoordinates = posInTerrainCoordinates * (1.0f/td.size.x) * (float) td.heightmapWidth; // depending on the resolution
						print (posInTerrainCoordinates);

						
						// get the height data
					modification_pos_x = (int) posInTerrainCoordinates.x - (kernelDimension / 2);
					modification_pos_y = (int) posInTerrainCoordinates.z - (kernelDimension / 2);

					height_buffer = td.GetHeights(modification_pos_x, modification_pos_y, kernelDimension, kernelDimension);





					}
					
					
					float distY = leapHand.PalmPosition.y - previousHandPosition.y;
					


					float scaledDist = distY / 1000.0f;

					print(scaledDist);

					float[,] temp_heights = new float[kernelDimension,kernelDimension];

					for (int x = 0; x < kernelDimension; x++) {
						for(int y = 0; y < kernelDimension; y++){
						temp_heights[x,y] = Mathf.Min(height_buffer[x,y] + scaledDist, 1.0f);
						}		
					
					}

					
					
					//print (distY);
					
				}
				
			}



			/*
						CircleGesture circle = null;
				
						foreach (Gesture g in handController.GetFrame ().Gestures()) {
			
								if (g.Type == Gesture.GestureType.TYPE_CIRCLE) {

										if(lastCircleGesture_id == -1){
											tempCircleValue = absoluteCircleValue;
										}
										
										bool gestureValid = false;
										bool ongoing = false;
										bool clockwise = false;
				
										if (lastCircleGesture_id != -1 && lastCircleGesture_id == g.Id) {
											ongoing = true;
										}



										if (g.Hands [0].Id == dominantHand.GetLeapHand ().Id) {
											circle = new CircleGesture(g);
											gestureValid = true; // it is the right hand
											
											if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Mathf.PI/4)
											{
												clockwise = true;
											}
											
											
											
										}
										else{
											gestureValid = false;
										}
										
										float dirScale = 1.0f;
					
										if(!clockwise)
											dirScale = -1.0f;


										

										if(gestureValid && ongoing){
											tempCircleValue = circle.Progress;
											print(tempCircleValue);			
										}


										float tempAbsoluteCircleValue = absoluteCircleValue + tempCircleValue * dirScale;
										
						
					
										if(gestureValid && !ongoing){
											// just started
											absoluteCircleValue = tempAbsoluteCircleValue;
						
										}

										
				
				
										lastCircleGesture_id = g.Id;
								}
			
			
						}
						*/



		// see if we switched the value
		//if(Mathf.Abs(circleValueBefore - absoluteCircleValue) > )






		
	}



	void OnDestroy(){
		// reset the old terrani
		td.SetHeights (0, 0, old_heights);
	}
}
