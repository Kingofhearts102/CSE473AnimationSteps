using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaAux;

namespace AnimationSteps
{
    public class AnimatedModel
    {
        private Model model; 
        /// <summary>
        /// reference to the game this class uses
        /// </summary>
        private StepGame game;
        /// <summary>
        /// name of the asset we are going to load
        /// </summary>
        private string asset;

        private float angle = 0;
        /// <summary>
        /// the bond transforms as loaded from this model
        /// </summary>
        private Matrix[] bindTransforms;
        /// <summary>
        /// The current gone transforms we will use
        /// </summary>
        private Matrix[] bonesTransforms;
        /// <summary>
        /// the computed absolute transforms
        /// </summary>
        private Matrix[] absoTransforms;

        private AnimationClips.Clip clip = null;

        public AnimatedModel(StepGame game, string asset)
        {
            this.game = game;
            this.asset = asset;
        }

        public void LoadContent(ContentManager content)
        {
            model = content.Load<Model>(asset);
            // allocate the array to the number of bones we have
            int boneCnt = model.Bones.Count;
            bindTransforms = new Matrix[boneCnt];
            bonesTransforms = new Matrix[boneCnt];
            absoTransforms = new Matrix[boneCnt];

            model.CopyBoneTransformsTo(bindTransforms);
            model.CopyBoneTransformsTo(bonesTransforms);
            model.CopyAbsoluteBoneTransformsTo(absoTransforms);

            PlayClip("Take 001");
        }

        private AnimationPlayer player = null;

        /// <summary>
        /// Play an animation clip on this model.
        /// </summary>
        /// <param name="name"></param>
        public AnimationClips.Clip PlayClip(string name)
        {
            AnimationClips clips = model.Tag as AnimationClips;
            if (clips != null)
            {
                clip = clips.Clips[name];

                player = new AnimationPlayer(clip);
                player.Initialize();
            }

            return clip;
        }

        public void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.TotalSeconds;

            if (clip != null)
            {
                // Update the clip
                player.Update(delta);

                for (int b = 0; b < player.BoneCount; b++)
                {
                    AnimationPlayer.Bone bone = player.GetBone(b);
                    if (!bone.Valid)
                        continue;

                    Vector3 scale = new Vector3(bindTransforms[b].Right.Length(),
                        bindTransforms[b].Up.Length(),
                        bindTransforms[b].Backward.Length());

                    bonesTransforms[b] = Matrix.CreateScale(scale) *
                        Matrix.CreateFromQuaternion(bone.Rotation) *
                        Matrix.CreateTranslation(bone.Translation);
                }

                model.CopyBoneTransformsFrom(bonesTransforms);
            }

            model.CopyBoneTransformsFrom(bonesTransforms);
            model.CopyAbsoluteBoneTransformsTo(absoTransforms);
        }

        public void Draw(GraphicsDeviceManager graphics, GameTime gameTime)
        {
            Matrix transform = Matrix.Identity;
            DrawModel(graphics, model, transform);
        }

        private void DrawModel(GraphicsDeviceManager graphics, Model model, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = absoTransforms[mesh.ParentBone.Index] * world;
                    effect.View = game.Camera.View;
                    effect.Projection = game.Camera.Projection;
                }
                mesh.Draw();
            }
        }

    }
}
