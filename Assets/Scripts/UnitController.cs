using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class UnitController : IOnUpdate
    {
        private float _timer;

        private List<BaseUnit> _hunters;
        private List<BaseUnit> _runners;
        private List<BaseUnit> _unit;


        public void DeleteUnits()
        {
            if (_unit.Count <= 0) return;

            foreach (var unit in _unit)
            {
                Object.DestroyImmediate(unit.gameObject);
            }
            
            _unit.Clear();
            RefreshUnitArrays();
        }

        public void OnUpdate()
        {
            BlueChangeDirIn(1, 3);
            RedHunt();
            BlueDangerTracking();
            UnitsMove();
        }

        public void UnitSwitchRole(BaseUnit unit)
        {
            unit.SwitchType();
            RefreshUnitArrays();
        }

        public void MassSwitchRoles()
        {
            foreach (var unit in _unit)
            {
                unit.SwitchType();
            }

            RefreshUnitArrays();
        }

        public void FillUnitArr(List<BaseUnit> unit)
        {
            _unit = unit;
            RefreshUnitArrays();
        }

        private void RefreshUnitArrays()
        {
            _hunters = _unit.Where(x => x.Type == UnitType.Red).ToList();
            _runners = _unit.Where(x => x.Type == UnitType.Blue).ToList();
        }

        private void BlueChangeDirIn(float min, float max)
        {
            if (_timer <= 0)
            {
                BlueGetRandomDir();
                _timer = Random.Range(min, max);
            }
            else
            {
                _timer -= Time.deltaTime;
            }
        }

        private void BlueGetRandomDir()
        {
            if (_runners.Count <= 0) return;

            foreach (var runner in _runners)
            {
                var rndVector = GetRndDir();
                runner.SetDir(rndVector);
            }
        }

        private Vector3 GetRndDir()
        {
            var rndDir = Random.insideUnitSphere;
            rndDir.y = 0;
            return rndDir;
        }

        private void BlueDangerTracking()
        {
            if (_runners.Count <= 0) return;

            foreach (var runner in _runners)
            {
                var runnerPos = runner.transform.position;
                var hunter = GetNearestTo(_hunters, runnerPos);

                if (!hunter) continue;

                var hunterPos = hunter.transform.position;

                var vector = hunterPos - runnerPos;
                var magnitude = vector.magnitude;

                if (magnitude < runner.Radius)
                {
                    runner.SetDir(-vector);
                }
            }
        }

        private void RedHunt()
        {
            if (_hunters.Count <= 0) return;

            foreach (var hunter in _hunters)
            {
                var runner = GetNearestTo(_runners, hunter.transform.position);
                
                if (runner)
                {
                    var runnerPos = runner.transform.position;
                    var hunterPos = hunter.transform.position;
                    
                    var vec = (runnerPos - hunterPos).normalized;
                    runnerPos.y = 0;

                    hunter.SetDir(vec);

                    Debug.DrawLine(hunterPos, runnerPos, Color.yellow);
                }
                else
                {
                    Debug.Log($"hunter= hunter {runner}");
                }
            }
        }

        private BaseUnit GetNearestTo(List<BaseUnit> arrFrom, Vector3 to)
        {
            if (arrFrom.Count <= 0) return null;

            var dist = float.MaxValue;

            BaseUnit unit = null;

            for (var i = 0; i < arrFrom.Count; i++)
            {
                var bestDistance = (arrFrom[i].transform.position - to).sqrMagnitude;

                if (bestDistance < dist)
                {
                    dist = bestDistance;
                    unit = arrFrom[i];
                }
            }

            return unit;
        }

        private void UnitsMove()
        {
            if (_unit.Count <= 0) return;

            foreach (var unit in _unit)
            {
                var direction = unit.CurMoveDirection * unit.Speed * Time.deltaTime;

                Vector3 dir;

                var pos = unit.transform.position;
                var tempPos = new Vector2(pos.x, pos.z);

                if (SceneManager.Instance.BoundsController.IsOutOfBounds(tempPos))
                {
                    dir = Vector3.zero;
                    unit.SetDir(dir);
                }
                else
                {
                    dir = direction;
                }
                unit.SetVel(dir);
            }
        }
    }

    public static class Variables
    {
        public const float OffsetY = 0.5f;
        public static Vector2 CoordinatesOffset = new Vector2(-50, -50);
    }
}