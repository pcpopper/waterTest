using System.Collections;
using UnityEngine;

//An octree implementation used to devide a world into managable
//sections
[System.Serializable]
public class cOctree
{
	//Member Variables
	////////////////////////////////////
	//Private
	public Vector3[] m_pVertices;
	//This stores the triangles that should be drawn with this node
	public Vector3 m_vCenter = Vector3.zero;
	//This is the center (X, Y, Z) point for this node
	public int m_TriangleCount = 0;
	//This holds the amount of triangles stored in this node
	public float m_Width = 0.0f;
	//This is the size of the cube for this current node
	public bool m_bSubDivided = false;
	//This tells us if we have divided this node into more subnodes
	public int g_CurrentSubdivision = 0;
	
	//Public
	public int g_MaxSubdivisions = 3;
	public int g_MaxTriangles = 100;
	public int g_EndNodeCount = 0;
	public cOctree[] m_pOctreeNodes;

	//Properties
	////////////////////////////////////
	
	//Returns the center of this node
	public Vector3 Center {
		get { return m_vCenter; }
		set { m_vCenter = value; }
	}

	//Returns the triangle count stored in this node
	public int TriangleCount {
		get { return m_TriangleCount; }
		set { m_TriangleCount = value; }
	}

	//Returns the width of this node (A cube's dimensions are the same)
	public float Width {
		get { return m_Width; }
		set { m_Width = value; }
	}

	//Returns true if the node is subdivided, possibly making it an end node
	public bool SubDivided {
		get { return m_bSubDivided; }
	}
	
	//Methods
	////////////////////////////////////
	
	//This sets the initial width, height and depth for the whole scene
	public void GetSceneDimensions (Vector3[] pVertices, int numberOfVerts)
	{
		//Go through all of the vertices and add them up to find the center
		for (int i = 0; i < numberOfVerts; i++) {
			//Add the current vertex to the center variable (operator overloaded)
			m_vCenter = m_vCenter + pVertices[i];
		}
		//Divide the total by the number of vertices to get the center point.
		//We could have overloaded the / symbol but I chose not to because we
		//rarely use it in the code.
		m_vCenter.x /= numberOfVerts;
		m_vCenter.y /= numberOfVerts;
		m_vCenter.z /= numberOfVerts;
		
		float maxWidth = 0;
		float maxHeight = 0;
		float maxDepth = 0;
		//Go through all of the vertices and find the max dimensions
		for (int i = 0; i < numberOfVerts; i++) {
			//Get the current dimensions for this vertex. abs() is used
			//to get the absolute value because it might return a negative number.
			float currentWidth = Mathf.Abs (pVertices[i].x - m_vCenter.x);
			float currentHeight = Mathf.Abs (pVertices[i].y - m_vCenter.y);
			float currentDepth = Mathf.Abs (pVertices[i].z - m_vCenter.z);
			//Check if the current width is greater than the max width stored.
			if (currentWidth > maxWidth)
				maxWidth = currentWidth;
			//Check if the current height is greater than the max height stored.
			if (currentHeight > maxHeight)
				maxHeight = currentHeight;
			//Check if the current depth is greater than the max depth stored.
			if (currentDepth > maxDepth)
				maxDepth = currentDepth;
		}
		//Get the full width, height, and depth
		maxWidth *= 2;
		maxHeight *= 2;
		maxDepth *= 2;
		
		if (maxWidth >= maxHeight && maxWidth >= maxDepth)
			//Check if the width is the highest and assign that for the cube dimension
			m_Width = maxWidth; else if (maxHeight >= maxWidth && maxHeight >= maxDepth)
			m_Width = maxHeight;
		else
			//Check if height is the highest and assign that for the cube dimension
			m_Width = maxDepth;
		//Else it must be the “depth” or it’s the same value as the other ones
	}

