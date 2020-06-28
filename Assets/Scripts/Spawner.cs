using System.Collections.Generic;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class Spawner : MonoBehaviour
	{
		[SerializeField] private BaseUnit _unitPrefab;
		[SerializeField] private GameObject _terrainPrefab;

		private readonly List<GameObject> _terrainsList = new List<GameObject>();
		private readonly List<BaseUnit> _unitsList = new List<BaseUnit>();

		public void Spawn(float unitsAmount, float terrainAmount, float radius, float speed)
		{
			var region = new Vector2(100, 100);

			for (var i = 0; i < terrainAmount; i++)
			{
				var terrainGo = TerrainSpawn(_terrainPrefab);
				_terrainsList.Add(terrainGo);
			}

			var vec = PoissonDiscSampling.GeneratePoints(unitsAmount, 0.5f, region, _terrainsList);
			var curVec = new List<Vector2>();

			for (var i = 0; i < unitsAmount; i++)
			{
				var range = Random.Range(0, vec.Count);
				curVec.Add(vec[range]);
				vec.RemoveAt(range);
			}

			for (var i = 0; i < unitsAmount; i++)
			{
				var unitGo = UnitSpawn(curVec[i] + Variables.CoordinatesOffset, _unitPrefab, radius, speed);
				_unitsList.Add(unitGo);
			}

			SceneManager.Instance.UnitController.FillUnitArr(_unitsList);
		}

		private GameObject TerrainSpawn(GameObject terrainPrefab)
		{
			return Instantiate(
				terrainPrefab,
				SceneManager.Instance.BoundsController.GetRndPos(),
				Quaternion.Euler(0, Random.Range(0, 360), 0)
			);
		}

		private BaseUnit UnitSpawn(Vector2 vec, BaseUnit unitPrefab, float radius, float speed)
		{
			var unit = Instantiate(
				unitPrefab,
				new Vector3(vec.x, Variables.OffsetY, vec.y),
				Quaternion.identity
			);
			
			var type = Random.value > 0.5f ? UnitType.Red : UnitType.Blue;
			unit.Init(radius, speed, type);
			
			return unit;
		}
	}
}