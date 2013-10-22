using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace AnimationSteps
{
    public class Camera
    {
        #region Fields
        private Vector3 eye = new Vector3(1000, 1000, 1000);
        private Vector3 center = new Vector3(0, 0, 0);
        private Vector3 up = new Vector3(0, 1, 0);
        private float fov = MathHelper.ToRadians(35);
        private float znear = 10;
        private float zfar = 10000;

        #region CAMERA SPRING VARIABLES

        private bool useChaseCamera = false;
        private Vector3 desiredEye = Vector3.Zero;
        private Vector3 velocity = Vector3.Zero;
        private float stiffness = 100;
        private float damping = 60;
        private Vector3 desiredUp;

        #endregion

        private Matrix view;
        private Matrix projection;

        #endregion

        #region Mouse Movement Variables
        private bool mousePitchYaw = true;
        private bool mousePanTilt = true;
        private MouseState lastMouseState;



        #endregion

        private GraphicsDeviceManager graphics;

        public Camera(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            desiredUp = up;
        }

        private void ComputeProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fov, graphics.GraphicsDevice.Viewport.AspectRatio, znear, zfar);
        }

        private void ComputeView()
        {
            view = Matrix.CreateLookAt(eye, center, up);
        }

        public void Initialize()
        {
            ComputeView();
            ComputeProjection();
            lastMouseState = Mouse.GetState();
        }

        public Matrix View
        {
            get
            {
                return view;
            }
        }

        public Matrix Projection
        {
            get
            {
                return projection;
            }
        }

        public void Pitch(float angle)
        {
            //need a vector in the camera x direction
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            float len = cameraX.LengthSquared();
            if (len > 0)
                cameraX.Normalize();
            else
                cameraX = new Vector3(1, 0, 0);

            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix r = Matrix.CreateFromAxisAngle(cameraX, angle);
            Matrix t2 = Matrix.CreateTranslation(center);

            Matrix M = t1 * r * t2;
            eye = Vector3.Transform(eye, M);
            ComputeView();
        }

        public void Yaw(float angle)
        {
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            Vector3 cameraY = Vector3.Cross(cameraZ, cameraX);
            float len = cameraY.LengthSquared();
            if (len > 0)
                cameraY.Normalize();
            else
                cameraY = new Vector3(0, 1, 0);

            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix r = Matrix.CreateFromAxisAngle(cameraY, angle);
            Matrix t2 = Matrix.CreateTranslation(center);

            Matrix M = t1 * r * t2;
            eye = Vector3.Transform(eye, M);
            ComputeView();

        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (mousePitchYaw && mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                float changeY = mouseState.Y - lastMouseState.Y;
                Pitch(-changeY * .005f);
            }

            if (mousePitchYaw && mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Pressed)
            {
                float changeX = mouseState.X - lastMouseState.X;
                Yaw(-changeX * .005f);
            }

            if (mousePanTilt && mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Pressed)
            {
                float changeX = mouseState.X - lastMouseState.X;
                Pan(changeX);
            }

            if (mousePanTilt && mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Pressed)
            {
                float changeY = mouseState.Y - lastMouseState.Y;
                Tilt(changeY);
            }


            if (useChaseCamera)
            {
                //calculate spring force
                Vector3 stretch = desiredEye - eye;
                Vector3 accleration = stretch * stiffness - velocity * damping;

                Vector3 angle = desiredUp - up;
                //Vector3 roll = spin * stiffness - velocity * damping;
                //Vector3 accleration = stretch * stiffness - velocity * damping;

                //apply accleration
                velocity += accleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                ////apply velocity
                eye += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //  up += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            lastMouseState = mouseState;
        }

        public void Pan(float angle)
        {

            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix t2 = Matrix.CreateTranslation(angle, 0, 0);
            Matrix t3 = Matrix.CreateTranslation(center);

            Matrix m = t1 * t2 * t3;
            center = Vector3.Transform(center, m);
            ComputeView();

        }

        public void Tilt(float angle)
        {
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix t2 = Matrix.CreateTranslation(0, angle, 0);
            Matrix t3 = Matrix.CreateTranslation(center);

            Matrix m = t1 * t2 * t3;
            center = Vector3.Transform(center, m);
            ComputeView();
        }

        #region CAMERA SETUP PROPERTIES

        public Vector3 Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
                ComputeView();
            }

        }

        public Vector3 Eye
        {
            get
            {
                return eye;
            }
            set
            {
                eye = value;
                ComputeView();
            }
        }

        public float FOV
        {
            get
            {
                return fov;
            }
            set
            {
                fov = value;
                ComputeView();
            }
        }

        public Vector3 Up
        {
            get
            {
                return up;
            }
            set
            {
                up = value;
            }
        }

        #endregion

        #region CAMERA SPRING PROPERTIES

        public bool UseChaseCamera
        {
            get
            {
                return useChaseCamera;
            }
            set
            {
                useChaseCamera = value;
            }
        }
        public Vector3 DesiredEye
        {
            get
            {
                return desiredEye;
            }
            set
            {
                desiredEye = value;
            }
        }

        public Vector3 DesiredUp
        {
            get
            {
                return desiredUp;
            }
            set
            {
                desiredUp = value;
            }
        }

        public float Stiffness
        {
            get
            {
                return stiffness;
            }
            set
            {
                stiffness = value;
            }
        }

        public float Damping
        {
            get
            {
                return damping;
            }
            set
            {

                damping = value;
            }

        }
        #endregion

        public bool MousePitchYaw
        {
            get
            {
                return mousePitchYaw;
            }
            set
            {
                mousePitchYaw = value;
            }
        }
        public bool MousePanTilt
        {
            get
            {
                return MousePanTilt;
            }
            set
            {
                MousePanTilt = value;
            }
        }

    }
}
