using UnityEngine;
using System.Collections;

namespace Nirvana.Scene
{
	/// <summary>
	/// The scene grid is used for collision detect, path finding
	/// and region query.
	/// </summary>
	[ExecuteInEditMode]
	public class Grid : MonoBehaviour
	{
        //grid的size必须是这个数字的整数倍
        public const int GridSizeDelta = 32;
		// Color for grid.
		static public Color32 ColorWalkable   = new Color32(0,  128, 0, 128);
		static public Color32 ColorUnwalkable = new Color32(128,  0, 0, 128);

		static public Color32 ColorSafe   = new Color32(  0,   0, 128, 128);
		static public Color32 ColorBattle = new Color32(128, 128,   0, 128);
		static public Color32 ColorPVP    = new Color32(128,   0,   0, 128);

		static public Color32 ColorWood   = new Color32(115,  74,  18, 128);
		static public Color32 ColorMetal  = new Color32(255, 235, 205, 128);
		static public Color32 ColorBrick  = new Color32(128,  42,  42, 128);
		static public Color32 ColorCobble = new Color32(245, 245, 245, 128);
		static public Color32 ColorDirty  = new Color32(  8,  64,  84, 128);
		static public Color32 ColorSand   = new Color32(227, 207,  87, 128);
		static public Color32 ColorLeaf   = new Color32( 61, 145,  64, 128);
		static public Color32 ColorGrass  = new Color32(127, 255,   0, 128);
		static public Color32 ColorSnow   = new Color32(255, 250, 240, 128);
		static public Color32 ColorWater  = new Color32( 65, 105, 225, 128);

		// Used for GridEditor.
		public int m_keepRow;
		public int m_keepColumn;
		public float m_lowestHigh = 0.0f;
		public Mesh[] m_meshs;
		public Color32[][] m_colors;

		// The brush type.
		public enum BrushType
		{
			Rect1x1,
			Rect3x3,
			Rect5x5,
			Rect7x7,
			Rect9x9,
			Cross3x3,
			Cross5x5,
			Cross7x7,
			Cross9x9,
		}
		public BrushType m_brushType = BrushType.Rect1x1;
		// The view type.
		public enum ViewType
		{
			WalkableView,
			SecurityView,
			MaterialView,
		}
		public ViewType m_viewType = ViewType.WalkableView;

		// The brush paint.
		public Cell.CollisionType m_brushCollision = Cell.CollisionType.Unwalkable;
		public Cell.SecurityType m_brushSecurity = Cell.SecurityType.SafeRegion;
		public Cell.MaterialType m_brushMaterial = Cell.MaterialType.Wood;

		// The row count of the grid.
		public int Row
		{
			get {return m_row;}
		}
		// The column count of the grid.
		public int Column
		{
			get {return m_column;}
		}
		// The grid cell size.
		public float GridSize;

		// The cell data in array.
		[SerializeField]
		private Cell[] m_cells;
		[SerializeField]
		private int m_row;
		[SerializeField]
		private int m_column;

		void Awake()
		{
			if((m_cells == null || m_cells.Length == 0) && m_row > 0 && m_column > 0)
			{
				m_cells = new Cell[m_row * m_column];
			}
			UpdateMesh();
		}

		/// <summary>
		/// Resize the grid with specified row and column.
		/// </summary>
		public void Resize(int row, int column,bool restore = true)
		{
			var cells = new Cell[row * column];
			//init the cells
			for(int i = 0; i < row; ++i)
			{
				for(int j = 0; j < column; ++j)
				{
					cells[i * column + j].collision = Cell.CollisionType.Unwalkable;
					cells[i * column + j].material = Cell.MaterialType.Sand;
				}
			}
			// Keep the old data.
			if(m_cells != null && m_row > 0)
			{
				if(restore)
				{
					var copyRow = Mathf.Min(m_row, row);
					var copyColumn = Mathf.Min(m_column, column);
					for(int i = 0; i < copyRow; ++i)
					{
						for(int j = 0; j < copyColumn; ++j)
						{
							cells[i * column + j] = m_cells[i * m_column + j];
						}
					}
				}	
			}
			m_cells = cells;
			m_row = row;
			m_column = column;

			UpdateMesh();
		}

