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

	public static class PoissonDiscSampling
	{
		public static List<Vector2> GeneratePoints(float unitAmount, float radius, Vector2 regionSize,
			List<GameObject> terrainGoList,
			int numSamplesBeforeRejection = 10)
		{
			var cellSize = radius / Mathf.Sqrt(2);

			var grid = new int[
				Mathf.CeilToInt(regionSize.x / cellSize),
				Mathf.CeilToInt(regionSize.y / cellSize)
			];
			var points = new List<Vector2>();
			var spawnPoints = new List<Vector2>();

			spawnPoints.Add(regionSize / 2);
			
			while (spawnPoints.Count <= unitAmount)
			{
				var spawnIndex = Random.Range(0, spawnPoints.Count);
				var spawnCentre = spawnPoints[spawnIndex];
				var candidateAccepted = false;

				for (var i = 0; i < numSamplesBeforeRejection; i++)
				{
					var angle = Random.value * Mathf.PI * 2;
					var dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
					var candidate = spawnCentre + dir * Random.Range(radius, 100 * radius);

					if (IsValid(candidate , terrainGoList, regionSize, cellSize, radius, points, grid))
					{
						points.Add(candidate);
						spawnPoints.Add(candidate);
						grid[(int) (candidate.x / cellSize), (int) (candidate.y / cellSize)] = points.Count;
						candidateAccepted = true;

						break;
					}
				}

				if (!candidateAccepted)
				{
					spawnPoints.RemoveAt(spawnIndex);
				}
			}

			return points;
		}

		static bool IsValid(
			Vector2 candidate, 
			List<GameObject> terrainGoList, 
			Vector2 sampleRegionSize, 
			float cellSize, 
			float radius,
			List<Vector2> points, 
			int[,] grid)
		{

		foreach (var terrain in terrainGoList)
		{
			var bounds = terrain.GetComponent<Collider>().bounds;

			var outOfTerrain = new BoundsController(bounds).IsOutOfBounds(candidate + Variables.CoordinatesOffset, radius);

			if (!outOfTerrain)
			{
				return false;
			}
		}

		if (candidate.x >= 0 && candidate.x < sampleRegionSize.x &&
		    candidate.y >= 0 && candidate.y < sampleRegionSize.y)
			{
				var cellX = (int) (candidate.x / cellSize);
				var cellY = (int) (candidate.y / cellSize);
				var searchStartX = Mathf.Max(0, cellX - 2);
				var searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
				var searchStartY = Mathf.Max(0, cellY - 2);
				var searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

				for (var x = searchStartX; x <= searchEndX; x++)
				{
					for (var y = searchStartY; y <= searchEndY; y++)
					{
						var pointIndex = grid[x, y] - 1;

						if (pointIndex != -1)
						{
							var sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
							if (sqrDst < radius * radius)
							{
								return false;
							}
						}
					}
				}

				return true;
			}

			return false;
		}
	}
}