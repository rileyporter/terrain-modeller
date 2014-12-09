using UnityEngine;
using System.Collections;



public class Modifytest : MonoBehaviour {

	TerrainData td;

	private float[,] old_heights;



	// Use this for initialization
	void Start () {
		TerrainCollider tdc;
		tdc = (TerrainCollider) GetComponent ("TerrainCollider");
		td = tdc.terrainData;


		old_heights = td.GetHeights(0,0,tdc.terrainData.heightmapWidth, tdc.terrainData.heightmapHeight);


		print("test");
	}


	void modifyT(){

		float[,] heights = td.GetHeights (0, 0, 200, 200);

		for (int x = 0; x < 200; x++) {
			for(int y = 0; y < 200; y++){
				heights[x,y] = Mathf.Min(heights[x,y] * 1.1f, 1.0f);
			}		
		
		}

		//heights [0, 0] = 1;

		td.SetHeights (0, 0, heights);

		
	}

	void OnDestroy(){
		// reset the old terrani
		td.SetHeights (0, 0, old_heights);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space")) {
			print ("space key was pressed");

			var timer = System.Diagnostics.Stopwatch.StartNew();
			modifyT();
			timer.Stop();
			var elapsed = timer.Elapsed;
			print (elapsed.Milliseconds);

		}
	}
}
