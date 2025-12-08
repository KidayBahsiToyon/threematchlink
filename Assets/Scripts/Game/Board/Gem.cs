using DG.Tweening;
using UnityEngine;
using Game.Constants;

namespace Game.Board
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Gem : MonoBehaviour
    {
        [SerializeField] 
        SpriteRenderer _renderer;
        
        private GemType _type;
        private int _x, _y;
        private bool _moving;

        public GemType Type => _type;
        public int X => _x;
        public int Y => _y;
        public bool IsMoving => _moving;

        private void Awake()
        {
            if (!_renderer)
            {
                _renderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Initialize(GemType type, int x, int y)
        {
            _type = type;
            _x = x;
            _y = y;
            transform.localScale = Vector3.one * GameConstants.GEM_SCALE;
            
            if (_renderer && type)
            {
                _renderer.sprite = type.sprite;
            }
        }

        public void SetGridPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public void Fall(Vector3 target, float duration = GameConstants.ANIM_DURATION)
        {
            _moving = true;
            transform.DOMove(target, duration).SetEase(Ease.InQuad).OnComplete(() => _moving = false);
        }

        public void PlaySpawnAnimation(float duration = GameConstants.ANIM_DURATION)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one * GameConstants.GEM_SCALE, duration).SetEase(Ease.OutBack);
        }

        public void PlayDestroyAnimation(float duration = GameConstants.ANIM_DURATION)
        {
            var seq = DOTween.Sequence();
            seq.Append(transform.DOScale(Vector3.one * GameConstants.GEM_SCALE * GameConstants.DESTROY_SCALE_UP, duration * GameConstants.DESTROY_SCALE_UP_RATIO));
            seq.Append(transform.DOScale(Vector3.zero, duration * GameConstants.DESTROY_SCALE_DOWN_RATIO).SetEase(Ease.InBack));
            
            if (_renderer)
            {
                seq.Join(_renderer.DOFade(0f, duration));
            }
            
            seq.Play();
        }

        public void PlaySelectAnimation()
        {
            transform.DOPunchScale(Vector3.one * GameConstants.GEM_SCALE * GameConstants.PUNCH_SCALE, GameConstants.ANIM_DURATION, GameConstants.PUNCH_VIBRATO, GameConstants.PUNCH_ELASTICITY);
        }

        void OnDestroy()
        {
            transform.DOKill();
            _renderer?.DOKill();
        }
    }
}
