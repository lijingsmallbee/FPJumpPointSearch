/*  This file is part of the "Simple Waypoint System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them directly or indirectly
 *  from Rebound Games. You shall not license, sublicense, sell, resell, transfer, assign,
 *  distribute or otherwise make available to any third party the Service or the Content. 
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TrueSync;
namespace SWS
{
    /// <summary>
    /// Waypoint and path creation editor.
    /// <summary>
	struct MapGridT 
	{
		public FP X;
		public FP Z;
		public int Width;
		public int Height;
		public FP GridSize;
		public int[] pGrid;
	};
	class Polygon
	{
		public List<Vector2> m_Points = new List<Vector2>();
	}
    [CustomEditor(typeof(GridPlane))]
	public class GridPlaneEditor : Editor
    {
        //manager reference
		GridPlane editPlane;
        //if we are placing new waypoints in editor
        private bool placing = false;
        public void OnSceneGUI()
        {
            //with creation mode enabled, place new waypoints on keypress
            if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.P)
            {
                //cast a ray against mouse position
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;
				MeshCollider collider = editPlane.GetComponent<MeshCollider>();
				if(collider.Raycast(worldRay,out hitInfo,100f))
                {
					Vector3 offset = editPlane.transform.position;
                    Event.current.Use();
					Vector3 pos = hitInfo.point;
					float fRow = (pos.z - offset.z)/editPlane.gridsize;
					float fColumn = (pos.x - offset.x)/editPlane.gridsize;
                    int row = (int)fRow;
					int column = (int)fColumn;
					if(placing)
					{
						editPlane.SetGrid(row,column);
					}
					else
					{
						editPlane.ResetGrid(row,column);
					}
                 }
            }
			else if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.W)
			{
				Vector3 pos = editPlane.transform.position;
				pos.y += .1f;
				editPlane.transform.position = pos;
			}
			else if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.S)
			{
				Vector3 pos = editPlane.transform.position;
				pos.y -= .1f;
				editPlane.transform.position = pos;
			}
        }


        public override void OnInspectorGUI()
        {
			editPlane = target as GridPlane;
            //show default variables of manager
            DrawDefaultInspector();
            //get manager reference
            EditorGUIUtility.LookLikeControls();
            EditorGUILayout.Space();
      
            EditorGUILayout.BeginHorizontal();

           	if(GUILayout.Button("One Point",GUILayout.Height(40)))
			{
				editPlane.SetBrushType(0);
				SceneView.currentDrawingSceneView.Focus();
			}
			if(GUILayout.Button("three Point",GUILayout.Height(40)))
			{
				editPlane.SetBrushType(1);
				SceneView.currentDrawingSceneView.Focus();
			}
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
			
			if (GUILayout.Button("Generate", GUILayout.Height(40)))
			{
				editPlane.GenerateMesh();
			}
            //draw path creation button
			GUI.backgroundColor = Color.yellow;
            if (!placing && GUILayout.Button("Brush", GUILayout.Height(40)))
            {
				
                //we passed all prior checks, toggle waypoint placement
                placing = true;
                //focus sceneview for placement
                SceneView.currentDrawingSceneView.Focus();
            }

			if (placing && GUILayout.Button("Erase", GUILayout.Height(40)))
			{
				//we passed all prior checks, toggle waypoint placement
				placing = false;
				//focus sceneview for placement
				SceneView.currentDrawingSceneView.Focus();
			}
			GUI.backgroundColor = Color.white;
			if (GUILayout.Button("ExportCollision", GUILayout.Height(40)))
			{
				string filepath = EditorUtility.SaveFilePanelInProject("Save Map","CollisionInfo","bytes","OKOK");
				LogManager.Log(filepath);
				FileStream fs = new FileStream(filepath,FileMode.Create,FileAccess.Write);
				BinaryWriter bw = new BinaryWriter(fs);
			//	string fileName = filepath;//文件名字
				Vector3 offset = editPlane.transform.position;
                //write offset x
                FP offsetX = (FP)(offset.x);
				bw.Write(offsetX.RawValue);
                //write offset z
                FP offsetZ = (FP)offset.z;
				bw.Write(offsetZ.RawValue);
				//write width
				bw.Write(editPlane.width);
				//write height
				bw.Write(editPlane.height);
				//write size
				bw.Write(editPlane.gridsize);
				int [] gridInfo = editPlane.GetGridInfo();
				int len = gridInfo.Length;
				BitArray bitarray = new BitArray(len);
				int byteCount = (len+7) /8;
				byte[] data = new byte[byteCount];
				for(int i = 0; i < editPlane.height; ++ i)
				{
					for(int j=0;j< editPlane.width; ++j)
					{
						int index = i*editPlane.width + j;
						if(gridInfo[i*editPlane.width + j] == 0)
						{
							bitarray.Set(index,false);
						}
						else
						{
							bitarray.Set(index,true);
						}
					}
				}
				//要写的数据源
				bitarray.CopyTo(data,0);
				for(int i=0;i<data.Length;++i)
				{
					bw.Write(data[i]);
				}
				bw.Flush();
				bw.Close();
				fs.Close();
			}
			if (GUILayout.Button("Export", GUILayout.Height(40)))
			{
				string filepath = EditorUtility.SaveFilePanelInProject("Save Map","GridInfo","txt","OKOK");
				LogManager.Log(filepath);
				string fileName = filepath;//文件名字
				
				StringBuilder sb = new StringBuilder();
				//offset
				sb.Append("type octile").Append("\r\n");
				Vector3 offset = editPlane.transform.position;
                FP x = (FP)offset.x;
				sb.Append("X ").Append(x.RawValue).Append("\r\n");
                FP z = (FP)offset.z;
				sb.Append("Z ").Append(z.RawValue).Append("\r\n");
				sb.Append("width ").Append(editPlane.width).Append("\r\n"); 
				sb.Append("height ").Append(editPlane.height).Append("\r\n");
                FP size = (FP)editPlane.gridsize;
				sb.Append("size ").Append(size.RawValue).Append("\r\n"); 
				sb.Append("map").Append("\r\n");
				int [] gridInfo = editPlane.GetGridInfo();
			//	int len = gridInfo.Length;
				for(int i = 0; i < editPlane.height; ++ i)
				{
					for(int j=0;j< editPlane.width; ++j)
					{
						if(gridInfo[i*editPlane.width + j] == 0)
						{
							sb.Append('@');
						}
						else
						{
							sb.Append('.');
						}
					}
					sb.Append("\r\n");
				}
				//要写的数据源
				EditorUtils.SaveTextFile(filepath,sb.ToString());	
			}

			if (GUILayout.Button("Import", GUILayout.Height(40)))
			{
				string filepath = EditorUtility.OpenFilePanel("Load map",Application.dataPath,"txt");
				if(filepath != null)
				{
					StreamReader reader = new StreamReader(filepath,new UTF8Encoding(false));
					if(reader != null)
					{
						MapGridT myGrid = new MapGridT();
						string content = reader.ReadToEnd();
						int readPos = 0;
						List<string> kv = null;
						while( readPos< content.Length )
						{
							string line = EditorUtils.readLine( content, ref readPos );
							kv = EditorUtils.splitLine(line);
							if( kv.Count == 0 )
								continue;
							if(kv[0] == "map")
								break;
							if(kv[0] == "X")
							{
								if( kv.Count >1)
								{
									myGrid.X = float.Parse(kv[1]);
								}
							}
							if( kv[0] == "Z" )
							{
								if( kv.Count>1)
									myGrid.Z = float.Parse(kv[1]);
							}
							if( kv[0] == "width")
							{
								if( kv.Count>1)
									myGrid.Width = int.Parse(kv[1]);
							}
							if( kv[0] == "height")
							{
								if( kv.Count>1)
									myGrid.Height = int.Parse(kv[1]);
							}
							if( kv[0] == "size")
							{
								if( kv.Count>1)
									myGrid.GridSize = float.Parse(kv[1]);
							}
						}
						if( myGrid.Width == 0 || myGrid.Height==0)
						{
							StringBuilder log = new StringBuilder();
							log.Append("invlid width").Append(myGrid.Width).Append("or height").Append(myGrid.Height);
							LogManager.Log(log.ToString());
							return;
						}
						editPlane.transform.position = new Vector3((float)myGrid.X,0f,(float)myGrid.Z);
						editPlane.width = myGrid.Width;
						editPlane.height = myGrid.Height;
						editPlane.gridsize = (float)myGrid.GridSize;
						editPlane.GenerateMesh();
						int[] gridData = editPlane.GetGridInfo();
						
						for( int i=0; i<myGrid.Height; i++ )
						{
							string line = EditorUtils.readLine(content,ref readPos);
							if( line.Length < myGrid.Width+1 )
							{
								StringBuilder log = new StringBuilder();
								log.Append("line").Append(i).Append("length is less than").Append(myGrid.Width);
								LogManager.Log(log.ToString());
								return;
							}
							for( int w=0; w<myGrid.Width; w++ )
							{
								if( line[w] == '.' )
									gridData[i*myGrid.Width+w] = 1;
								else
									gridData[i*myGrid.Width+w] = 0;
							}
						}
						
					}
				}
			}
			if (GUILayout.Button("ImportCollision", GUILayout.Height(40)))
			{
				string filepath = EditorUtility.OpenFilePanel("Load collision",Application.dataPath,"bytes");
				if(filepath != null)
				{
					FileStream fs = new FileStream(filepath,FileMode.Open,FileAccess.Read);
					if(fs != null)
					{
						BinaryReader br = new BinaryReader(fs);
						MapGridT myGrid = new MapGridT();
						//read offset x
						myGrid.X = br.ReadSingle();
						//read offset z
						myGrid.Z = br.ReadSingle();
						//read width
						myGrid.Width = br.ReadInt32();
						//read height
						myGrid.Height = br.ReadInt32();
						//read size
						myGrid.GridSize = br.ReadSingle();
						if( myGrid.Width == 0 || myGrid.Height==0)
						{
							StringBuilder log = new StringBuilder();
							log.Append("invlid width").Append(myGrid.Width).Append("or height").Append(myGrid.Height);
							LogManager.Log(log.ToString());
							return;
						}
						editPlane.transform.position = new Vector3((float)myGrid.X,0f,(float)myGrid.Z);
						editPlane.width = myGrid.Width;
						editPlane.height = myGrid.Height;
						editPlane.gridsize = (float)myGrid.GridSize;
						editPlane.GenerateMesh();
						int [] gridInfo = editPlane.GetGridInfo();
						int len = gridInfo.Length;
						int byteCount = (len+7) /8;
						byte[] data = br.ReadBytes(byteCount);
						BitArray bitarray = new BitArray(data);
						for( int i=0; i<myGrid.Height; i++ )
						{
							for( int w=0; w<myGrid.Width; w++ )
							{
								gridInfo[i*myGrid.Width+w] = bitarray.Get(i*myGrid.Width+w) == true ? 1:0;
							}
						}
						
					}
				}
			}


			if (GUILayout.Button("CreateFromPath", GUILayout.Height(40)))
			{
	
				int walkNum = 0,nonWalkNum = 0;
				Polygon[] walkables = new Polygon[16];
				for(int a = 0;a<walkables.Length;++a)
				{
					walkables[a] = new Polygon();
				}
				Polygon[] nonWalkables = new Polygon[32];
				for(int b = 0;b<nonWalkables.Length;++b)
				{
					nonWalkables[b] = new Polygon();
				}
				//get all pathes
				WaypointManager manager = GameObject.FindObjectOfType<WaypointManager>();
				if(manager)
				{
					Vector3 offset = editPlane.transform.position;
					PathManager[] pathes = manager.GetComponentsInChildren<PathManager>();
					foreach(PathManager path in pathes)
					{
						if(path.walkable)
						{
							for(int i=0;i<path.waypoints.Length;++i)
							{
								Vector3 pos = path.waypoints[i].position;
								walkables[walkNum].m_Points.Add(new Vector2(pos.x-offset.x,pos.z-offset.z));
							}
							Vector3 firstPos = path.waypoints[0].position;
							walkables[walkNum].m_Points.Add(new Vector2(firstPos.x-offset.x,firstPos.z-offset.z));
							walkNum ++;
						}
						else
						{
							for(int i=0;i<path.waypoints.Length;++i)
							{
								Vector3 pos = path.waypoints[i].position;
								nonWalkables[nonWalkNum].m_Points.Add(new Vector2(pos.x-offset.x,pos.z-offset.z));
							}
							Vector3 firstPos = path.waypoints[0].position;
							nonWalkables[nonWalkNum].m_Points.Add(new Vector2(firstPos.x-offset.x,firstPos.z-offset.z));
							nonWalkNum ++;
						}
					}
				}
				int[] gridData = editPlane.GetGridInfo();
				
				for( int i=0; i<editPlane.height; i++ )
				{
					for( int w=0; w<editPlane.width; w++ )
					{
						bool processed = false;
						Vector2 pos = new Vector2(w*editPlane.gridsize + editPlane.gridsize*0.5f,i*editPlane.gridsize + editPlane.gridsize*0.5f);
						for(int nonWalkIndex = 0; nonWalkIndex < nonWalkNum; ++nonWalkIndex)
						{
							if(PtInPolygon(pos,nonWalkables[nonWalkIndex].m_Points))
							{
								processed = true;
								gridData[i*editPlane.width + w] = 0;
							}
						} 
						if(!processed)
						{
							for(int walkIndex = 0; walkIndex < walkNum; ++walkIndex)
							{
								if(PtInPolygon(pos,walkables[walkIndex].m_Points))
								{
									processed = true;
									gridData[i*editPlane.width + w] = 1;
								}
							} 
						}
					}
				}
			}
					EditorGUILayout.Space();
            //draw instructions
            GUILayout.TextArea("Hint:\nPress 'Brush' to begin a new path, then press 'p' "
                            + "on your keyboard to place green points in the SceneView "
                            + "\n\nPress 'Erase' to do the opposite operation.");
        }

		bool PtInPolygon ( Vector2 pt, List<Vector2> vPolygon) 
		{ 
			int nCross = 0; 
			int polygonC = vPolygon.Count;
			for (int i = 0; i <polygonC-1; i++) 
			{
				Vector2 p1 = vPolygon[i];
				Vector2 p2 = vPolygon[i+1];
				
				// 求解 z=p.z 与 p1p2 的交点
				if(Mathf.Abs(p1.y - p2.y) < 1e-4f)
				{
					continue;
				} 
			//	if ( p1.nZ == p2.nZ ) // p1p2 与 y=p0.y平行 
			//		continue; 
				
				if ( pt.y < Mathf.Min(p1.y, p2.y) ) // 交点在p1p2延长线上 
					continue; 
				if ( pt.y >= Mathf.Max(p1.y, p2.y) ) // 交点在p1p2延长线上 
					continue; 
			
				// 求交点的 X 坐标 -------------------------------------------------------------- 
				float x = (pt.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x; 
				
				if ( x > pt.x ) 
					nCross++; // 只统计单边交点 
			} 
			
			// 单边交点为偶数，点在多边形之外 --- 
			return (nCross % 2 == 1); 
		} 
		
	}
}