		/// <summary>
		/// Fit each cell to the ground.
		/// </summary>
		public void FitToGround()
		{
			for(int i = 0; i < m_row; ++i)
			{
				for(int j = 0; j < m_column; ++j)
				{
					var pos = transform.position;
					var origin = new Vector3(
						j * GridSize + pos.x, 10000.0f + pos.y, i * GridSize + pos.z);

					var highest = -100000.0f;
					var hits = Physics.RaycastAll(origin, Vector3.down, 100000.0f,1<<LayerMask.NameToLayer("Ground"));
					if(hits.Length == 0)
					{
						m_cells[i * m_column + j].high = 0.0f;
					}
					else
					{
						foreach(var hit in hits)
						{
							if(highest < hit.point.y)
							{
								highest = hit.point.y;
							}
						}
						m_cells[i * m_column + j].high = highest + 0.2f;
					}
				}
			}
			UpdateMesh();
		}

		/// Set the cell collision.
		public void SetCellCollision(int row, int column, Cell.CollisionType collision)
		{
			m_cells[row * m_column + column].collision = collision;
			if(m_viewType == ViewType.WalkableView)
			{
				SetGridColor(row, column, GetCollisionColor(collision));
			}
		}

		/// Set the cell security.
		public void SetCellSecurity(int row, int column, Cell.SecurityType security)
		{
			m_cells[row * m_column + column].security = security;
			if(m_viewType == ViewType.SecurityView)
			{
				SetGridColor(row, column, GetSecurityColor(security));
			}
		}

		/// Set the cell material.
		public void SetCellMaterial(int row, int column, Cell.MaterialType material)
		{
			m_cells[row * m_column + column].material = material;
			if(m_viewType == ViewType.MaterialView)
			{
				SetGridColor(row, column, GetMaterialColor(material));
			}
		}

		/// Access cell data at specify position.
		public Cell GetCell(int row, int column)
		{
			return m_cells[row * m_column + column];
		}

		/// Get the cell position.
		public Vector2 GetCellPosition(int row, int column)
		{
			return new Vector2(row * GridSize, column * GridSize);
		}

		/// Get the cell world position.
		public Vector3 GetCellWorldPosition(int row, int column)
		{
			var pos = transform.position;
			var cell = m_cells[row * m_column + column];
			return new Vector3(column * GridSize + pos.x, cell.high, row * GridSize + pos.z);
		}

		/// Get the cell local position.
		public Vector3 GetCellLocalPosition(int row, int column)
		{
			var cell = m_cells[row * m_column + column];
			return new Vector3(column * GridSize, cell.high - transform.position.y, row * GridSize);
		}

		/// Get the cell world position.
		public Vector3 GetCellWorldPositionSafe(int row, int column)
		{
			if(row < 0)
			{
				row = 0;
			}
			if(row >= m_row)
			{
				row = m_row - 1;
			}

			if(column < 0)
			{
				column = 0;
			}
			if(column >= m_column)
            {
				column = m_column - 1;
            }

			return GetCellWorldPosition(row, column);
		}

		/// Get the cell local position.
		public Vector3 GetCellLocalPositionSafe(int row, int column)
		{
			if(row < 0)
			{
				row = 0;
			}
			if(row >= m_row)
			{
				row = m_row - 1;
			}
			
			if(column < 0)
			{
				column = 0;
			}
			if(column >= m_column)
			{
				column = m_column - 1;
			}
			
			return GetCellLocalPosition(row, column);
		}

		/// Get the grid verts.
		public bool GetGridVerts(int i, int j, Vector3[] verts)
		{
			if(i < 0 || i > m_row || j < 0 || j > m_column)
			{
				return false;
			}

			var cellPos = GetCellWorldPosition(i, j);
			var cellB   = GetCellWorldPositionSafe(i,   j+1);
			var cellR   = GetCellWorldPositionSafe(i+1, j);
			var cellRB  = GetCellWorldPositionSafe(i+1, j+1);
			
			verts[0] = new Vector3(cellPos.x,            cellPos.y, cellPos.z);
			verts[1] = new Vector3(cellPos.x,              cellR.y, cellPos.z + GridSize);
			verts[2] = new Vector3(cellPos.x + GridSize,  cellRB.y, cellPos.z + GridSize);
			verts[3] = new Vector3(cellPos.x + GridSize,   cellB.y, cellPos.z);

			return true;
        }

