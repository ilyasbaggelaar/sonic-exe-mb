using System;
using UnityEngine;

namespace _Scripts
{
    public class ParallaxEffect : MonoBehaviour
    {
        private float _startingPosX, _startingPosY;
          private float  _lengthOfSprite; //This is the length of the sprites.
        public float AmountOfParallax; //This is amount of parallax scroll. 
        
        public Camera MainCamera; //Reference of the camera.

        public float boundarySize = 26.6f;

        public float divisionAmount = 2f;


        [Header("Vertical Parallax (Optional)")]
        public bool enableVerticalParallax = false;
        public float verticalParallaxAmount = 0.5f;



        private void Start()
        {
            //Getting the starting X position of sprite.
            _startingPosX = transform.position.x;
            _startingPosY = transform.position.y;
            //Getting the length of the sprites + adding padding so it can loop.
            _lengthOfSprite = GetComponent<SpriteRenderer>().bounds.size.x + boundarySize;
        }



        private void Update()
        {
            Vector3 Position = MainCamera.transform.position;
            float Temp = Position.x * (1 - AmountOfParallax); // howmuch the camera has moved based on the parallax amount.
            float DistanceY = 0f;
            float Distance = Position.x * AmountOfParallax; // how much the sprite should move based on the camera position and the parallax amount.

            if (enableVerticalParallax)
            {
                DistanceY = Position.y * verticalParallaxAmount;
            }

            Vector3 NewPosition = new Vector3(_startingPosX + Distance, _startingPosY + DistanceY, transform.position.z);

            transform.position = NewPosition;

            if (Temp > _startingPosX + (_lengthOfSprite / divisionAmount)) //this halfs the sprite length so it can loop.
            {
                _startingPosX += _lengthOfSprite;
            }
            else if (Temp < _startingPosX - (_lengthOfSprite / divisionAmount))
            {
                _startingPosX -= _lengthOfSprite;
            }
        }
    }
}