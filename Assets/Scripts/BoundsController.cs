using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class BoundsController
    {
        private Bounds _bounds;
        private Vector3 _min;
        private Vector3 _max;
        
        public BoundsController(Bounds bounds)
        {
            _bounds = bounds;
            _min = _bounds.min;
            _max = _bounds.max;
        }
        
        public bool IsOutOfBounds(Vector2 pos, float offset = 0)
        {
            return pos.x  < _min.x ||
                   pos.x  > _max.x ||
                   pos.y  < _min.z ||
                   pos.y  > _max.z;  
        }

        public Vector3 GetRndPos()
        {
            var offsetX = Random.Range(_min.x, _max.x);
            var offsetZ = Random.Range(_min.z, _max.z);

            return _bounds.center + new Vector3(offsetX, Variables.OffsetY, offsetZ);
        }
    }
}