		/// Change the grid view.
		public void ChangeView(ViewType viewType)
		{
			if(m_viewType == viewType)
			{
				return;
			}
			m_viewType = viewType;

			int segmentX = Mathf.CeilToInt(m_row / (float)GridSizeDelta);
			int segmentY = Mathf.CeilToInt(m_column / (float)GridSizeDelta);
			switch(m_viewType)
			{
			case ViewType.WalkableView:
				for(int i = 0; i < segmentX; ++i)
				{
					for(int j = 0; j < segmentY; ++j)
					{
						ChangeToWalkableViewSegment(i, j, m_colors[i * segmentY + j], m_meshs[i * segmentY + j]);
					}
				}
				break;
			case ViewType.SecurityView:
				for(int i = 0; i < segmentX; ++i)
				{
					for(int j = 0; j < segmentY; ++j)
					{
						ChangeToSecurityViewSegment(i, j, m_colors[i * segmentY + j], m_meshs[i * segmentY + j]);
					}
				}
				break;
			case ViewType.MaterialView:
				for(int i = 0; i < segmentX; ++i)
				{
					for(int j = 0; j < segmentY; ++j)
					{
						ChangeToMaterialViewSegment(i, j, m_colors[i * segmentY + j], m_meshs[i * segmentY + j]);
					}
				}
				break;
			}
		}
        
        /// Update the mesh.
		private void UpdateMesh()
		{
			// Remove all children.
			var children = new Transform[transform.childCount];
			for(int i = 0; i < transform.childCount; ++i)
			{
				children[i] = transform.GetChild(i);
			}
			foreach(var child in children)
			{
				DestroyImmediate(child.gameObject);
			}

			// Divide each submesh by 64 x 64.
			int segmentX = Mathf.CeilToInt(m_row / (float)GridSizeDelta);
			int segmentY = Mathf.CeilToInt(m_column / (float)GridSizeDelta);
			m_colors = new Color32[segmentX * segmentY][];
			m_meshs = new Mesh[segmentX * segmentY];
			for(int i = 0; i < segmentX; ++i)
			{
				for(int j = 0; j < segmentY; ++j)
				{
					UpdateSubmeshCell(i, j, 
						out m_colors[i * segmentY + j], 
						out m_meshs[i * segmentY + j]);
					UpdateSubmeshOutline(i, j);
				}
			}
		}

		private void UpdateSubmeshCell(int segX, int segY, 
			out Color32[] colors, out Mesh mesh)
		{
			// Create segment gameobject.
			var go = new GameObject();
			go.name = "GridCell: " + segX + "-" + segY;
			go.tag = "EditorOnly";
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

			var renderer = go.AddComponent<MeshRenderer>();
			var collider = go.AddComponent<MeshCollider>();
			var filter = go.AddComponent<MeshFilter>();

			// Create mesh for it.
			mesh = new Mesh();
			mesh.name = "GridCell: " + segX + "-" + segY;

			int startRow = GridSizeDelta * segX;
			int startCol = GridSizeDelta * segY;
			int endRow = Mathf.Min(GridSizeDelta * segX + GridSizeDelta, m_row);
			int endCol = Mathf.Min(GridSizeDelta * segY + GridSizeDelta, m_row);
			int rowCount = endRow - startRow;
			int colCount = endCol - startCol;

			mesh.Clear();
			var verts = new Vector3[rowCount * colCount * 4];
			var vertIdx = 0;
			for(int i = startRow; i < endRow; ++i)
			{
				for(int j = startCol; j < endCol; ++j)
				{
					var cellPos = GetCellLocalPosition(i, j);
					var cellB   = GetCellLocalPositionSafe(i,   j+1);
					var cellR   = GetCellLocalPositionSafe(i+1, j);
					var cellRB  = GetCellLocalPositionSafe(i+1, j+1);

					verts[vertIdx + 0] = new Vector3(cellPos.x,            cellPos.y, cellPos.z);
					verts[vertIdx + 1] = new Vector3(cellPos.x,              cellR.y, cellPos.z + GridSize);
					verts[vertIdx + 2] = new Vector3(cellPos.x + GridSize,  cellRB.y, cellPos.z + GridSize);
					verts[vertIdx + 3] = new Vector3(cellPos.x + GridSize,   cellB.y, cellPos.z);
					vertIdx += 4;
				}
			}
			mesh.vertices = verts;
			
			var tris = new int[6 * rowCount * colCount];
			for(int i = 0; i < rowCount * colCount; ++i)
			{
				tris[6 * i + 0] = 4 * i + 0;
				tris[6 * i + 1] = 4 * i + 1;
				tris[6 * i + 2] = 4 * i + 2;
				
				tris[6 * i + 3] = 4 * i + 2;
				tris[6 * i + 4] = 4 * i + 3;
				tris[6 * i + 5] = 4 * i + 0;
			}
			mesh.triangles = tris;

			colors = new Color32[rowCount * colCount * 4];
			var colorIdx = 0;
			for(int i = startRow; i < endRow; ++i)
			{
				for(int j = startCol; j < endCol; ++j)
				{
					var cell = GetCell(i, j);
					var color = GetCollisionColor(cell.collision);

					colors[colorIdx + 0] = color;
					colors[colorIdx + 1] = color;
					colors[colorIdx + 2] = color;
					colors[colorIdx + 3] = color;

					colorIdx += 4;
				}
			}
			mesh.colors32 = colors;

			mesh.RecalculateBounds();

			collider.sharedMesh = mesh;
			filter.sharedMesh = mesh;

			renderer.material = new Material(Shader.Find("Nirvana/Editor/GridCell"));
		}

