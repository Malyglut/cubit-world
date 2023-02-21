using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld
{
    public class ScreenshotCamera : MonoBehaviour
    {
        [FormerlySerializedAs("camera"),SerializeField]
        private Camera _camera;

        [SerializeField]
        private int _screenshotWidth= 512;
        
        [SerializeField]
        private int _screenshotHeight = 512;

        public Sprite TakeScreenshot()
        {
            // Create a new texture with the specified width and height
            var texture = new Texture2D(_screenshotWidth, _screenshotHeight, TextureFormat.RGB24, false);

            // Render the camera's view to the texture
            var currentRT = RenderTexture.active;
            var renderTexture = new RenderTexture(_screenshotWidth, _screenshotHeight, 24);
            _camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            _camera.Render();
            texture.ReadPixels(new Rect(0, 0, _screenshotWidth, _screenshotHeight), 0, 0);
            _camera.targetTexture = null;
            RenderTexture.active = currentRT;
            Destroy(renderTexture);

            texture.Apply();
            // Create a new sprite from the texture
            var sprite = Sprite.Create(texture, new Rect(0, 0, _screenshotWidth, _screenshotHeight), new Vector2(0.5f, 0.5f));

            return sprite;
        }
    }
}