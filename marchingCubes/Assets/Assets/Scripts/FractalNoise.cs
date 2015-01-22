using UnityEngine;
using System.Collections;

[System.Serializable]
public enum NoiseType
{
	Brownian,
	HybridMultifractal,
	RidgedMultifractal,
	Perlin
}

[System.Serializable]
public enum BlendType
{
	Accumulative,
	Iterative
}

//A fractal noise class that can blend different fractals
[System.Serializable]
public class cFractalNoise
{
	
	public bool enabled = true;

	public NoiseType noiseType = NoiseType.HybridMultifractal;
	public BlendType blendType = BlendType.Accumulative;
	public bool threeDimensional = false;
	
	[System.NonSerialized]
	public int width = 16;
	[System.NonSerialized]
	public int depth = 16;
	[System.NonSerialized]
	public int height = 16;

	public int seed = 5;
	public float lacunarity = 9.5f;
	public float h = 0.1f;
	public float octaves = 2.0f;
	public float offset = 0.3f;
	public float scale = 0.01f;
	public float gain = 1.0f;
	
	private Perlin perlin;
	private FractalNoise fractal;

	public void Intialize ()
	{
	}

	public float Calculate (Vector2 texturePosition, float scaleInput, int x, int y, int z)
	{
		perlin = new Perlin (seed);
		
		fractal = new FractalNoise (h, lacunarity, octaves, perlin);
		
		float value = 0;
		switch (noiseType) {
		case NoiseType.Brownian:
			value = fractal.BrownianMotion (x * scale * scaleInput + texturePosition.x, y * scale * scaleInput, z * scale * scaleInput + texturePosition.y);
			break;
		case NoiseType.HybridMultifractal:
			value = fractal.HybridMultifractal (x * scale * scaleInput + texturePosition.x, y * scale * scaleInput, z * scale * scaleInput + texturePosition.y, offset);
			break;
		case NoiseType.RidgedMultifractal:
			value = fractal.RidgedMultifractal (x * scale * scaleInput + texturePosition.x, y * scale * scaleInput, z * scale * scaleInput + texturePosition.y, offset, gain);
			break;
		case NoiseType.Perlin:
			value = perlin.Noise(x * scale * scaleInput + texturePosition.x, y * scale * scaleInput, z * scale * scaleInput + texturePosition.y);
			break;
		}
		
		return value;
	}

	public float[,] Calculate (Vector2 texturePosition, float scaleInput, float[,] heightMap, int iterater)
	{
		if (enabled == false)
			return new float[height, width];
		
		perlin = new Perlin (seed);
		
		fractal = new FractalNoise (h, lacunarity, octaves, perlin);
		
		float[,] hMA = new float[width,height];
		
		for (var y = 0; y < height; y++) {
			for (var x = 0; x < width; x++) {
				float value = 0;
				
				switch (noiseType) {
				case NoiseType.Brownian:
					value = fractal.BrownianMotion (x * scale * scaleInput + texturePosition.x, y * scale * scaleInput + texturePosition.y);
					break;
				case NoiseType.HybridMultifractal:
					value = fractal.HybridMultifractal (x * scale * scaleInput + texturePosition.x, y * scale * scaleInput + texturePosition.y, offset);
					break;
				case NoiseType.RidgedMultifractal:
					value = fractal.RidgedMultifractal (x * scale * scaleInput + texturePosition.x, y * scale * scaleInput + texturePosition.y, offset, gain);
					break;
				case NoiseType.Perlin:
					value = perlin.Noise(x * scale * scaleInput + texturePosition.x, y * scale * scaleInput + texturePosition.y);
					break;
				}
				
				//Blending
				if (iterater == 0)
					hMA[x,y] = value;
				else if (blendType == BlendType.Accumulative)
					hMA[x, y] = Mathf.Lerp(heightMap[x,y], value, 0.5f);
				else if (blendType == BlendType.Iterative)
					hMA[x,y] = (value + heightMap[x,y]) / 2;
			}
		}
		
		return hMA;
	}
}

