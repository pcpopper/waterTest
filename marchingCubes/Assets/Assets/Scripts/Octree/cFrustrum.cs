using UnityEngine;
using System.Collections;

//A frustrum pulled from a book to use with the octree
[System.Serializable]
public class cFrustum
{
	// This holds the A B C and D values for each side of our frustum.
	private float[,] m_Frustum;
	public Camera cam;

	public cFrustum ()
	{
		m_Frustum = new float[6, 4];
	}

	// Call this every time the camera moves to update the frustum
	public void CalculateFrustum ()
	{
		float[] prj = new float[16];
		// This will hold our projection matrix
		float[] mdl = new float[16];
		// This will hold our model view matrix
		float[] clip = new float[16];
		// This will hold the clipping planes
		// glGetFloatv() is used to extract information about our OpenGL world.
		// stores the matrix into an array of [16].
		for (int i = 0; i < 16; i++) {
			prj[i] = cam.projectionMatrix[i];
			mdl[i] = GL.modelview[i];
		}
		
		clip[0] = mdl[0] * prj[0] + mdl[1] * prj[4] + mdl[2] * prj[8] + mdl[3] * prj[12];
		clip[1] = mdl[0] * prj[1] + mdl[1] * prj[5] + mdl[2] * prj[9] + mdl[3] * prj[13];
		clip[2] = mdl[0] * prj[2] + mdl[1] * prj[6] + mdl[2] * prj[10] + mdl[3] * prj[14];
		clip[3] = mdl[0] * prj[3] + mdl[1] * prj[7] + mdl[2] * prj[11] + mdl[3] * prj[15];
		clip[4] = mdl[4] * prj[0] + mdl[5] * prj[4] + mdl[6] * prj[8] + mdl[7] * prj[12];
		clip[5] = mdl[4] * prj[1] + mdl[5] * prj[5] + mdl[6] * prj[9] + mdl[7] * prj[13];
		clip[6] = mdl[4] * prj[2] + mdl[5] * prj[6] + mdl[6] * prj[10] + mdl[7] * prj[14];
		clip[7] = mdl[4] * prj[3] + mdl[5] * prj[7] + mdl[6] * prj[11] + mdl[7] * prj[15];
		clip[8] = mdl[8] * prj[0] + mdl[9] * prj[4] + mdl[10] * prj[8] + mdl[11] * prj[12];
		clip[9] = mdl[8] * prj[1] + mdl[9] * prj[5] + mdl[10] * prj[9] + mdl[11] * prj[13];
		clip[10] = mdl[8] * prj[2] + mdl[9] * prj[6] + mdl[10] * prj[10] + mdl[11] * prj[14];
		clip[11] = mdl[8] * prj[3] + mdl[9] * prj[7] + mdl[10] * prj[11] + mdl[11] * prj[15];
		clip[12] = mdl[12] * prj[0] + mdl[13] * prj[4] + mdl[14] * prj[8] + mdl[15] * prj[12];
		clip[13] = mdl[12] * prj[1] + mdl[13] * prj[5] + mdl[14] * prj[9] + mdl[15] * prj[13];
		clip[14] = mdl[12] * prj[2] + mdl[13] * prj[6] + mdl[14] * prj[10] + mdl[15] * prj[14];
		clip[15] = mdl[12] * prj[3] + mdl[13] * prj[7] + mdl[14] * prj[11] + mdl[15] * prj[15];
		
		// This will extract the RIGHT side of the frustum
		m_Frustum[(int)eFrustumSide.RIGHT, (int)ePlaneData.A] = clip[3] - clip[0];
		m_Frustum[(int)eFrustumSide.RIGHT, (int)ePlaneData.B] = clip[7] - clip[4];
		m_Frustum[(int)eFrustumSide.RIGHT, (int)ePlaneData.C] = clip[11] - clip[8];
		m_Frustum[(int)eFrustumSide.RIGHT, (int)ePlaneData.D] = clip[15] - clip[12];
		
		// This will extract the LEFT side of the frustum
		m_Frustum[(int)eFrustumSide.LEFT, (int)ePlaneData.A] = clip[3] + clip[0];
		m_Frustum[(int)eFrustumSide.LEFT, (int)ePlaneData.B] = clip[7] + clip[4];
		m_Frustum[(int)eFrustumSide.LEFT, (int)ePlaneData.C] = clip[11] + clip[8];
		m_Frustum[(int)eFrustumSide.LEFT, (int)ePlaneData.D] = clip[15] + clip[12];
		
		// This will extract the BOTTOM side of the frustum
		m_Frustum[(int)eFrustumSide.BOTTOM, (int)ePlaneData.A] = clip[3] + clip[1];
		m_Frustum[(int)eFrustumSide.BOTTOM, (int)ePlaneData.B] = clip[7] + clip[5];
		m_Frustum[(int)eFrustumSide.BOTTOM, (int)ePlaneData.C] = clip[11] + clip[9];
		m_Frustum[(int)eFrustumSide.BOTTOM, (int)ePlaneData.D] = clip[15] + clip[13];
		
		// This will extract the TOP side of the frustum
		m_Frustum[(int)eFrustumSide.TOP, (int)ePlaneData.A] = clip[3] - clip[1];
		m_Frustum[(int)eFrustumSide.TOP, (int)ePlaneData.B] = clip[7] - clip[5];
		m_Frustum[(int)eFrustumSide.TOP, (int)ePlaneData.C] = clip[11] - clip[9];
		m_Frustum[(int)eFrustumSide.TOP, (int)ePlaneData.D] = clip[15] - clip[13];
		
		// This will extract the BACK side of the frustum
		m_Frustum[(int)eFrustumSide.BACK, (int)ePlaneData.A] = clip[3] - clip[2];
		m_Frustum[(int)eFrustumSide.BACK, (int)ePlaneData.B] = clip[7] - clip[6];
		m_Frustum[(int)eFrustumSide.BACK, (int)ePlaneData.C] = clip[11] - clip[10];
		m_Frustum[(int)eFrustumSide.BACK, (int)ePlaneData.D] = clip[15] - clip[14];
		
		// This will extract the FRONT side of the frustum
		m_Frustum[(int)eFrustumSide.FRONT, (int)ePlaneData.A] = clip[3] + clip[2];
		m_Frustum[(int)eFrustumSide.FRONT, (int)ePlaneData.B] = clip[7] + clip[6];
		m_Frustum[(int)eFrustumSide.FRONT, (int)ePlaneData.C] = clip[11] + clip[10];
		m_Frustum[(int)eFrustumSide.FRONT, (int)ePlaneData.D] = clip[15] + clip[14];
		
		NormalizePlane (m_Frustum, (int)eFrustumSide.RIGHT);
		NormalizePlane (m_Frustum, (int)eFrustumSide.LEFT);
		NormalizePlane (m_Frustum, (int)eFrustumSide.TOP);
		NormalizePlane (m_Frustum, (int)eFrustumSide.BOTTOM);
		NormalizePlane (m_Frustum, (int)eFrustumSide.FRONT);
		NormalizePlane (m_Frustum, (int)eFrustumSide.BACK);
	}

