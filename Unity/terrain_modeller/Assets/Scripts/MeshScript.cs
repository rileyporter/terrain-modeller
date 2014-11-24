using UnityEngine;
using System.Collections;


using Leap;




public class MeshScript : MonoBehaviour {

	static bool DEBUG_MODE = false;

	public  HandController hc;

	private float padding = 1f;
	public int width = 10;
	public int height = 10;

	public Material activeHandle;
	public Material inactiveHandle;

	public Vector3[] vertices;
	public GameObject[] handles;

	private bool isGrabbing = false;

	Controller controller;

	GameObject testSpehere;

	protected const float PALM_CENTER_OFFSET = 0.0150f;
	
	// Use this for initialization
	void Start () {
		controller = new Controller();

		if(DEBUG_MODE)
			testSpehere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
	}


	Vector leapToWorld(Leap.Vector leapPoint, InteractionBox iBox)
	{
		leapPoint.z *= -1.0f; //right-hand to left-hand rule
		Leap.Vector normalized = iBox.NormalizePoint(leapPoint, false);
		normalized += new Leap.Vector(0.5f, 0f, 0.5f); //recenter origin
		return normalized * 100.0f; //scale
	}


	int calculateIndex(int x, int y){
		return y * (width + 1) + x;
	}

	int getClosestVector(Vector3 v){


		// dummy implementation
		float smallestDistance = float.MaxValue;
		int smallestIndex = 0;

		for(int i = 0; i < handles.Length; i++){
			GameObject h = handles[i];



			// calculate distance
			Vector3 dist = v - h.transform.localPosition;
			float d = dist.magnitude;

			if(d<smallestDistance){
				smallestDistance = d;
				smallestIndex = i;
			}
		}

		return smallestIndex;

		/*

		float offset_x = (-width/2.0f) * padding;
		float offset_z = (-height/2.0f) * padding;

		bool validInput = true;

		float x_transformed = v.x*-1f;
		float z_transformed = v.z*-1f;

		Debug.Log ("V: "+x_transformed + ", " + z_transformed);


		int x_index = 0;
		int y_index = 0;

		float full_width = width * padding;
		float full_height = height * padding;



		x_index = Mathf.FloorToInt (full_width / x_transformed);
		y_index = Mathf.FloorToInt (full_height / z_transformed);

		Debug.Log (y_index + ", " + full_height);

		x_index -= width/2;
		y_index -= height/2; 

		if (x_index < 0) {
			x_index = 0;
		}
		if (x_index >= width) {
			x_index = width-1;		
		}

		if (y_index < 0) {				
			y_index = 0;
		}
		if (y_index >= height) {
			y_index = height-1;		
		}

		return calculateIndex (x_index, y_index);

		*/


	}


	private float handtempY = -1;
	private float vertexY = -1;

	private bool wasEmpty = true;

	private int currentVertex = -1;

