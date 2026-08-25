using UnityEngine;

namespace Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 Range(this Vector3 v, Vector3 from, Vector3 to)
        {
            return new Vector3(Random.Range(from.x, to.x), Random.Range(from.y, to.y), Random.Range(from.z, to.z));
        }
    }
}