	//This Subdivides a node depending on the triangle and node width
	public void CreateNode (Vector3[] pVertices, int numberOfVerts, Vector3 vCenter, float width)
	{
		//Create a variable to hold the number of triangles
		int numberOfTriangles = numberOfVerts / 3;
		//Initialize the node’s center point. Now we know the center of this node.
		m_vCenter = vCenter;
		//Initialize the node’s cube width. Now we know the width of this node.
		m_Width = width;
		if ((numberOfTriangles > g_MaxTriangles) && (g_CurrentSubdivision < g_MaxSubdivisions)) {
			m_bSubDivided = true;
			
			//Create the list of booleans for each triangle index
			bool[] pList1 = new bool[numberOfTriangles];
			//TOP_LEFT_FRONT node list
			bool[] pList2 = new bool[numberOfTriangles];
			//TOP_LEFT_BACK node list
			bool[] pList3 = new bool[numberOfTriangles];
			//TOP_RIGHT_BACK node list
			bool[] pList4 = new bool[numberOfTriangles];
			//TOP_RIGHT_FRONT node list
			bool[] pList5 = new bool[numberOfTriangles];
			//BOTTOM_LEFT_FRONT node list
			bool[] pList6 = new bool[numberOfTriangles];
			//BOTTOM_LEFT_BACK node list
			bool[] pList7 = new bool[numberOfTriangles];
			//BOTTOM_RIGHT_BACK node list
			bool[] pList8 = new bool[numberOfTriangles];
			//BOTTOM_RIGHT_FRONT node list
			// Create a variable to cut down the thickness of the code below
			Vector3 vCtr = vCenter;
			
			for (int i = 0; i < numberOfVerts; i++) {
				// Create a variable to cut down the thickness of the code
				Vector3 vPt = pVertices[i];
				//Check if the point lines within the TOP LEFT FRONT node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z >= vCtr.z))
					pList1[i / 3] = true;
				//Check if the point lines within the TOP LEFT BACK node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z <= vCtr.z))
					pList2[i / 3] = true;
				//Check if the point lines within the TOP RIGHT BACK node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z <= vCtr.z))
					pList3[i / 3] = true;
				//Check if the point lines within the TOP RIGHT FRONT node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z <= vCtr.z))
					pList4[i / 3] = true;
				//Check if the point lines within the BOTTOM LEFT FRONT node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z <= vCtr.z))
					pList5[i / 3] = true;
				//Check if the point lines within the BOTTOM LEFT BACK node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z <= vCtr.z))
					pList6[i / 3] = true;
				//Check if the point lines within the BOTTOM RIGHT BACK node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z <= vCtr.z))
					pList7[i / 3] = true;
				//Check if the point lines within the BOTTOM RIGHT FRONT node
				if ((vPt.x <= vCtr.x) && (vPt.y >= vCtr.y) && (vPt.z <= vCtr.z))
					pList8[i / 3] = true;
			}
			
			//Create a variable for each list that holds how many triangles
			//were found for each of the 8 subdivided nodes.
			int triCount1 = 0;
			int triCount2 = 0;
			int triCount3 = 0;
			int triCount4 = 0;
			int triCount5 = 0;
			int triCount6 = 0;
			int triCount7 = 0;
			int triCount8 = 0;
			
			//Go through each of the lists and increase the
			//triangle count for each node.
			for (int i = 0; i < numberOfTriangles; i++) {
				//Increase the triangle count for each
				//node that has a “true” for the index i.
				if (pList1[i])
					triCount1++;
				if (pList2[i])
					triCount2++;
				if (pList3[i])
					triCount3++;
				if (pList4[i])
					triCount4++;
				if (pList5[i])
					triCount5++;
				if (pList6[i])
					triCount6++;
				if (pList7[i])
					triCount7++;
				if (pList8[i])
					triCount8++;
			}
			
			m_pOctreeNodes = new cOctree[8];
			
			//Create the subdivided nodes if necessary and then
			//recurse through them. The information passed into CreateNewNode() is
			//essential for creating the new nodes. We pass in one of the 8 ID’s
			//so it knows how to calculate it’s new center.
			CreateNewNode (pVertices, pList1, numberOfVerts, vCenter, width, triCount1, (int)eOctreeNodes.TOP_LEFT_FRONT);
			CreateNewNode (pVertices, pList2, numberOfVerts, vCenter, width, triCount2, (int)eOctreeNodes.TOP_LEFT_BACK);
			CreateNewNode (pVertices, pList3, numberOfVerts, vCenter, width, triCount3, (int)eOctreeNodes.TOP_RIGHT_BACK);
			CreateNewNode (pVertices, pList4, numberOfVerts, vCenter, width, triCount3, (int)eOctreeNodes.TOP_RIGHT_FRONT);
			CreateNewNode (pVertices, pList5, numberOfVerts, vCenter, width, triCount3, (int)eOctreeNodes.BOTTOM_LEFT_FRONT);
			CreateNewNode (pVertices, pList6, numberOfVerts, vCenter, width, triCount3, (int)eOctreeNodes.BOTTOM_LEFT_BACK);
			CreateNewNode (pVertices, pList7, numberOfVerts, vCenter, width, triCount3, (int)eOctreeNodes.BOTTOM_RIGHT_BACK);
			CreateNewNode (pVertices, pList8, numberOfVerts, vCenter, width, triCount3, (int)eOctreeNodes.BOTTOM_RIGHT_FRONT);
		} else {
			//Assign the vertices to this node since we reached an end node
			AssignVerticesToNode (pVertices, numberOfVerts);
		}
		
		
	}