	int switchCounter = 0;
	// Update is called once per frame
	void Update () {

		Frame frame = controller.Frame();


		if (!frame.Hands.IsEmpty) {
						// get a hand

						if (wasEmpty) {
								wasEmpty = false;
								for (int i = 0; i < handles.Length; i++) {
										handles [i].renderer.enabled = true;
								}
						}


						HandModel[] hands = hc.GetAllGraphicsHands ();

						HandModel h = hands [0];

						Vector3 p = h.GetPalmPosition ();


						Hand leapHand = h.GetLeapHand ();
						

						bool justSwitched = false;

						bool beforeCheck = isGrabbing;

						isGrabbing = leapHand.GrabStrength > 0.9;

						justSwitched = beforeCheck != isGrabbing;

						if(justSwitched){
							Debug.Log ("switched " + switchCounter++);
						}
				



						int closestVectorIndex = getClosestVector (p);
						

						if (isGrabbing) {



								if (justSwitched) {
										handtempY = p.y;
										currentVertex = closestVectorIndex;
										vertexY = vertices [closestVectorIndex].y;
								}

								float delta = p.y - handtempY;

								float newVertexPositionY = vertexY + delta;

								vertices [currentVertex].y = newVertexPositionY;

								Vector3 oldSpherePosition = handles [currentVertex].transform.localPosition;
								oldSpherePosition.y = newVertexPositionY;
								handles [currentVertex].transform.localPosition = oldSpherePosition;



						} else {

								// iterate over all handles
								for (int i = 0; i < handles.Length; i++) {
										GameObject handle = handles [i];
										Vector3 currentLocation = handle.transform.localPosition;

										if (i == closestVectorIndex) {
												handle.renderer.material = activeHandle;
										} else {
												handle.renderer.material = inactiveHandle;
										}
				
										handle.transform.localPosition = currentLocation;
								}

						}

						//Debug.Log (getClosestVector(p));
						if(DEBUG_MODE)
							testSpehere.transform.localPosition = p;


						//Debug.Log (p);

				} else {
				
			if(!wasEmpty){
				wasEmpty = true;
				for(int i = 0; i < handles.Length; i++){
					handles[i].renderer.enabled=false;
				}
			}

		}

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		mesh.vertices = vertices;

	}

	


	Mesh CreateMesh(int width, int height)
	{


		float offset_x = (-width/2.0f) * padding;
		float offset_z = (-height/2.0f) * padding;

		// calculate the number of vertices
		int num_vertices = (width + 1) * (height + 1);

		Mesh m = new Mesh();
		m.name = "ScriptedMesh";

		// setup the vertices
		m.vertices = new Vector3[num_vertices];
		vertices = new Vector3[num_vertices];
		handles = new GameObject[num_vertices];

		// and uv coordinates
		m.uv = new Vector2[num_vertices];

		for (int x = 0; x <= width; x++) {
			for(int y = 0; y <= height; y++){
				vertices[y*(width+1) + x] = new Vector3(offset_x + x*padding ,0.0f ,offset_z + y*padding);
				m.uv[y*(width+1) + x] = new Vector2(x/width,y/height);

				// create a visual handle for each vertex
				GameObject s = GameObject.CreatePrimitive( PrimitiveType.Sphere);
				s.renderer.material = inactiveHandle;
				s.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
				s.transform.localPosition = vertices[y*(width+1) + x];

				s.renderer.enabled = false;

				handles[y*(width+1) + x] = s;

			}
		}

		m.vertices = vertices;

	


		// number of triangles
		int num_triangles = 3* 2 * width * height;
		int index_counter = 0;
		m.triangles = new int[num_triangles];

		int[] triangles = new int[num_triangles];


		// setup the triangles
		for (int x = 0; x < width; x++){
			for(int y =0; y < height; y++){

				triangles[index_counter] = (y+0)*(width+1) + (x+0);
				index_counter++;
				triangles[index_counter] = (y+1)*(width+1) + (x+0);
				index_counter++;
				triangles[index_counter] = (y+1)*(width+1) + (x+1);
				index_counter++;
				
				triangles[index_counter] = (y+0)*(width+1) + (x+0);
				index_counter++;
				triangles[index_counter] = (y+1)*(width+1) + (x+1);
				index_counter++;
				triangles[index_counter] = (y+0)*(width+1) + (x+1);
				index_counter++;
				

			
			}
		}

		m.triangles = triangles;

	

		m.RecalculateNormals();
		
		return m;
	}


	void Awake() {
		GameObject plane = gameObject ;
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent(typeof(MeshFilter));
		meshFilter.mesh = CreateMesh(width,height);
		MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material.shader = Shader.Find ("Specular");
		Texture2D tex = new Texture2D(1, 1);
		tex.SetPixel(0, 0, Color.green);
		tex.Apply();
		renderer.material.mainTexture = tex;
		renderer.material.color = Color.green;
	}


}
