using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationClipHolder", menuName = "Create Animation Holder")]
public class AnimationHolder : ScriptableObject
{
    public List<AnimationClip> animationClips;

    public AnimationClip ReturnAnimationOverride(string animationId)
    {
        AnimationClip chosenOverrideController = null;
        foreach (AnimationClip clip in animationClips)
        {
            if (clip.animationName == animationId)
            {
                chosenOverrideController = clip;
                break;
            }
        }
        return chosenOverrideController;
    }

    public int ReturnSelectedAnimationClipIndex(string animationId)
    {
        int index = 0;
        for (int i = 0; i < animationClips.Count; i++)
        {
            if (animationClips[i].animationName == animationId)
            {
                index = i;
            }
        }
        return index;
    }

}

[System.Serializable]
public class AnimationClip
{
    public string animationName;
    public AnimatorOverrideController animationOverridePerClip;
    public float speed;
    public bool turnModelTo90Deg;
    public string clipComment = "NA";
}
