namespace UnityEngine
{
    public static class Extensions
    {
        public static Vector3 DirectionTo(this Transform transform, Vector3 target)
        {
            return (target - transform.position).normalized;
        }

        public static string RemoveBlankCharacters(this string value)
        {
            value = value.Replace(" ", "");
            value = value.Replace("\n", "");
            value = value.Replace("\t", "");

            return value;
        }
    }
}
