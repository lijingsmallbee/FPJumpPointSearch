using UnityEngine;
using System.Collections;

namespace Nirvana.Scene
{
	/// <summary>
	/// It represent a cell data in a scene grid.
	/// </summary>
	[System.Serializable]
	public struct Cell
	{
		/// The left top point high of this cell.
		public float high;
		public enum CollisionType
		{
			Walkable,
			Unwalkable,
		}
		public CollisionType collision;
		/// The safe type of this region.
		public enum SecurityType
		{
			SafeRegion,
			BattleRegion,
			PVPRegion,
		}
		public SecurityType security;
		/// The cell material, used for sound and movement.
		public enum MaterialType
		{
			Wood   = 0,
			Metal  = 1,
			Brick  = 2,
			Cobble = 3,
			Dirty  = 4,
			Sand   = 5,
			Leaf   = 6,
			Grass  = 7,
			Snow   = 8,
			Water  = 9,
		}
		public MaterialType material;
	}
}
