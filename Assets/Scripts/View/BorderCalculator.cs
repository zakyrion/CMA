using CMA.Messages;
using Model;
using UnityEngine;

namespace View
{
    [RequireComponent(typeof(Camera))]
    public class BorderCalculator : MonoBehaviour
    {
        private Rect _border;

        private void Awake()
        {
            var camera = GetComponent<Camera>();
            var height = 2.0f * transform.position.y * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            var width = height * camera.aspect;

            _border = new Rect(width / -2, height / -2, width, height);
            Main.Instance.SubscribeMessage<Rect>(OnRectRequest);
        }

        private void OnRectRequest(IMessage message)
        {
            message.Done(_border);
        }
    }
}