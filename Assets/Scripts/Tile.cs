using DG.Tweening;
using ScriptableObjects;
using UnityEngine;

/// <summary>
/// Component that represents the tile. It's used to change the color of the tile. Play animations.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private TimeForAnimations timeForAnimations;
    
     private Point _point;
     
     public void SetPoint(Point point)
     {
         _point = point;
     }
    

    public void PlaySwitchAnimation(Vector2 moveTo, int timeMultiplier = 1, TweenCallback onComplete = null)
    {
        float time = timeForAnimations.timeForSwitchAnimation;
        transform.DOMove(moveTo, time * timeMultiplier).onComplete += onComplete;
    }
    
    public void PlayDestroyAnimation(int timeMultiplier, TweenCallback onComplete = null)
    {
        float time =  timeForAnimations.timeForDestroyAnimation;
        transform.DOScale(Vector3.zero, time * timeMultiplier).onComplete += onComplete;
    }
    
    public void PlayFallAnimation(Vector2 moveTo, int timeMultiplier = 1, TweenCallback onComplete = null)
    {
        float time =  timeForAnimations.timeForFallAnimation;
        transform.DOMove(moveTo, time * timeMultiplier)
            .SetEase(Ease.Linear)
            .onComplete += onComplete;
    }

    
    public float GetDistanceToCompleteSwitch()
    {
        return gameObject.transform.localScale.x / 2;
    }

    public Point GetPoint()
    {
        return _point;
    }
}