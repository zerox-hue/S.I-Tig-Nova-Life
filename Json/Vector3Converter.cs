using UnityEngine;

namespace Json
{
    public static class Vector3Converters
    {
        public static string ConvertToJson(Vector3 vector)
        {
            if(vector == null)
            {
                return "";
            }
            string json = $"{vector.x}/{vector.y}/{vector.z}";
            return json;
        }
        public static Vector3 ConvertFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return Vector3.zero;
            }
            string[] values = json.Split('/');
            if (values.Length != 3)
            {
                return Vector3.zero;
            }
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = float.Parse(values[2]);
            return new Vector3(x, y, z);
        }
    }
}
