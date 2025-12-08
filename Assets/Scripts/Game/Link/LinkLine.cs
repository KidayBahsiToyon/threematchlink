using System.Collections.Generic;
using Game.Board;
using UnityEngine;

namespace Game.Link
{
    [RequireComponent(typeof(LineRenderer))]
    public class LinkLine : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] 
        private float _width = 0.15f;
        
        [SerializeField] 
        private int _minMatch = 3;
        
        [SerializeField] 
        private Material _material;
        
        [SerializeField] 
        private int _sortOrder = 10;

        private LineRenderer _line;
        private List<Vector3> _points = new List<Vector3>();
        private Vector3 _pointer;
        private bool _showPointer;
        
        private Color _validColor = Color.green;
        private Color _invalidColor = Color.yellow;

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            Setup();
        }

        private void Setup()
        {
            _line.startWidth = _width;
            _line.endWidth = _width;
            _line.positionCount = 0;
            _line.useWorldSpace = true;
            _line.sortingOrder = _sortOrder;
            
            _line.material = _material != null ? _material : new Material(Shader.Find("Sprites/Default"));
            
            _line.startColor = _invalidColor;
            _line.endColor = _invalidColor;
        }

        public void UpdateLine(List<Gem> gems)
        {
            if (gems == null || gems.Count == 0)
            {
                ClearLine();
                return;
            }

            _points.Clear();
            foreach (var gem in gems)
            {
                if (gem != null)
                {
                    _points.Add(gem.transform.position);
                }
            }

            // change color based on validity
            bool valid = gems.Count >= _minMatch;
            var color = valid ? _validColor : _invalidColor;
            _line.startColor = color;
            _line.endColor = color;

            Render();
        }

        public void SetPointerPosition(Vector3 pos)
        {
            _pointer = pos;
            _showPointer = true;
            Render();
        }

        public void ClearLine()
        {
            _points.Clear();
            _showPointer = false;
            _line.positionCount = 0;
        }

        private void Render()
        {
            int total = _points.Count + (_showPointer ? 1 : 0);
            
            if (total == 0)
            {
                _line.positionCount = 0;
                return;
            }

            _line.positionCount = total;
            
            for (int i = 0; i < _points.Count; i++)
            {
                _line.SetPosition(i, _points[i]);
            }

            if (_showPointer && _points.Count > 0)
            {
                _line.SetPosition(_points.Count, _pointer);
            }
        }

        public void SetMinMatch(int val) => _minMatch = val;
        
        public void SetColors(Color valid, Color invalid)
        {
            _validColor = valid;
            _invalidColor = invalid;
        }
    }
}
