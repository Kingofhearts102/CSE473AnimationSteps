using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaAux;
using Microsoft.Xna.Framework;

namespace AnimationSteps
{
    public class AnimationPlayer
    {
        private AnimationClips.Clip clip;
        private bool looping = false;
        private double speed = 1.0f;
        private double time = 0;

        public AnimationPlayer(AnimationClips.Clip clip)
        {
            this.clip = clip;
        }

        private BoneInfo[] boneInfos;
        private int boneCnt;

        public int BoneCount { get { return boneCnt; } }
        public Bone GetBone(int b) { return boneInfos[b]; }

        public void Initialize()
        {
            boneCnt = clip.Keyframes.Length;
            boneInfos = new BoneInfo[boneCnt];

            time = 0;
            for (int b = 0; b < boneCnt; b++)
            {
                boneInfos[b].CurrentKeyframe = -1;
                boneInfos[b].Valid = false;
            }
        }

        public void Update(double delta)
        {
            time += delta;

            for (int b = 0; b < boneInfos.Length; b++)
            {
                List<AnimationClips.Keyframe> keyframes = clip.Keyframes[b];
                if (keyframes.Count == 0)
                    continue;

                // The time needs to be greater than or equal to the
                // current keyframe time and less than the next keyframe 
                // time.
                while (boneInfos[b].CurrentKeyframe < 0 ||
                    (boneInfos[b].CurrentKeyframe < keyframes.Count - 1 &&
                    keyframes[boneInfos[b].CurrentKeyframe + 1].Time <= time))
                {
                    // Advance to the next keyframe
                    boneInfos[b].CurrentKeyframe++;
                }

                //
                // Update the bone
                //

                int c = boneInfos[b].CurrentKeyframe;
                if (c >= 0)
                {
                    AnimationClips.Keyframe keyframe = keyframes[c];
                    boneInfos[b].Valid = true;
                    boneInfos[b].Rotation = keyframe.Rotation;
                    boneInfos[b].Translation = keyframe.Translastion;
                }
            }
        }    

        private struct BoneInfo : Bone
        {
            private int currentKeyframe;
            private bool valid;

            private Quaternion rotation;
            private Vector3 translation;

            public int CurrentKeyframe { get { return currentKeyframe; } set { currentKeyframe = value; } }
            public bool Valid { get { return valid; } set { valid = value; } }
            public Quaternion Rotation { get { return rotation; } set { rotation = value; } }
            public Vector3 Translation { get { return translation; } set { translation = value; } }
        }

        public interface Bone
        {
            bool Valid { get; }
            Quaternion Rotation { get; }
            Vector3 Translation { get; }
        }

        public bool Looping
        {
            get { return looping; }
            set { looping = value; }
        }
        public double Speed
        {
            get { return speed; }
            set { speed = value; }
        }

    } // end class
}
