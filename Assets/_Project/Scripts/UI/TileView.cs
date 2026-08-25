using System;
using DragLinks.Board;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DragLinks.UI
{
    public sealed class TileView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Color numberColor = new Color(0.2f, 0.55f, 0.9f);
        [SerializeField] private Color operatorColor = new Color(0.35f, 0.35f, 0.4f);
        [SerializeField, Range(1f, 2f)] private float selectedBrightness = 1.35f;

        private Color baseColor;
        private Action<BoardCoordinate> pointerDown;
        private Action<BoardCoordinate> pointerEnter;
        private Action pointerUp;

        public BoardCoordinate Coordinate { get; private set; }

        public void Initialize(BoardCoordinate coordinate, Action<BoardCoordinate> onPointerDown,
            Action<BoardCoordinate> onPointerEnter, Action onPointerUp)
        {
            Coordinate = coordinate;
            pointerDown = onPointerDown;
            pointerEnter = onPointerEnter;
            pointerUp = onPointerUp;
        }

        public void Render(TileState state)
        {
            if (state == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            label.text = state.Kind == TileKind.Number
                ? state.CurrentValue.ToString()
                : ToSymbol(state.OperatorType);
            baseColor = state.Kind == TileKind.Number ? numberColor : operatorColor;
            background.color = baseColor;
        }

        public void SetSelected(bool selected)
        {
            background.color = selected
                ? new Color(
                    Mathf.Clamp01(baseColor.r * selectedBrightness),
                    Mathf.Clamp01(baseColor.g * selectedBrightness),
                    Mathf.Clamp01(baseColor.b * selectedBrightness),
                    baseColor.a)
                : baseColor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) pointerDown?.Invoke(Coordinate);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // pointerPress remains assigned for the whole held-button gesture even without an IDragHandler.
            if (eventData.pointerPress != null) pointerEnter?.Invoke(Coordinate);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) pointerUp?.Invoke();
        }

        private static string ToSymbol(OperatorType type)
        {
            switch (type)
            {
                case OperatorType.Add: return "+";
                case OperatorType.Subtract: return "−";
                case OperatorType.Multiply: return "×";
                case OperatorType.Divide: return "÷";
                default: return "?";
            }
        }

        private void OnValidate()
        {
            if (background == null) background = GetComponent<Image>();
            if (label == null) label = GetComponentInChildren<TMP_Text>();
        }
    }
}
