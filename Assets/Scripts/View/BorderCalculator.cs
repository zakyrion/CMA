using CMA;
using Model;
using UnityEngine;

namespace View
{
    [RequireComponent(typeof(Camera))]
    public class BorderCalculator : MonoActor<string>
    {
        private Rect _border;

        private void Awake()
        {
            Init("BorderCalculator");
            Main.Instance.AddActor(this);

            var camera = GetComponent<Camera>();
            var height = 2.0f * transform.position.y * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var width = height * camera.aspect;

            _border = new Rect(width / -2, height / -2, width, height);
        }

        private void OnRectRequest()
        {
            Message.Done(_border);
        }

        protected override void Subscribe()
        {
            Receive<Rect>(OnRectRequest);
        }
    }
}