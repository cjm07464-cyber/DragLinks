using DragLinks.Data;
using DragLinks.UI;
using DragLinks.Input;
using DragLinks.Turn;
using DragLinks.Linking;
using DragLinks.Character;
using UnityEngine;

namespace DragLinks.Board
{
    /// <summary>Config, 초기 보드 생성, View 갱신만 연결하는 씬 진입점이다.</summary>
    public sealed class BoardBootstrap : MonoBehaviour
    {
        [SerializeField] private BoardConfig boardConfig;
        [SerializeField] private BoardView boardView;
        [SerializeField] private BoardInputController boardInputController;
        [SerializeField] private bool useRandomSeed = true;
        [SerializeField] private int fixedSeed = 12345;

        public BoardState Board { get; private set; }
        public ChainComboRuntimeState ChainComboState { get; private set; }

        private void Start()
        {
            if (boardConfig == null || boardView == null || boardInputController == null)
            {
                Debug.LogError("BoardBootstrap requires BoardConfig, BoardView, and BoardInputController references.", this);
                enabled = false;
                return;
            }

            var seed = useRandomSeed ? System.Environment.TickCount : fixedSeed;
            var settings = boardConfig.CreateRuntimeSettings();
            var generator = new BoardGenerator(new SeededRandomSource(seed));
            Board = generator.Generate(settings);
            var gravityResolver = new BoardGravityResolver();
            var refillResolver = new BoardRefillResolver(generator);
            ChainComboState = new ChainComboRuntimeState();
            var actionController = new GameplayActionController(
                Board,
                settings,
                gravityResolver,
                refillResolver,
                new LinkingResolver(new LinkingDetector(), gravityResolver, refillResolver),
                ChainComboState,
                new ChainComboResolver());
            boardInputController.Initialize(Board, settings.BaseMaxDragLength, actionController);
            boardView.Render(Board);
        }
    }
}