	//This goes through each node and then draws the end node's verticies
	//This function should be called by starting with the rood node.
	public void DrawOctree (cOctree pNode, cFrustum g_Frustum)
	{
		//Make sure a valid node was passed in; otherwise go back to the last node
		if (pNode == null)
			return;
		
		// Make sure its dimensions are within our frustum
		if(!g_Frustum.CubeInFrustum(pNode.m_vCenter.x, pNode.m_vCenter.y, pNode.m_vCenter.z, pNode.m_Width / 2)) {
			return;
		}
		
		//Check if this node is subdivided. If so, then we need to draw its nodes
		if (pNode.SubDivided) {
			//Recurse to the bottom of these nodes and draw
			//the end node’s vertices, Like creating the octree,
			//we need to recurse through each of the 8 nodes.
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.TOP_LEFT_FRONT], g_Frustum);
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.TOP_LEFT_BACK], g_Frustum);
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.TOP_RIGHT_BACK], g_Frustum);
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.TOP_RIGHT_FRONT], g_Frustum);
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.BOTTOM_LEFT_FRONT], g_Frustum);
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.BOTTOM_LEFT_BACK], g_Frustum);
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.BOTTOM_RIGHT_BACK], g_Frustum);
			DrawOctree (pNode.m_pOctreeNodes[(int)eOctreeNodes.BOTTOM_RIGHT_FRONT], g_Frustum);
		} else {
			//Make sure we have valid vertices assigned to this node
			if(pNode.m_pVertices.Length == 0) return;
			
			// Store the vertices in a local pointer to keep code more clean
			Vector3[] pVertices = pNode.m_pVertices;
			
			// Go through all of the vertices (the number of triangles * 3)
			for(int i = 0; i < pVertices.Length; i += 3)
			{
				Debug.DrawLine(pVertices[i], pVertices[i + 1]);
				Debug.DrawLine(pVertices[i + 1], pVertices[i + 2]);
				Debug.DrawLine(pVertices[i + 2], pVertices[i]);
			}
		}
	}

	//This frees the data allocated in the octree and restores the variables
	public void DestroyOctree ()
	{
		//Free the triangle data if it’s not NULL
		if (m_pVertices.Length != 0) {
			m_pVertices = null;
		}
		//Go through all of the nodes and free them if they were allocated
		for (int i = 0; i < 8; i++) {
			//Make sure this node is valid
			if (m_pOctreeNodes[i] != null) {
				//Free this array index. This will call the deconstructor,
				//which will free the octree data correctly. This allows
				//us to forget about a complicated clean up
				m_pOctreeNodes[i] = null;
			}
		}
		//Initialize the octree data members
		InitOctree ();
	}

	//This initializes the data members
	private void InitOctree ()
	{
		
	}

	//This takes in the previous nodes center, width and which node ID that will be subdivided
	private Vector3 GetNewNodeCenter (Vector3 vCenter, float width, int nodeID)
	{
		// Initialize the new node center
		Vector3 vNodeCenter = new Vector3 (0, 0, 0);
		// Create a dummy variable to cut down the code size
		Vector3 vCtr = vCenter;
		// Store the distance the new node center will be from the center
		float distance = width / 4.0f;
		// Switch on the ID to see which subdivided node we are finding the center
		switch (nodeID) {
		case (int)eOctreeNodes.TOP_LEFT_FRONT:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x - distance, vCtr.y + distance, vCtr.z + distance);
			break;
		case (int)eOctreeNodes.TOP_LEFT_BACK:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x - distance, vCtr.y + distance, vCtr.z - distance);
			break;
		case (int)eOctreeNodes.TOP_RIGHT_BACK:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x + distance, vCtr.y + distance, vCtr.z - distance);
			break;
		case (int)eOctreeNodes.TOP_RIGHT_FRONT:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x + distance, vCtr.y + distance, vCtr.z + distance);
			break;
		case (int)eOctreeNodes.BOTTOM_LEFT_FRONT:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x - distance, vCtr.y - distance, vCtr.z + distance);
			break;
		case (int)eOctreeNodes.BOTTOM_LEFT_BACK:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x - distance, vCtr.y - distance, vCtr.z - distance);
			break;
		case (int)eOctreeNodes.BOTTOM_RIGHT_BACK:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x + distance, vCtr.y - distance, vCtr.z - distance);
			break;
		case (int)eOctreeNodes.BOTTOM_RIGHT_FRONT:
			// Calculate the center of this new node
			vNodeCenter = new Vector3 (vCtr.x + distance, vCtr.y - distance, vCtr.z + distance);
			break;
		}
		
		// Return the new node center
		return vNodeCenter;
	}

	//Cleans up the subdivided node creation process, so our code isn't Huge
	private void CreateNewNode (Vector3[] pVertices, bool[] pList, int numberOfVerts, Vector3 vCenter, float width, int triangleCount, int nodeID)
	{
		//Check if the first node found some triangles in it, else, return
		if (triangleCount <= 0)
			return;
		
		//Allocate memory for the triangles found in this node
		Vector3[] pNodeVertices = new Vector3[triangleCount * 3];
		
		//Create a counter to count the current index of the new node vertices
		int index = 0;
		//Go through all the vertices and assign the vertices to the node’s list
		for (int i = 0; i < numberOfVerts; i++) {
			//If this current triangle is in the node, assign its vertices to it
			if (pList[i / 3]) {
				pNodeVertices[index] = pVertices[i];
				index++;
			}
		}
		
		//Allocate a new node for the octree
		m_pOctreeNodes[nodeID] = new cOctree ();
		
		//Get the new node’s center point depending on the nodeID
		//(nodeID: meaning, which of the 8 subdivided cubes).
		Vector3 vNodeCenter = GetNewNodeCenter (vCenter, width, nodeID);
		
		//Increase the current level of subdivision
		g_CurrentSubdivision++;
		
		//Recurse through this node and subdivide it if necessary
		m_pOctreeNodes[nodeID].CreateNode (pNodeVertices, triangleCount * 3, vNodeCenter, width / 2);
		m_pOctreeNodes[nodeID].g_CurrentSubdivision = g_CurrentSubdivision;
		//Decrease the current level of subdivision
		g_CurrentSubdivision--;
		
		
	}

	//This assigns the verticies to the end node
	private void AssignVerticesToNode (Vector3[] pVertices, int numberOfVerts)
	{
		// Since we did not subdivide this node we want to set our flag to false
		m_bSubDivided = false;
		m_TriangleCount = numberOfVerts / 3;
		// Allocate enough memory to hold the needed vertices for the triangles
		m_pVertices = pVertices;
		// Increase the amount of end nodes created (Nodes with vertices stored)
		g_EndNodeCount++;
	}

	//Constructor
	public cOctree ()
	{
	}

	public enum eOctreeNodes
	{
		TOP_LEFT_FRONT = 0,
		TOP_LEFT_BACK = 1,
		TOP_RIGHT_BACK = 2,
		TOP_RIGHT_FRONT = 3,
		BOTTOM_LEFT_FRONT = 4,
		BOTTOM_LEFT_BACK = 5,
		BOTTOM_RIGHT_BACK = 6,
		BOTTOM_RIGHT_FRONT = 7
	}
}
