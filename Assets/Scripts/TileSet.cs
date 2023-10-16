using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
///  Class that used to play animations for the set of tiles. And call the callback when all animations are finished.
/// </summary>
public class TileSet
{
    private TweenCallback _onComplete;
    private readonly List<AnimationData> _animationData = new ();
    private int _counter;

    public void Add(Tile tile, Vector2 moveTo, int timeMultiplier = 1)
    {
        _animationData.Add(new AnimationData()
        {
            Tile = tile,
            Time = timeMultiplier,
            MoveTo = moveTo
        });
    }

    public void Add(Tile tile, int timeMultiplier = 1)
    {
        _animationData.Add(new AnimationData()
        {
            Tile = tile,
            Time = timeMultiplier
        });
    }

    public void Add(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Add(tile);
        }
    }

    public void PlaySwitchAnimation(TweenCallback onComplete = null)
    {
        _onComplete = onComplete;

        foreach (AnimationData data in _animationData)
        {
            data.Tile.PlaySwitchAnimation(data.MoveTo, data.Time, OnAnimationComplete);
        }
    }
    
    public void PlayDestroyAnimation(TweenCallback onComplete = null)
    {
        _onComplete = onComplete;

        foreach (AnimationData data in _animationData)
        {
            data.Tile.PlayDestroyAnimation(data.Time, OnAnimationComplete);
        }
    }
    
    public void PlayFallAnimation(TweenCallback onComplete = null)
    {
        _onComplete = onComplete;

        foreach (AnimationData data in _animationData)
        {
            data.Tile.PlayFallAnimation(data.MoveTo, data.Time, OnAnimationComplete);
        }
    }
    
    /// <summary>
    ///  Callback for animation complete. It's used to count the number of completed animations. and if it's equal to the number of animations then call the Finish Callback.
    /// </summary>
    private void OnAnimationComplete()
    {
        _counter++;
        if(_counter == _animationData.Count)
        {
            _onComplete?.Invoke();
        }
    }
  
    /// <summary>
    /// Class that represents the animation data. It's used to store the data about the animation.
    /// </summary>
    private class AnimationData
    {
        public Tile Tile;
        public int Time;
        public Vector2 MoveTo;
    }


}