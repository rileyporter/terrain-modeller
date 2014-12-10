using UnityEngine;
using System.Collections;
using System; 
using Leap;


public class Gaussian : MonoBehaviour {

	public HandController handController;
	public TerrainData td;
	private float[,] old_heights;
	private bool manipulating;

	void Start () {
		Controller leapController = handController.GetLeapController ();
		leapController.EnableGesture(Gesture.GestureType.TYPESCREENTAP);

		manipulating = false; // able to travel

		TerrainCollider tdc;
		tdc = (TerrainCollider) GetComponent ("TerrainCollider");
		td = tdc.terrainData;
		old_heights = td.GetHeights(0,0,tdc.terrainData.heightmapWidth, tdc.terrainData.heightmapHeight);
	}


	// must take an odd number as length
	public static float[,] generateGaussian(int length, double weight) {
		float[,] Kernel = new float [length, length];  
		float sumTotal = 0;  
		int kernelRadius = length / 2;  
		float distance = 0;  
		float calculatedEuler = (float)(1.0 / (2.0 * Math.PI * Math.Pow(weight, 2)));  
		
		for (int filterY = -kernelRadius; filterY <= kernelRadius; filterY++) { 
			for (int filterX = -kernelRadius; filterX <= kernelRadius; filterX++) { 
				distance = (float) (((filterX * filterX) +   
				                     (filterY * filterY)) /   
				                    (2 * (weight * weight)));  
				
				Kernel[filterY + kernelRadius,   
				       filterX + kernelRadius] = (float)  (
					calculatedEuler * Math.Exp(-distance));  
				
				sumTotal += Kernel[filterY + kernelRadius,   
				                   filterX + kernelRadius];  
			}  
		}  
		
		for (int y = 0; y < length; y++) {  
			for (int x = 0; x < length; x++) {  
				Kernel[y, x] = (float) (Kernel[y, x] * (1.0 / sumTotal));  
			}  
		}  
		return Kernel;  
	}

	public static void printArr(float[,] arr) {
		int rowLength = arr.GetLength (0);
		int colLength = arr.GetLength (1);
		
		for (int i = 0; i < rowLength; i++) {
			string foo = "";
			for (int j = 0; j < colLength; j++) {
				foo += string.Format ("{0} ", arr[i, j]) + "\t";
			
			}
			print(foo);
		}
	}
	
	
	public static void applyFilter (float[,] data, float[,] filter, int startRow, int startCol) {
		float scale = 5.0f;
		int filterRowLength = filter.GetLength (0);
		int filterColLength = filter.GetLength (1);
		int dataRowLength = data.GetLength (0);
		int dataColLength = data.GetLength (1);

		if (startRow + filterRowLength > dataRowLength || startCol + filterColLength > dataColLength) {
			Debug.Log("Error, filter goes off bounds of data array");
			return;
		}
		
		for (int i = 0; i < filterRowLength; i++) {
			for (int j = 0; j < filterColLength; j++) {
				data[i + startRow, j + startCol] += data[i + startRow, j + startCol] * (filter[i, j] * scale);
			}
		}
	}


	void Update () {

		Frame frame = handController.GetFrame ();

		foreach (Gesture gesture in frame.Gestures()) {
			switch(gesture.Type) {
				case(Gesture.GestureType.TYPECIRCLE): {
					// not enabled yet, should be used for digging?
					Debug.Log("Circle gesture recognized.  SHOULD NOT HAPPEN");
					break;
				} case(Gesture.GestureType.TYPEKEYTAP): {
					// not in use
					Debug.Log("Key Tap gesture recognized.  SHOULD NOT HAPPEN");
					break;
				} case(Gesture.GestureType.TYPESCREENTAP): {
					Debug.Log("Screen tap gesture recognized. YAY");
					// get the x and z coordinates of the red ball
					// change it's colour to blue/etc to show 'selected'
				manipulating = true;
					break;
				} case(Gesture.GestureType.TYPESWIPE): {
					// not in use
					Debug.Log("Swipe gesture recognized.  SHOULD NOT HAPPEN");
					break;
				} default: {
					break;
				}
			}
		}
	

		if (Input.GetKeyDown ("space")) {
			print ("space key was pressed");
			float[,] heights = td.GetHeights (0, 0, 200, 200);

			int width = 151;
			double weight = 0.25f * width;

			float[,] filter = generateGaussian(width, 4.0);
			printArr(filter);
			applyFilter(heights, filter, 25, 25);
			td.SetHeights (0, 0, heights);

		}
	}

	void OnDestroy(){
		// reset the old terrani
		td.SetHeights (0, 0, old_heights);
	}

	
}