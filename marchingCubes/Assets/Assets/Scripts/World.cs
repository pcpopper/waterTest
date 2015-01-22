using UnityEngine;
using System.Collections;

//A script that builds the various chunks
[System.Serializable]
public class World : MonoBehaviour
{
	//A list of the chunks
	private Chunk[] chunk;
	//The marching cube algorythm that creates a mesh
	//Based on a 3d scalar field
	private MarchingCubes marchCube;
	
	//Should we generate caves? Alot of calculations here
	//very slow
	public bool generateCaves = false;
	
	//The size of each chunk
	public Vector3 chunkSize;
	//Number of chunks to generate
	public Vector2 chunkPositions;
	
	//The heightmap fractals that are combined to create a random heightmap
	public cFractalNoise[] heightMapFractals;
	//The 3d cave fractal generator
	public cFractalNoise caveFractal;
	
	//Holds a a simple object with a mesh renderer, mesh filter, and mesh collider
	public GameObject chunkObj;
	//Material to be assigned to the chunk obj's mesh
	public Material mat;
	
	IEnumerator Start ()
	{
		marchCube = gameObject.GetComponent<MarchingCubes>();
		chunk = new Chunk[Mathf.RoundToInt(chunkPositions.x*chunkPositions.y)];
	
		//Create the chunks
		int iterator = 0;
		for( int x = 0; x < chunkPositions.x; x++ )
		{
			for( int y = 0; y < chunkPositions.y; y++ )
			{
				//refreshHM = false;
				GameObject chunkInst = (GameObject)Instantiate(chunkObj, new Vector3((chunkSize.x-1) * x, 0, (chunkSize.z-1) * y), Quaternion.identity);
				chunkInst.transform.parent = this.transform;
				chunk[iterator] = chunkInst.AddComponent<Chunk>();
				chunk[iterator].Assign(iterator, Mathf.RoundToInt((chunkSize.x)*x), Mathf.RoundToInt((chunkSize.z)*y));
				chunk[iterator].generateCaves = generateCaves;
				chunk[iterator].Initialize(heightMapFractals, caveFractal, chunkSize, new Vector2(x, y));
				marchCube.density = chunk[iterator].density;
				marchCube.Initialize(chunkSize);
				
				
				chunkInst.GetComponent<MeshFilter>().mesh = marchCube.mesh;
				chunkInst.GetComponent<MeshCollider>().sharedMesh = marchCube.mesh;
				chunkInst.GetComponent<MeshFilter>().mesh.RecalculateBounds();
				chunkInst.GetComponent<MeshFilter>().mesh.RecalculateNormals();
				chunkInst.GetComponent<MeshRenderer>().material = mat;
				iterator++;
				yield return 1;
			}
		}
		
		//Used for mesh combining to minimize draw calls
		//gameObject.GetComponent<CombineChildren>().Combine();
		//An octree for future development
		//gameObject.GetComponent<Octree>().Initialize();
	}
	
	public void Update()
	{
		/*if (Refresh)
		{
			//refreshHM = false;
			chunk.CalculateFractal();
			chunk.Initialize();
			marchCube.density = chunk.density;
			marchCube.Initialize(chunk.chunkSize);
		}*/
	}
}