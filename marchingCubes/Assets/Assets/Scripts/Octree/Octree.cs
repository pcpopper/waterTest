using UnityEngine;
using System.Collections;

//Octree using cOctree and cFrustum
[System.Serializable]
public class Octree : MonoBehaviour
{
	public cOctree g_Octree = new cOctree();
	public cFrustum g_Frustum = new cFrustum();
	
	private bool initialized = false;
	// Use this for initialization
	public void Initialize ()
	{
		g_Frustum.CalculateFrustum();
		
		MeshFilter[] meshF = gameObject.GetComponentsInChildren<MeshFilter>();
		
		foreach (MeshFilter filter in meshF)
		{
			g_Octree.GetSceneDimensions(filter.mesh.vertices, filter.mesh.vertices.Length);
			g_Octree.CreateNode(filter.mesh.vertices, filter.mesh.vertices.Length, g_Octree.Center, g_Octree.Width);			
		}
		
		initialized = true;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if ( initialized )
		{
			g_Frustum.CalculateFrustum();
			g_Octree.DrawOctree(g_Octree, g_Frustum);
		}
	}
}

