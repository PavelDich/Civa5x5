using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Minicop.Game.GravityRave
{
    public class CameraMovement : MonoBehaviour
    {
        private Transform _transform;
        private Rigidbody _rigidbody;
        private float _rotationY;
        private float _rotationX;

        [SerializeField]
        private float _rotateSpeed = 1f;

        [SerializeField]
        private float _walkVelocity = 50f;
        [SerializeField]
        private float _walkSpeed = 1f;
        [SerializeField]
        private float _runVelocity = 50f;
        [SerializeField]
        private float _runSpeed = 1f;
        private void Start()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
        }
        public void Rotate(Vector2 direction)
        {
            _rotationY -= direction.y * _rotateSpeed * Time.deltaTime;
            _rotationY = Mathf.Clamp(_rotationY, -90, 90);
            _rotationX += direction.x * _rotateSpeed * Time.deltaTime;
            _transform.rotation = Quaternion.Euler(_rotationY, _rotationX, 0f);
        }

        public void Walk(Vector2 direction)
        {
            Move(direction, _walkVelocity, _walkSpeed);
        }
        public void Run(Vector2 direction)
        {
            Move(direction, _runVelocity, _runSpeed);
        }
        public void Move(Vector2 direction, float velocity, float speed)
        {
            _rigidbody.AddRelativeForce(new Vector3(direction.x, 0f, direction.y) * _walkVelocity * Time.fixedDeltaTime);
            float magnitude = Vector3.Magnitude(_rigidbody.velocity);
            if (magnitude > _walkSpeed)
            {
                float brakeSpeed = magnitude - _walkSpeed;
                Vector3 normalisedVelocity = _rigidbody.velocity.normalized;
                Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;
                _rigidbody.AddForce(-brakeVelocity);
            }
        }

        public void Break()
        {
            float speed = Vector3.Magnitude(_rigidbody.velocity);
            float brakeSpeed = speed - 0f;
            Vector3 normalisedVelocity = _rigidbody.velocity.normalized;
            Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;
            _rigidbody.AddForce(-brakeVelocity);
        }
    }
}