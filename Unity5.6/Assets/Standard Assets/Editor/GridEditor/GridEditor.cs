using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using SWS;
using TrueSync;
namespace Nirvana.Scene
{
	[CustomEditor(typeof(Grid))]
	public class GridEditor : Editor
	{
		// The brush color
		static Color32 BrushColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		// The mouse select target.
		int m_mouseRow = -1;
		int m_mouseColumn = -1;

		public override void OnInspectorGUI()
		{
			var grid = target as Grid;
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Grid Row");
			grid.m_keepRow = EditorGUILayout.IntField(grid.m_keepRow);//("Grid Row", grid.m_keepRow, 1, 1024);
			GUILayout.EndHorizontal();
			for(int i = 0;i< 100;++i)
			{
				if(Grid.GridSizeDelta*i >= grid.m_keepRow)
				{
					grid.m_keepRow = Grid.GridSizeDelta*i;
					break;
				}
			}
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Grid Column");
			grid.m_keepColumn = EditorGUILayout.IntField(grid.m_keepColumn);//("Grid Column", grid.m_keepColumn, 1, 1024);
			GUILayout.EndHorizontal();
			for(int j = 0;j< 100;++j)
			{
				if(Grid.GridSizeDelta*j >= grid.m_keepColumn)
				{
					grid.m_keepColumn = Grid.GridSizeDelta*j;
					break;
				}
			}
			grid.GridSize = EditorGUILayout.Slider("Grid Size", grid.GridSize, 0.1f, 3.0f);

			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Resize"))
			{
				if(grid.m_keepRow != grid.Row || grid.m_keepColumn != grid.Column)
				{
					grid.Resize(grid.m_keepRow, grid.m_keepColumn);
					SceneView.RepaintAll();
				}
			}
			if(GUILayout.Button("Fit to ground"))
			{
				grid.FitToGround();
				SceneView.RepaintAll();
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Export"))
			{
				string filepath = EditorUtility.SaveFilePanelInProject("Save Map","GridInfo","txt","OKOK");
				LogManager.Log(filepath);
//				string fileName = filepath;//文件名字
				
				StringBuilder sb = new StringBuilder();
				//offset
				sb.Append("type octile").Append("\r\n");
				Vector3 offset = grid.transform.position;
                FP x = (FP)offset.x;
                sb.Append("X ").Append(x.RawValue).Append("\r\n");
                FP z = (FP)offset.z;
                sb.Append("Z ").Append(z.RawValue).Append("\r\n");
                sb.Append("width ").Append(grid.Column).Append("\r\n");
                sb.Append("height ").Append(grid.Row).Append("\r\n");
                FP size = (FP)grid.GridSize;
                sb.Append("size ").Append(size.RawValue).Append("\r\n"); 
				sb.Append("map").Append("\r\n");
				
				for(int i = 0; i < grid.Row; ++ i)
				{
					for(int j=0;j< grid.Column; ++j)
					{
						Cell c = grid.GetCell(i,j);
						if(c.collision == Cell.CollisionType.Unwalkable)
						{
							sb.Append('@');
						}
						else
						{
							int ascCode = (int)c.material + '0';
							sb.Append((char)ascCode);
						}
					}
					sb.Append("\r\n");
				}
				//要写的数据源
				EditorUtils.SaveTextFile(filepath,sb.ToString());	
			}
			if(GUILayout.Button("Import"))
			{
				string filepath = EditorUtility.OpenFilePanel("Load map",Application.dataPath,"txt");
				if(filepath != null)
				{
					StreamReader reader = new StreamReader(filepath,new UTF8Encoding(false));
					if(reader != null)
					{
						SWS.MapGridT myGrid = new SWS.MapGridT();
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
									myGrid.X = FP.FromRaw(long.Parse(kv[1]));
								}
							}
							if( kv[0] == "Z" )
							{
								if( kv.Count>1)
									myGrid.Z = FP.FromRaw(long.Parse(kv[1]));
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
									myGrid.GridSize = FP.FromRaw(long.Parse(kv[1]));
							}
						}
						if( myGrid.Width == 0 || myGrid.Height==0)
						{
							StringBuilder log = new StringBuilder();
							log.Append("invlid width").Append(myGrid.Width).Append("or height").Append(myGrid.Height);
							LogManager.Log(log.ToString());
							return;
						}
						grid.transform.position = new Vector3((float)myGrid.X,0f,(float)myGrid.Z);
						grid.GridSize = (float)myGrid.GridSize;
						grid.Resize(myGrid.Height,myGrid.Width,false);
                        grid.m_keepRow = myGrid.Height;
                        grid.m_keepColumn = myGrid.Width;
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
								Cell c = grid.GetCell(i,w);
								if( line[w] == '@' )
									grid.SetCellCollision(i,w,Cell.CollisionType.Unwalkable);
								else
								{
									grid.SetCellCollision(i,w,Cell.CollisionType.Walkable);
									int enumNum = line[w]-'0';
									c.material = (Cell.MaterialType)enumNum;
								}
							}
						}
						SceneView.RepaintAll();
					}
				}
			}

            if (GUILayout.Button("MakeAllWalkable"))
            {
            //    Grid grid = target as Grid;
                for (int i = 0; i < grid.Row; ++i)
                {
                    for (int j = 0; j < grid.Column; ++j)
                    {
                        grid.SetCellCollision(i, j, Cell.CollisionType.Walkable);
                    }

                }
            }

                if (GUILayout.Button("CreateFromPath"))
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
					Vector3 offset = grid.transform.position;
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
				//int[] gridData = editPlane.GetGridInfo();
				
				for( int i=0; i<grid.Row; i++ )
				{
					for( int w=0; w<grid.Column; w++ )
					{
						float gridSize = grid.GridSize;
						bool processed = false;
						Vector2 pos = new Vector2(w*gridSize + gridSize*0.5f,i*gridSize + gridSize*0.5f);
						for(int nonWalkIndex = 0; nonWalkIndex < nonWalkNum; ++nonWalkIndex)
						{
							if(PtInPolygon(pos,nonWalkables[nonWalkIndex].m_Points))
							{
								processed = true;
								grid.SetCellCollision(i,w,Cell.CollisionType.Unwalkable);
							}
						} 
						if(!processed)
						{
							for(int walkIndex = 0; walkIndex < walkNum; ++walkIndex)
							{
								if(PtInPolygon(pos,walkables[walkIndex].m_Points))
								{
									processed = true;
									grid.SetCellCollision(i,w,Cell.CollisionType.Walkable);
								}
							} 
						}
					}
				}
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();

			GUILayout.Label("Brush Shap: ");
			GUILayout.BeginHorizontal();
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Rect1x1), "Rect1x1"))
            {
				grid.m_brushType = Grid.BrushType.Rect1x1;
			}
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Rect3x3), "Rect3x3"))
			{
				grid.m_brushType = Grid.BrushType.Rect3x3;
            }
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Rect5x5), "Rect5x5"))
			{
				grid.m_brushType = Grid.BrushType.Rect5x5;
            }
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Rect7x7), "Rect7x7"))
			{
				grid.m_brushType = Grid.BrushType.Rect7x7;
            }
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Rect9x9), "Rect9x9"))
			{
				grid.m_brushType = Grid.BrushType.Rect9x9;
			}
            GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Cross3x3), "Cross3x3"))
			{
				grid.m_brushType = Grid.BrushType.Cross3x3;
			}
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Cross5x5), "Cross5x5"))
			{
				grid.m_brushType = Grid.BrushType.Cross5x5;
			}
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Cross7x7), "Cross7x7"))
			{
				grid.m_brushType = Grid.BrushType.Cross7x7;
			}
			if(GUILayout.Toggle((grid.m_brushType == Grid.BrushType.Cross9x9), "Cross9x9"))
			{
				grid.m_brushType = Grid.BrushType.Cross9x9;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();

			GUILayout.Label("Paint: ");
			GUILayout.BeginVertical();
			var viewType = (Grid.ViewType)EditorGUILayout.EnumPopup(
				"View", grid.m_viewType);
			if(viewType != grid.m_viewType)
			{
				grid.ChangeView(viewType);
			}
			switch(grid.m_viewType)
			{
			case Grid.ViewType.WalkableView:
				grid.m_brushCollision = (Cell.CollisionType)EditorGUILayout.EnumPopup(
					"Collision", grid.m_brushCollision);
				break;
			case Grid.ViewType.SecurityView:
				grid.m_brushSecurity = (Cell.SecurityType)EditorGUILayout.EnumPopup(
					"Security", grid.m_brushSecurity);
				break;
			case Grid.ViewType.MaterialView:
				grid.m_brushMaterial = (Cell.MaterialType)EditorGUILayout.EnumPopup(
					"Material", grid.m_brushMaterial);
				break;
            }
            GUILayout.EndVertical();
        }
        
        void OnSceneGUI()
        {
            if(Event.current.isMouse)
            {
                FocusMousePosition();
            }
            
            if(m_mouseRow >= 0 && m_mouseColumn >= 0)
			{
				// Draw brush.
				DrawBrush();
				// Paint color
				if(Event.current.type == EventType.MouseDown &&
				   Event.current.button == 0)
				{
					GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();

					Grid grid = target as Grid;
					switch(grid.m_viewType)
					{
					case Grid.ViewType.WalkableView:
						PaintWalkable();
						break;
					case Grid.ViewType.SecurityView:
						PaintSecurity();
						break;
					case Grid.ViewType.MaterialView:
						PaintMaterial();
						break;
					}
				}
			}
        }

		private void FocusMousePosition()
		{
			Grid grid = target as Grid;

			//cast a ray against mouse position
			Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hitInfo;
			var colliders = grid.GetComponentsInChildren<MeshCollider>();
			bool isCollision = false;
			foreach(var collider in colliders)
			{
				if(collider.Raycast(worldRay, out hitInfo, 100000.0f))
				{
					Vector3 offset = grid.transform.position;
					Vector3 pos = hitInfo.point;
					float fRow = (pos.z - offset.z) / grid.GridSize;
					float fColumn = (pos.x - offset.x) / grid.GridSize;
					var mRow = (int)fRow;
					var mColumn = (int)fColumn;
					if(mRow != m_mouseRow || mColumn != m_mouseColumn)
					{
						m_mouseRow = mRow;
						m_mouseColumn = mColumn;
						SceneView.RepaintAll();
					}
					isCollision = true;
					break;
				}
			}
			if(!isCollision && (m_mouseRow > 0 || m_mouseColumn > 0))
			{
				m_mouseRow = -1;
				m_mouseColumn = -1;
				SceneView.RepaintAll();
			}
		}

		private void DrawBrush()
		{
			Grid grid = target as Grid;

			var verts = new Vector3[4];
			switch(grid.m_brushType)
			{
			case Grid.BrushType.Rect1x1:
				grid.GetGridVerts(m_mouseRow, m_mouseColumn, verts);
				Handles.DrawSolidRectangleWithOutline(verts, BrushColor, Color.black);
				break;
			case Grid.BrushType.Rect3x3:
				DrawBrushRect(1, grid, verts);
				break;
			case Grid.BrushType.Rect5x5:
				DrawBrushRect(2, grid, verts);
				break;
			case Grid.BrushType.Rect7x7:
				DrawBrushRect(3, grid, verts);
				break;
			case Grid.BrushType.Rect9x9:
				DrawBrushRect(4, grid, verts);
				break;
			case Grid.BrushType.Cross3x3:
				DrawBrushCross(1, grid, verts);
				break;
			case Grid.BrushType.Cross5x5:
				DrawBrushCross(2, grid, verts);
				break;
			case Grid.BrushType.Cross7x7:
				DrawBrushCross(3, grid, verts);
				break;
			case Grid.BrushType.Cross9x9:
				DrawBrushCross(4, grid, verts);
				break;
			}
		}

		private void DrawBrushRect(int size, Grid grid, Vector3[] verts)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					if(grid.GetGridVerts(m_mouseRow + i, m_mouseColumn + j, verts))
					{
						Handles.DrawSolidRectangleWithOutline(verts, BrushColor, Color.black);
					}
				}
			}
		}

		private void DrawBrushCross(int size, Grid grid, Vector3[] verts)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					if(Mathf.Abs(i) + Mathf.Abs(j) > size)
					{
						continue;
					}
					
					if(grid.GetGridVerts(m_mouseRow + i, m_mouseColumn + j, verts))
					{
						Handles.DrawSolidRectangleWithOutline(verts, BrushColor, Color.black);
					}
				}
			}
		}

		private void PaintWalkable()
		{
			Grid grid = target as Grid;

			switch(grid.m_brushType)
			{
			case Grid.BrushType.Rect1x1:
				PaintWalkablePoint(m_mouseRow, m_mouseColumn, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Rect3x3:
				PaintWalkableBrushRect(1, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Rect5x5:
				PaintWalkableBrushRect(2, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Rect7x7:
				PaintWalkableBrushRect(3, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Rect9x9:
				PaintWalkableBrushRect(4, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Cross3x3:
				PaintWalkableBrushCross(1, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Cross5x5:
				PaintWalkableBrushCross(2, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Cross7x7:
				PaintWalkableBrushCross(3, grid, grid.m_brushCollision);
				break;
			case Grid.BrushType.Cross9x9:
				PaintWalkableBrushCross(4, grid, grid.m_brushCollision);
				break;
			}
		}

		private void PaintSecurity()
		{
			Grid grid = target as Grid;
			
			switch(grid.m_brushType)
			{
			case Grid.BrushType.Rect1x1:
				PaintSecurityPoint(m_mouseRow, m_mouseColumn, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Rect3x3:
				PaintSecurityBrushRect(1, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Rect5x5:
				PaintSecurityBrushRect(2, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Rect7x7:
				PaintSecurityBrushRect(3, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Rect9x9:
				PaintSecurityBrushRect(4, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Cross3x3:
				PaintSecurityBrushCross(1, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Cross5x5:
				PaintSecurityBrushCross(2, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Cross7x7:
				PaintSecurityBrushCross(3, grid, grid.m_brushSecurity);
				break;
			case Grid.BrushType.Cross9x9:
				PaintSecurityBrushCross(4, grid, grid.m_brushSecurity);
				break;
			}
		}

		private void PaintMaterial()
		{
			Grid grid = target as Grid;
			
			switch(grid.m_brushType)
			{
			case Grid.BrushType.Rect1x1:
				PaintMaterialPoint(m_mouseRow, m_mouseColumn, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Rect3x3:
				PaintMaterialBrushRect(1, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Rect5x5:
				PaintMaterialBrushRect(2, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Rect7x7:
				PaintMaterialBrushRect(3, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Rect9x9:
				PaintMaterialBrushRect(4, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Cross3x3:
				PaintMaterialBrushCross(1, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Cross5x5:
				PaintMaterialBrushCross(2, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Cross7x7:
				PaintMaterialBrushCross(3, grid, grid.m_brushMaterial);
				break;
			case Grid.BrushType.Cross9x9:
				PaintMaterialBrushCross(4, grid, grid.m_brushMaterial);
				break;
			}
		}

		private void PaintWalkablePoint(int i, int j, Grid grid, Cell.CollisionType collision)
		{
			grid.SetCellCollision(i, j, collision);
		}
		private void PaintWalkableBrushRect(int size, Grid grid, Cell.CollisionType collision)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					PaintWalkablePoint(m_mouseRow + i, m_mouseColumn + j, grid, collision);
				}
			}
		}
		private void PaintWalkableBrushCross(int size, Grid grid, Cell.CollisionType collision)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					if(Mathf.Abs(i) + Mathf.Abs(j) > size)
					{
						continue;
					}
					PaintWalkablePoint(m_mouseRow + i, m_mouseColumn + j, grid, collision);
				}
			}
		}

		private void PaintSecurityPoint(int i, int j, Grid grid, Cell.SecurityType security)
		{
			grid.SetCellSecurity(i, j, security);
		}
		private void PaintSecurityBrushRect(int size, Grid grid, Cell.SecurityType security)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					PaintSecurityPoint(m_mouseRow + i, m_mouseColumn + j, grid, security);
				}
			}
		}
		private void PaintSecurityBrushCross(int size, Grid grid, Cell.SecurityType security)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					if(Mathf.Abs(i) + Mathf.Abs(j) > size)
					{
						continue;
					}
					PaintSecurityPoint(m_mouseRow + i, m_mouseColumn + j, grid, security);
				}
			}
		}

		private void PaintMaterialPoint(int i, int j, Grid grid, Cell.MaterialType material)
		{
			grid.SetCellMaterial(i, j, material);
		}
		private void PaintMaterialBrushRect(int size, Grid grid, Cell.MaterialType material)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					PaintMaterialPoint(m_mouseRow + i, m_mouseColumn + j, grid, material);
				}
			}
		}
		private void PaintMaterialBrushCross(int size, Grid grid, Cell.MaterialType material)
		{
			for(int i = -size; i <= size; ++i)
			{
				for(int j = -size; j <= size; ++j)
				{
					if(Mathf.Abs(i) + Mathf.Abs(j) > size)
					{
						continue;
					}
					PaintMaterialPoint(m_mouseRow + i, m_mouseColumn + j, grid, material);
				}
			}
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

        