	// This takes a 3-D point and returns TRUE if it’s inside of the frustum
	public bool PointInFrustum (float x, float y, float z)
	{
		// Go through all the sides of the frustum
		for (int i = 0; i < 6; i++) {
			// Calculate the plane equation and check if
			// the point is behind a side of the frustum
			if (m_Frustum[i, (int)ePlaneData.A] * x + m_Frustum[i, (int)ePlaneData.B] * y + m_Frustum[i, (int)ePlaneData.C] * z + m_Frustum[i, (int)ePlaneData.D] <= 0) {
				// The point was behind a side, so it ISN’T in the frustum
				return false;
			}
		}
		
		// The point was inside of the frustum
		return true;
	}

	// This takes a 3-D point and a radius and returns TRUE if the sphere is inside of the frustum
	public bool SphereInFrustum (float x, float y, float z, float radius)
	{
		// Go through all the sides of the frustum
		for (int i = 0; i < 6; i++) {
			// If the sphere’s center is farther away from
			// the plane than the size of the radius
			if (m_Frustum[i, (int)ePlaneData.A] * x + m_Frustum[i, (int)ePlaneData.B] * y + m_Frustum[i, (int)ePlaneData.C] * z + m_Frustum[i, (int)ePlaneData.D] <= -radius) {
				// The distance was greater than the radius
				// so the sphere is outside of the frustum
				return false;
			}
		}
		// The sphere was inside of the frustum!
		return true;
	}

