using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public class VideoSourceAccess
    {
        private ScreenCaptureTextureManager screenCapture;

        /// <summary>
        ///     Singleton instance of <see cref="VideoSourceAccess"/>.
        /// </summary>
        public static VideoSourceAccess Instance = new();

        /// <summary>
        ///     Creates a new instance of <see cref="VideoSourceAccess" />.
        /// </summary>
        public VideoSourceAccess()
        {
            screenCapture = Object.FindFirstObjectByType<ScreenCaptureTextureManager>();
            this.Texture = screenCapture.Texture;
            this.ActualCameraSize = new Vector2Int(1024, 1024);
        }

     

        /// <summary>
        ///     The resolution of the camera.
        /// </summary>
        public Vector2Int ActualCameraSize { get; private set; }

        public Texture Texture { get; private set; }
    }
}