using UnityEngine;

namespace Game.CameraHelper
{
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;
        
        [SerializeField]
        private float padding = 1f;

        public void AdjustCamera(int boardWidth, int boardHeight, float cellSize)
        {
            float gridTotalWidth = boardWidth * cellSize;
            float gridTotalHeight = boardHeight * cellSize;

            float screenRatio = (float)Screen.width / Screen.height;
            float targetRatio = gridTotalWidth / gridTotalHeight;

            if (screenRatio >= targetRatio)
            {
                cam.orthographicSize = (gridTotalHeight / 2f) + padding;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                cam.orthographicSize = (gridTotalHeight / 2f * differenceInSize) + padding;
            }
        }
    }
}
