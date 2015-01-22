using UnityEngine;
using System.Collections;


[System.Serializable]
public class Chunk : MonoBehaviour
{
	//Fields
	public int id = 0;

	public int positionX = 0;
	public int positionZ = 0;

	//Save data and info
	//Density field for the marching cubes algorythm
	[System.NonSerialized]
	public float[,,] density;
	[System.NonSerialized]
	public bool generateCaves = false;
	
	public Mesh mesh;
	
	private int heightOffset;
	public float[,] heightMap;

	public float scale = 1.0f;
	
	private Vector2[] texturePositions;
	public Vector2 caveTexturePosition;
	
	[System.NonSerialized]
	public Vector3 chunkSize;

	//Constructor
	public void Assign (int inputID, int posX, int posZ)
	{
		id = inputID;
		positionX = posX;
		positionZ = posZ;
	}

	public void Initialize (cFractalNoise[] fractalNoise, cFractalNoise caveFractal, Vector3 chunkSizeInput, Vector2 xy)
	{
		chunkSize = chunkSizeInput;
		
		heightOffset = Mathf.RoundToInt(chunkSize.y * 0.3f);
		
		texturePositions = new Vector2[fractalNoise.Length];
		for (int i = 0; i < fractalNoise.Length; i ++) {
			texturePositions[i] = new Vector2( xy.y * fractalNoise[i].scale * (chunkSize.z-1), xy.x * fractalNoise[i].scale * (chunkSize.x-1));
		}
		
		caveTexturePosition = new Vector2( xy.y * caveFractal.scale * (chunkSize.z-1), xy.x * caveFractal.scale * (chunkSize.x-1));
		
		CalculateFractal (fractalNoise);
		
		AppendHeightMap(caveFractal);
		
		if (generateCaves)
			Calculate3D(caveFractal, caveTexturePosition);
	}

	public void CalculateFractal (cFractalNoise[] fractalNoise)
	{
		int x = Mathf.RoundToInt (chunkSize.x);
		int y = Mathf.RoundToInt (chunkSize.y);
		int z = Mathf.RoundToInt (chunkSize.z);
		
		density = new float[x, y, z];
		heightMap = new float[x, z];
		int iterater = 0;
		
		for (int i = 0; i < fractalNoise.Length; i ++) {
			
			if (fractalNoise[i].enabled == false)
				continue;
			
			if ( fractalNoise[i].threeDimensional ) {
				Calculate3D(fractalNoise[i], texturePositions[i]);
				return;
			}
			
			fractalNoise[i].width = x;
			fractalNoise[i].height = y;
			fractalNoise[i].depth = z;
	
			heightMap = fractalNoise[i].Calculate (texturePositions[i], scale, heightMap, iterater);
			
			if (fractalNoise[i].blendType == BlendType.Accumulative || iterater == 0)
				iterater ++;
		}
		
		/*for (int vX = 0; vX < chunkSize.x; vX++) {
			for (int vZ = 0; vZ < chunkSize.z; vZ++) {
			 	heightMap[vX,vZ] /= iterater;
			}
		}*/
	}

	public void AppendHeightMap (cFractalNoise caveFractal)
	{
		for (int x = 0; x < chunkSize.x; x++) {
			for (int z = 0; z < chunkSize.z; z++) {
				//Create 2d heighmap along the x and z axis
				int y2 = Mathf.Abs(Mathf.RoundToInt (heightMap[x, z] * (chunkSize.y * 0.7f))) + heightOffset;
				density[x, y2, z] = 1;

				for (int y = 0; y < chunkSize.y; y++) {
					if (y == y2)
						continue; 
					else if (y < y2){
							density[x, y, z] = 1;
					}
					else if (y > y2)
						density[x, y, z] = 0;
				}
			}
		}
	}
	
	public void Calculate3D( cFractalNoise fractalNoise, Vector2 texturePosition )
	{
		int i = 0;
		for (int x = 0; x < chunkSize.x; x++) {
			for (int y = 0; y < chunkSize.y; y++) {
				for (int z = 0; z < chunkSize.z; z++) {
					if (density[x,y,z] == 0 )
						continue;
					
					float value = fractalNoise.Calculate(texturePosition, scale, x, y, z);
					if (value < 0.5f)
						density[x, y, z] = 0;
					
					i++;
					//if (i == 100)
					//	yield return 1;
					//Debug.Log(density[x,y,z]);
				}
			}
		}
	}
	
	public void Save ()
	{
		
	}

	public void Load ()
	{
		
	}
}

