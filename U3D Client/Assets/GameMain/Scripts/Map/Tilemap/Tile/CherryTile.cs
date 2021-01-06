using UnityEngine;
using System;
using UnityEngine.Tilemaps;

namespace Cherry.Tilemaps
{
	[Serializable]
	[CreateAssetMenu]
	public class CherryTile : Tile
	{
		#region new
		[SerializeField]
		[HideInInspector]
		public bool IsNpc = false;
		[SerializeField]
		[HideInInspector]
		public string NpcName = "None";
		[SerializeField]
		[HideInInspector]
		public bool IsMonsterCreator = false;
		[SerializeField]
		[HideInInspector]
		public string MonsterName = "None";
		#endregion

		//[SerializeField]
		//public bool walkable = false;
		//[SerializeField]
		//public bool destroable = false;
		//[SerializeField]
		//public Sprite[] m_RandomSprites;
		//[SerializeField]
		//public Sprite[] m_AnimatedSprites;
		//[SerializeField]
		//public float m_MinSpeed = 1f;
		//[SerializeField]
		//public float m_MaxSpeed = 1f;
		//[SerializeField]
		//public float m_AnimationStartTime;

		//public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
		//{
		//	if (m_RandomSprites != null && m_RandomSprites.Length > 0 && m_AnimatedSprites != null && m_AnimatedSprites.Length > 0)
		//	{
		//		Debug.LogError("RandomSprites and AnimatedSprites can't exist at the same time");
		//		return;
		//	}
		//	base.GetTileData(location, tileMap, ref tileData);
		//	if (m_RandomSprites != null && m_RandomSprites.Length > 0)
		//	{
		//		long hash = location.x;
		//		hash = (hash + 0xabcd1234) + (hash << 15);
		//		hash = (hash + 0x0987efab) ^ (hash >> 11);
		//		hash ^= location.y;
		//		hash = (hash + 0x46ac12fd) + (hash << 7);
		//		hash = (hash + 0xbe9730af) ^ (hash << 11);
		//		UnityEngine.Random.InitState((int)hash);
		//		tileData.sprite = m_RandomSprites[(int)(m_RandomSprites.Length * UnityEngine.Random.value)];
		//	}
		//	if (m_AnimatedSprites != null && m_AnimatedSprites.Length > 0)
		//	{
		//		tileData.sprite = m_AnimatedSprites[m_AnimatedSprites.Length - 1];
		//	}
		//}

		//public override bool GetTileAnimationData(Vector3Int location, ITilemap tileMap, ref TileAnimationData tileAnimationData)
		//{
		//	if (m_AnimatedSprites != null && m_AnimatedSprites.Length > 0)
		//	{
		//		tileAnimationData.animatedSprites = m_AnimatedSprites;
		//		tileAnimationData.animationSpeed = UnityEngine.Random.Range(m_MinSpeed, m_MaxSpeed);
		//		tileAnimationData.animationStartTime = m_AnimationStartTime;
		//		return true;
		//	}
		//	return false;
		//}
	}
}