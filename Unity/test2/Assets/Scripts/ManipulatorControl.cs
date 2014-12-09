using UnityEngine;
using System.Collections;
using Leap;

public class ManipulatorControl : MonoBehaviour {

	public HandController handController;
	public GameObject positionIndicator;	
	public Terrain terrain;
	private TerrainCollider tc;
	private TerrainData td;

	// Use this for initialization
	public void Start () {


		tc = (TerrainCollider) terrain.GetComponent("TerrainCollider");
		td = tc.terrainData;

		print ("parent");
	}
	
	// Update is called once per frame
	public void Update () {
		// see if we have a hand

		if (handController.GetAllGraphicsHands ().Length == 0)
			return;
		
		HandModel hand = handController.GetAllGraphicsHands()[0];
		
		// get the palm positions

		// this is the absolute position in unity coordinates
		Vector3 palmPosition = hand.GetPalmPosition();


		// get the delta between the handcontroller and the palm
		Vector3 controllerHandDelta = handController.transform.position - palmPosition;


		// calculate the new position for the position indicator
		Vector3 newIndicatorPosition = new Vector3 ();
		newIndicatorPosition.x = handController.transform.position.x + controllerHandDelta.x * -60.0f;
		newIndicatorPosition.z = handController.transform.position.z + controllerHandDelta.z * -60.0f;
		newIndicatorPosition.y = positionIndicator.transform.localPosition.y;



		// get the position in terrain coordinates
		Vector3 posInTerrainCoordinates = newIndicatorPosition - terrain.transform.position;

		posInTerrainCoordinates = posInTerrainCoordinates * (1.0f/td.size.x) * (float) td.heightmapWidth;




		// try to get the height value
		float terrainHeightNormalized = td.GetHeight((int) posInTerrainCoordinates.x, (int) posInTerrainCoordinates.z);

		// scale it
		float terrainHeightAbsolut = terrainHeightNormalized * td.size.y;
		
		newIndicatorPosition.y = terrainHeightNormalized;
		positionIndicator.transform.position = newIndicatorPosition;


	}
}
