using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    ///  Scriptable object for available colors. It's used to determine the colors of the tiles.
    /// </summary>
    [CreateAssetMenu(fileName = "AvailableColors", menuName = "ScriptableObjects/AvailableColors")]
    public class AvailableColorsSO : ScriptableObject
    {
        public Color[] colors;
    }
}
