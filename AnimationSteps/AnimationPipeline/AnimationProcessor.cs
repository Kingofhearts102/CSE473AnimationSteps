using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using XnaAux;

// TODO: replace these with the processor input and output types.


namespace AnimationPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Animation Processor")]
    public class AnimationProcesssor : ModelProcessor
    {
        ModelContent model;
        /// <summary>
        /// Bones lookup table, converts bone names to indices
        /// </summary>
        private Dictionary<string, int> bones = new Dictionary<string, int>();

        private AnimationClips ProcessAnimations(ModelContent model, NodeContent input, ContentProcessorContext context)
        {
            //first build a lookup table so we can determine the index into the list of bones from a bone name
            for (int i = 0; i < model.Bones.Count; i++)
            {
                bones[model.Bones[i].Name] = i;
            }

            AnimationClips animationClips = new AnimationClips();
            ProcessAnimationRecursive(input, animationClips);
            return animationClips;
        }

        private void ProcessAnimationRecursive(NodeContent input, AnimationClips animationClips)
        {
            foreach(KeyValuePair<string, AnimationContent> animation in input.Animations)
            {
                // do we have this animation before?
                AnimationClips.Clip clip;
                if (!animationClips.Clips.TryGetValue(animation.Key, out clip))
                {
                    //never before seen clip
                    System.Diagnostics.Trace.WriteLine("New clip: " + animation.Key);
                    clip = new AnimationClips.Clip();
                    clip.Name = animation.Key;
                    clip.Duration = animation.Value.Duration.TotalSeconds;
                    clip.Keyframes = new List<AnimationClips.Keyframe>[bones.Count];
                    for (int b = 0; b < bones.Count; b++)
                        clip.Keyframes[b] = new List<AnimationClips.Keyframe>();

                    animationClips.Clips[animation.Key] = clip;
                }

                //for each canell, determine the bone and then process all of the keyframes for that bone
                foreach (KeyValuePair<string, AnimationChannel> channel in animation.Value.Channels)
                {
                    // what is the bone index?
                    int boneIndex;
                    if (!bones.TryGetValue(channel.Key, out boneIndex))
                        continue; //ignore if not a named bone

                    foreach (AnimationKeyframe keyframe in channel.Value)
                    {
                        Matrix transform = keyframe.Transform; //keyframe transformation
                        AnimationClips.Keyframe newKeyFrame = new AnimationClips.Keyframe();
                        newKeyFrame.Time = keyframe.Time.TotalSeconds;

                        transform.Right = Vector3.Normalize(transform.Right);
                        transform.Up = Vector3.Normalize(transform.Up);
                        transform.Backward = Vector3.Normalize(transform.Backward);
                        newKeyFrame.Rotation = Quaternion.CreateFromRotationMatrix(transform);
                        newKeyFrame.Translastion = transform.Translation;

                        clip.Keyframes[boneIndex].Add(newKeyFrame);
                    }
                }
            }
            //System.Diagnostics.Trace.WriteLine(input.Name);

            foreach (NodeContent child in input.Children)
            {
                ProcessAnimationRecursive(child, animationClips);
            }
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            model = base.Process(input, context);
            AnimationClips clips = ProcessAnimations(model, input, context);
            model.Tag = clips;
            return model;
        }
    }
}