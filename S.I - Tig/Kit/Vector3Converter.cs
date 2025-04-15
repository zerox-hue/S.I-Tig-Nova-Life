using UnityEngine;
namespace Kit
{
    public class Vector3Converter
    {
        public static string ConvertToJson(Vector3 vector)
        {
            return $"{vector.x}/{vector.y}/{vector.z}";
        }
        public static Vector3 ConvertFromJson(string json)
        {
            string[] parts = json.Split('/');
            if (parts.Length != 3)
            {
                return Vector3.zero;
            }
            if (!float.TryParse(parts[0], out float x) || !float.TryParse(parts[1], out float y) || !float.TryParse(parts[2], out float z))
            {
                return Vector3.zero;
            }
            return new Vector3(x, y, z);
        }
    }
}