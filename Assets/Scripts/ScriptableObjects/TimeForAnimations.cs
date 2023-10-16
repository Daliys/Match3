using UnityEngine;

namespace ScriptableObjects
{
    /// <summary>
    ///  Scriptable object for time for animations. It's used to determine the time for animations.
    /// </summary>
    [ CreateAssetMenu(fileName = "TimeForAnimations", menuName = "ScriptableObjects/TimeForAnimations")]
    public class TimeForAnimations : ScriptableObject
    {
        public float timeForSwitchAnimation;
        public float timeForDestroyAnimation;
        public float timeForFallAnimation;
    }
}