		private void UpdateSubmeshOutline(int segX, int segY)
		{
			// Create segment gameobject.
			var go = new GameObject();
			go.name = "GridOutline: " + segX + "-" + segY;
			go.tag = "EditorOnly";
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			
			var renderer = go.AddComponent<MeshRenderer>();
			var filter = go.AddComponent<MeshFilter>();
			
			// Create mesh for it.
			var mesh = new Mesh();
			mesh.name = "GridOutline: " + segX + "-" + segY;
			
			int startRow = GridSizeDelta * segX;
			int startCol = GridSizeDelta * segY;
			int endRow = Mathf.Min(GridSizeDelta * segX + GridSizeDelta, m_row);
			int endCol = Mathf.Min(GridSizeDelta * segY + GridSizeDelta, m_row);
			int rowCount = endRow - startRow;
			int colCount = endCol - startCol;
			
			mesh.Clear();
			var verts = new Vector3[rowCount * colCount * 4];
			var vertIdx = 0;
			for(int i = startRow; i < endRow; ++i)
			{
				for(int j = startCol; j < endCol; ++j)
				{
					var cellPos = GetCellLocalPosition(i, j);
					var cellB   = GetCellLocalPositionSafe(i,   j+1);
					var cellR   = GetCellLocalPositionSafe(i+1, j);
					var cellRB  = GetCellLocalPositionSafe(i+1, j+1);
					
					verts[vertIdx + 0] = new Vector3(cellPos.x,            cellPos.y, cellPos.z);
					verts[vertIdx + 1] = new Vector3(cellPos.x,              cellR.y, cellPos.z + GridSize);
					verts[vertIdx + 2] = new Vector3(cellPos.x + GridSize,  cellRB.y, cellPos.z + GridSize);
					verts[vertIdx + 3] = new Vector3(cellPos.x + GridSize,   cellB.y, cellPos.z);
					vertIdx += 4;
				}
			}
			mesh.vertices = verts;
			
			var lines = new int[8 * rowCount * colCount];
			for(int i = 0; i < rowCount * colCount; ++i)
			{
				lines[8 * i + 0] = 4 * i + 0;
				lines[8 * i + 1] = 4 * i + 1;
				
				lines[8 * i + 2] = 4 * i + 1;
				lines[8 * i + 3] = 4 * i + 2;
				
				lines[8 * i + 4] = 4 * i + 2;
				lines[8 * i + 5] = 4 * i + 3;
				
				lines[8 * i + 6] = 4 * i + 3;
				lines[8 * i + 7] = 4 * i + 0;
			}
			mesh.SetIndices(lines, MeshTopology.Lines, 0);
			
			mesh.RecalculateBounds();

			filter.sharedMesh = mesh;
			
			renderer.material = new Material(Shader.Find("Nirvana/Editor/GridOutline"));
		}

		// Set the color for specify grid.
		private void SetGridColor(int row, int column, Color32 color)
		{
			int segmentX = Mathf.CeilToInt(m_row / (float)GridSizeDelta);
			int segmentY = Mathf.CeilToInt(m_column / (float)GridSizeDelta);
			int segX = Mathf.FloorToInt(row / (float)GridSizeDelta);
			int segY = Mathf.FloorToInt(column / (float)GridSizeDelta);
			
			var c = m_colors[segX * segmentY + segY];
			var m = m_meshs[segX * segmentY + segY];
			int i = row - GridSizeDelta * segX;
			int j = column - GridSizeDelta * segY;
			
			c[4 * (i * GridSizeDelta + j) + 0] = color;
			c[4 * (i * GridSizeDelta + j) + 1] = color;
			c[4 * (i * GridSizeDelta + j) + 2] = color;
			c[4 * (i * GridSizeDelta + j) + 3] = color;
			
			m.colors32 = c;
		}

