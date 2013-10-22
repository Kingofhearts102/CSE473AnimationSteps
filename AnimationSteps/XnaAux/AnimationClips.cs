using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaAux
{
   public class AnimationClips
    {

       public class Keyframe
       {
           public double Time; //the keyframe time
           public Quaternion Rotation; // the fortation for the bone
           public Vector3 Translastion; // translation for the bone
       }

       public class Clip
       {
           /// <summary>
           /// Name of animation clip
           /// </summary>
           public string Name;
           /// <summary>
           /// duration of the animation clip
           /// </summary>
           public double Duration;
           /// <summary>
           /// The keyframes in the animation. We have an array of bones each with a list of keyframes
           /// </summary>
           public List<Keyframe>[] Keyframes;

           //private bool looping = false;
           //private double speed = 1.0f;
           //private double time = 0;



          /* private struct BoneInfo : Bone
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
           */


           //public int BoneCount { get { return boneCnt; } }
           //public Bone GetBone(int b) { return boneInfos[b]; }

          /* public void Initialize()
           {
               boneCnt = Keyframes.Length;
               boneInfos = new BoneInfo[boneCnt];

               time = 0;
               for(int b = 0;b <boneCnt;b++)
               {
                   boneInfos[b].CurrentKeyframe = -1;
                   boneInfos[b].Valid = false;
               }
           }*/

           /// <summary>
           /// Update the clip position
           /// </summary>
           /// <param name="delta">The amount of time that has passed.</param>
           
           

       }



       public Dictionary<string, Clip> Clips = new Dictionary<string, Clip>();
    }
}
