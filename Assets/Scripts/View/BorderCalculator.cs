using CMA;
using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    [RequireComponent(typeof(Camera))]
    public class BorderCalculator : MonoActor
    {
        private Rect _border;

        protected override void Awake()
        {
            base.Awake();

            Main.Instance.AddActor(this, "Main/BorderCalculator");

            var camera = GetComponent<Camera>();
            var height = 2.0f * transform.position.y * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var width = height * camera.aspect;

            _border = new Rect(width / -2, height / -2, width, height);
        }

        private void OnRectRequest(IMessage message)
        {
            Debug.Log($"Catch OnRectRequest Adress:{message.Adress}, Back Adress: {message.BackAdress}");
            Respounce(message, _border);
        }

        protected override void Subscribe()
        {
            Receive<Rect>(OnRectRequest);
        }
    }
}