using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace TopDownCharacter
{
    public static class AnimationSelector
    {

        public static ClipTransition MatchMotionMagnitude(List<ClipTransition> clipTransitions, Vector3 motion)
        {
            float motionMagnitude = motion.sqrMagnitude;
            
            float closestDifference = float.PositiveInfinity;
            int closestDifferenceIndex = 0;

            for (int i = 0; i < clipTransitions.Count; i++)
            {
                float difference = (clipTransitions[i].Clip.averageSpeed.sqrMagnitude - motionMagnitude).Abs();
                if (difference < closestDifference)
                {
                    closestDifference = difference;
                    closestDifferenceIndex = i;
                }
            }

            return clipTransitions[closestDifferenceIndex];
        }

        public static ClipTransition RandomClipTransition(List<ClipTransition> clipTransitions)
        {
            return clipTransitions[Random.Range(0, clipTransitions.Count)];
        }
        
    }
}