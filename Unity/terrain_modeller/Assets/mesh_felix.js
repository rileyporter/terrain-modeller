#pragma strict

var newVertices : Vector3[];
var newNormals : Vector3[];

var newUV : Vector2[];
var newTriangles : int[];

var mesh : Mesh;

function Start () {

	newVertices = new Vector3[3];
	newTriangles= new int[3];
	
	newVertices[0] = new Vector3(1,1,1);
	newVertices[1] = new Vector3(1,10,1);
	newVertices[2] = new Vector3(1,1,10);
	
	newUV = new Vector2[2];
	
	newUV[0] = new Vector2(1,1);
	newUV[1] = new Vector2(1,1);
	
	
	newTriangles[0] = 0;
	newTriangles[1] = 1;
	newTriangles[2] = 2;
	
	var a = newVertices[1] - newVertices[0];
	var b = newVertices[2] - newVertices[0];
	
	newNormals[0] = -Vector3.forward;
	newNormals[1] = -Vector3.forward;
	newNormals[2] = -Vector3.forward;
	
	

	mesh  = new Mesh();
	
	
	
	GetComponent(MeshFilter).mesh = mesh;
	mesh.vertices = newVertices;
	mesh.uv = newUV;
	mesh.normals = newNormals;
	mesh.triangles = newTriangles;
}

function Update () {

}