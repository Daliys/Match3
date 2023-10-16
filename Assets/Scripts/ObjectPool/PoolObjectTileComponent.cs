using System;

namespace ObjectPool
{
    /// <summary>
    ///  Class that adding as component to the game object that will be used in object pooling.
    /// </summary>
    [Serializable]
    public class PoolObjectTileComponent : PoolObjectType
    {
        public TileType type;
    }
}