	// This takes the center and half the length of the cube.
	public bool CubeInFrustum (float x, float y, float z, float size)
	{
		// Go through the frustum planes and make sure that at
		// least 1 point is in front of each plane
		for (int i = 0; i < 6; i++) {
			if (m_Frustum[i, (int)ePlaneData.A] * (x - size) + m_Frustum[i, (int)ePlaneData.B] * (y - size) + m_Frustum[i, (int)ePlaneData.C] * (z - size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			if (m_Frustum[i, (int)ePlaneData.A] * (x + size) + m_Frustum[i, (int)ePlaneData.B] * (y - size) + m_Frustum[i, (int)ePlaneData.C] * (z - size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			if (m_Frustum[i, (int)ePlaneData.A] * (x - size) + m_Frustum[i, (int)ePlaneData.B] * (y + size) + m_Frustum[i, (int)ePlaneData.C] * (z - size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			if (m_Frustum[i, (int)ePlaneData.A] * (x + size) + m_Frustum[i, (int)ePlaneData.B] * (y + size) + m_Frustum[i, (int)ePlaneData.C] * (z - size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			if (m_Frustum[i, (int)ePlaneData.A] * (x - size) + m_Frustum[i, (int)ePlaneData.B] * (y - size) + m_Frustum[i, (int)ePlaneData.C] * (z + size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			if (m_Frustum[i, (int)ePlaneData.A] * (x + size) + m_Frustum[i, (int)ePlaneData.B] * (y - size) + m_Frustum[i, (int)ePlaneData.C] * (z + size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			if (m_Frustum[i, (int)ePlaneData.A] * (x - size) + m_Frustum[i, (int)ePlaneData.B] * (y + size) + m_Frustum[i, (int)ePlaneData.C] * (z + size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			if (m_Frustum[i, (int)ePlaneData.A] * (x + size) + m_Frustum[i, (int)ePlaneData.B] * (y + size) + m_Frustum[i, (int)ePlaneData.C] * (z + size) + m_Frustum[i, (int)ePlaneData.D] > 0)
				continue;
			// If we get here, there was no point in the cube that was in
			// front of this plane, so the whole cube is behind this plane
			return false;
		}
		// By getting here it states that the cube is inside of the frustum
		return true;
	}

	public void NormalizePlane (float[,] frustum, int side)
	{
		float magnitude = (float)Mathf.Sqrt (frustum[side, (int)ePlaneData.A] * frustum[side, (int)ePlaneData.A] + frustum[side, (int)ePlaneData.B] * frustum[side, (int)ePlaneData.B] + frustum[side, (int)ePlaneData.C] * frustum[side, (int)ePlaneData.C]);
		
		// Divide the plane’s values by its magnitude.
		frustum[side, (int)ePlaneData.A] /= magnitude;
		frustum[side, (int)ePlaneData.B] /= magnitude;
		frustum[side, (int)ePlaneData.C] /= magnitude;
		frustum[side, (int)ePlaneData.D] /= magnitude;
	}

	public enum eFrustumSide
	{
		RIGHT = 0,
		// The RIGHT side of the frustum
		LEFT = 1,
		// The LEFT side of the frustum
		BOTTOM = 2,
		// The BOTTOM side of the frustum
		TOP = 3,
		// The TOP side of the frustum
		BACK = 4,
		// The BACK side of the frustum
		FRONT = 5
		// The FRONT side of the frustum
	}

	public enum ePlaneData
	{
		A = 0,
		// The X value of the plane’s normal
		B = 1,
		// The Y value of the plane’s normal
		C = 2,
		// The Z value of the plane’s normal
		D = 3
		// The distance the plane is from the origin
	}
}
