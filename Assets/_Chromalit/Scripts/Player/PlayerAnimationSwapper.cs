using UnityEngine;
using Chromalit.Color;

namespace Chromalit.Player
{
    public class PlayerAnimationSwapper : MonoBehaviour
    {
        [System.Serializable]
        public struct ColorAnimSet
        {
            public ColorType colorType;
            public RuntimeAnimatorController controller;
        }

        [SerializeField] private RuntimeAnimatorController baseController;
        [SerializeField] private ColorAnimSet[] colorAnimSets;

        private Animator _animator;
        private PlayerColorState _colorState;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _colorState = GetComponent<PlayerColorState>();
        }

        private void OnEnable()
        {
            if (_colorState != null)
                _colorState.OnColorChanged += OnColorChanged;
        }

        private void OnDisable()
        {
            if (_colorState != null)
                _colorState.OnColorChanged -= OnColorChanged;
        }

        private void OnColorChanged(ColorData color, ColorSource source)
        {
            if (color == null || color.colorType == ColorType.White)
            {
                _animator.runtimeAnimatorController = baseController;
                return;
            }

            foreach (var set in colorAnimSets)
            {
                if (set.colorType == color.colorType)
                {
                    _animator.runtimeAnimatorController = set.controller;
                    return;
                }
            }

            _animator.runtimeAnimatorController = baseController;
        }
    }
}