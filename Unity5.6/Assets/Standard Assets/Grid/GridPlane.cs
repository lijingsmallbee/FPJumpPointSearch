/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SWS
{
    /// <summary>
    /// The editor part of this class allows you to create paths in 2D or 3D space.
    /// At runtime, it manages path instances for easy lookup of references.
    /// <summary>
//	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshCollider))]
    public class GridPlane : MonoBehaviour
    {
        public int width;
		public int height;
		public float gridsize;
		private Mesh mesh = null;
		private int[] m_grid = null;
		private int[] indices = null;
		private Vector3[] vertices = null;
		private Color[] colors = null;
		private int m_curWidth;
		private int m_curHeight;
		private int m_brushType = 1;
		public void SetBrushType(int brushType)
		{
			m_brushType = brushType;
		}
		public void SetGrid(int row,int column)
		{
			GridValue(row,column,1);
		}
		public void ResetGrid(int row,int column)
		{
			GridValue(row,column,0);
		}
		public int[] GetGridInfo()
		{
			return m_grid;
		}
		private void GridValue(int row,int column,int value)
		{
			m_grid[row*m_curWidth + column] = value;
			if(m_brushType == 1)
			{
				//this line
				if(column - 1 >= 0)
				{
					m_grid[row*m_curWidth + (column-1)] = value;
				}
				if((column +1) < m_curWidth)
				{
					m_grid[row*m_curWidth + (column+1)] = value;
				}
				//bottom line
				if( (row - 1) >= 0)
				{
					m_grid[(row-1)*m_curWidth + column] = value;
					if(column - 1 >= 0)
					{
						m_grid[(row-1)*m_curWidth + (column-1)] = value;
					}
					if((column +1) < m_curWidth)
					{
						m_grid[(row-1)*m_curWidth + (column+1)] = value;
					}
					
				}
				//up line
				if( (row + 1) < height)
				{
					m_grid[(row+1)*m_curWidth + column] = value;
					if(column - 1 >= 0)
					{
						m_grid[(row+1)*m_curWidth + (column-1)] = value;
					}
					if((column +1) < m_curWidth)
					{
						m_grid[(row+1)*m_curWidth + (column+1)] = value;
					}
					
				}
			}
		}
		public void GenerateMesh()
		{
			if( width <= 0 || height <= 0)
			{
				Debug.LogError("width or height can not be zero");
				return;
			}
			m_grid = new int[width * height];
			Mesh mesh = new Mesh();
			vertices = new Vector3[width * height * 6];
			indices = new int[width * height * 6];
			for(int tmpIndex = 0;tmpIndex < indices.Length;++tmpIndex)
			{
				indices[tmpIndex] = tmpIndex;
			}
			colors = new Color[width * height * 6];
			for(int tmpIndex = 0;tmpIndex < colors.Length;++tmpIndex)
			{
				colors[tmpIndex].r = 0f;
				colors[tmpIndex].a = 0.5f;
			}
			
			for(int i=0;i<height;++i)
			{
				for(int j=0;j<width;++j)
				{
					Vector3 leftbottom = new Vector3(j*gridsize,0f,i*gridsize);
					Vector3 rightbottom = new Vector3(j*gridsize + gridsize,0f,i*gridsize);
					Vector3 righttop = new Vector3(j*gridsize + gridsize,0f,i*gridsize + gridsize);
					Vector3 lefttop = new Vector3(j*gridsize,0f,i*gridsize + gridsize);
					int begin = (i*width + j)*6;
					vertices[begin] = leftbottom;
					vertices[begin + 1] = lefttop;
					vertices[begin + 2] = righttop;
					vertices[begin + 3] = leftbottom;
					vertices[begin + 4] = righttop;
					vertices[begin + 5] = rightbottom;
					m_grid[i*width + j] = 0;
				}
			}
			m_curWidth = width;
			m_curHeight = height;
			Vector3[] meshVertices = new Vector3[4];
			meshVertices[0] = new Vector3(0f,0f,0f);
			meshVertices[1] = new Vector3(0f,0f,height*gridsize);
			meshVertices[2] = new Vector3(width*gridsize,0f,height*gridsize);
			meshVertices[3] = new Vector3(width*gridsize,0f,0f);
			int[] meshIndices = new int[6];
			meshIndices[0] = 0;
			meshIndices[1] = 1;
			meshIndices[2] = 2;
			meshIndices[3] = 0;
			meshIndices[4] = 2;
			meshIndices[5] = 3;
			mesh.vertices = meshVertices;
			mesh.triangles = meshIndices;
			MeshFilter filter = GetComponent<MeshFilter>();
			if(filter == null)
			{
				gameObject.AddComponent<MeshFilter>();
			}
			GetComponent<MeshFilter>().mesh = mesh;
			GetComponent<MeshCollider>().sharedMesh = mesh;
		}
		void Awake()
		{
			gameObject.AddComponent<MeshFilter>();
		}

		void OnDrawGizmos()
		{
			Vector3 selfPos = transform.position;
			for(int i=0;i<m_curHeight;++i)
			{
				for(int j=0;j<m_curWidth;++j)
				{
					Color old = Gizmos.color;
					int begin = (i*m_curWidth + j)*6;
					Gizmos.DrawLine(vertices[begin] + selfPos,vertices[begin+1] + selfPos);
					Gizmos.DrawLine(vertices[begin+1]+ selfPos,vertices[begin+2]+ selfPos);
					if(m_grid[i*m_curWidth+j] == 0)
					{
						Gizmos.color = new Color(1f,0f,0f,0.5f);	
					}
					else
					{
						Gizmos.color = new Color(0f,1f,0f,0.5f);
						Gizmos.DrawLine(vertices[begin+2]+ selfPos,vertices[begin]+ selfPos);
						Gizmos.DrawLine(vertices[begin+1]+ selfPos,vertices[begin+5]+ selfPos);	
					}			
					
					Gizmos.color = old;
					Gizmos.DrawLine(vertices[begin+4]+ selfPos,vertices[begin+5]+ selfPos);
					Gizmos.DrawLine(vertices[begin+5]+ selfPos,vertices[begin+3]+ selfPos);
				}
			}
		} 
    } 
}