		private void ChangeToWalkableViewSegment(
			int segX, int segY, Color32[] colors, Mesh mesh)
		{
			int startRow = GridSizeDelta * segX;
			int startCol = GridSizeDelta * segY;
			int endRow = Mathf.Min(GridSizeDelta * segX + GridSizeDelta, m_row);
			int endCol = Mathf.Min(GridSizeDelta * segY + GridSizeDelta, m_row);

			var colorIdx = 0;
			for(int i = startRow; i < endRow; ++i)
			{
				for(int j = startCol; j < endCol; ++j)
				{
					var cell = GetCell(i, j);
					var color = GetCollisionColor(cell.collision);
					
					colors[colorIdx + 0] = color;
					colors[colorIdx + 1] = color;
					colors[colorIdx + 2] = color;
					colors[colorIdx + 3] = color;
					
					colorIdx += 4;
				}
			}
			mesh.colors32 = colors;
		}

		private void ChangeToSecurityViewSegment(
			int segX, int segY, Color32[] colors, Mesh mesh)
		{
			int startRow = GridSizeDelta * segX;
			int startCol = GridSizeDelta * segY;
			int endRow = Mathf.Min(GridSizeDelta * segX + GridSizeDelta, m_row);
			int endCol = Mathf.Min(GridSizeDelta * segY + GridSizeDelta, m_row);
			
			var colorIdx = 0;
			for(int i = startRow; i < endRow; ++i)
			{
				for(int j = startCol; j < endCol; ++j)
				{
					var cell = GetCell(i, j);
					var color = GetSecurityColor(cell.security);
					
					colors[colorIdx + 0] = color;
					colors[colorIdx + 1] = color;
					colors[colorIdx + 2] = color;
					colors[colorIdx + 3] = color;
					
					colorIdx += 4;
				}
			}
			mesh.colors32 = colors;
		}

		private void ChangeToMaterialViewSegment(
			int segX, int segY, Color32[] colors, Mesh mesh)
		{
			int startRow = GridSizeDelta * segX;
			int startCol = GridSizeDelta * segY;
			int endRow = Mathf.Min(GridSizeDelta * segX + GridSizeDelta, m_row);
			int endCol = Mathf.Min(GridSizeDelta * segY + GridSizeDelta, m_row);
			
			var colorIdx = 0;
			for(int i = startRow; i < endRow; ++i)
			{
				for(int j = startCol; j < endCol; ++j)
				{
					var cell = GetCell(i, j);
					var color = GetMaterialColor(cell.material);

					colors[colorIdx + 0] = color;
					colors[colorIdx + 1] = color;
					colors[colorIdx + 2] = color;
					colors[colorIdx + 3] = color;

					colorIdx += 4;
				}
			}
			mesh.colors32 = colors;
		}

		private Color32 GetCollisionColor(Cell.CollisionType collision)
		{
			switch(collision)
			{
			case Cell.CollisionType.Walkable:
				return ColorWalkable;
			case Cell.CollisionType.Unwalkable:
				return ColorUnwalkable;
			}
			return new Color32(0, 0, 0, 1);
		}

		private Color32 GetSecurityColor(Cell.SecurityType security)
		{
			switch(security)
			{
			case Cell.SecurityType.SafeRegion:
				return ColorSafe;
			case Cell.SecurityType.BattleRegion:
				return ColorBattle;
			case Cell.SecurityType.PVPRegion:
				return ColorPVP;
			}
			return new Color32(0, 0, 0, 1);
		}

		private Color32 GetMaterialColor(Cell.MaterialType material)
		{
			switch(material)
			{
			case Cell.MaterialType.Wood:
				return ColorWood;
			case Cell.MaterialType.Metal:
				return ColorMetal;
			case Cell.MaterialType.Brick:
				return ColorBrick;
			case Cell.MaterialType.Cobble:
				return ColorCobble;
			case Cell.MaterialType.Dirty:
				return ColorDirty;
			case Cell.MaterialType.Sand:
				return ColorSand;
			case Cell.MaterialType.Leaf:
				return ColorLeaf;
			case Cell.MaterialType.Grass:
				return ColorGrass;
			case Cell.MaterialType.Snow:
				return ColorSnow;
			case Cell.MaterialType.Water:
				return ColorWater;
			}
			return new Color32(0, 0, 0, 1);
		}
    }
}
