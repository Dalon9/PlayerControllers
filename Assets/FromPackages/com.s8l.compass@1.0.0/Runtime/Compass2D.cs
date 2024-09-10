using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace S8l.Compass.Runtime
{
    public class Compass2D : MonoBehaviour
    {
        //Transform of direction arrow used to rotate towards target
        public RectTransform DirectionArrow;
        //Colors of the arrow if the player looks exactly towards to or away from the current target 
        public Color DirectionArrowTowardsColor, DirectionArrowAwayColor = Color.white;

        [SerializeField]
        private Transform player;
        [SerializeField]
        private Transform target;
        private Image directionArrowImage;
        private float dirationArrowTowardsHue, directionArrowAwayHue;
        private float directionArrowSat, directionArrowVal;

        void Awake()
        {
            if (player == null)
            {
                player = Camera.main.transform;
            }
            directionArrowImage = DirectionArrow.GetComponent<Image>();
            //Get the hue values of the arrow when walking in the right and the wrong direction
            Color.RGBToHSV(DirectionArrowTowardsColor, out dirationArrowTowardsHue, out directionArrowSat, out directionArrowVal);
            Color.RGBToHSV(DirectionArrowAwayColor, out directionArrowAwayHue, out directionArrowSat, out directionArrowVal);
        }

        void Update()
        {
            if (target != null)
            {
                //Rotate arrow depending on current player rotation
                float angle = Vector3.SignedAngle(Vector3.Scale(target.position - player.position, new Vector3(1f, 0f, 1f)), Vector3.Scale(player.forward, new Vector3(1f, 0f, 1f)), Vector3.up);
                DirectionArrow.localRotation = Quaternion.Euler(DirectionArrow.localRotation.x, DirectionArrow.rotation.y, angle);
                //Change color of the arrow depending on the player rotation
                float newHue = (1 - Mathf.Abs(angle) / 180f) * (dirationArrowTowardsHue - directionArrowAwayHue) + directionArrowAwayHue;
                directionArrowImage.color = Color.HSVToRGB(newHue, directionArrowSat, directionArrowVal);
            }
            else if (directionArrowImage.enabled)
            {
                directionArrowImage.enabled = false;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            directionArrowImage.enabled = true;
        }

        public void SetPlayer(Transform newPlayer)
        {
            player = newPlayer;
        }
    }
}