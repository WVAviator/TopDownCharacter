using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace TopDownCharacter
{
    public static class AnimationSelector
    {

        public static ClipTransition MatchLateralMotion(List<ClipTransition> clipTransitions, Vector3 motion)
        {
            float closestDifference = float.PositiveInfinity;
            int closestDifferenceIndex = 0;

            for (int i = 0; i < clipTransitions.Count; i++)
            {
                float difference = (clipTransitions[i].Clip.averageSpeed.z - motion.z).Abs();
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