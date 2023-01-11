using _Client_.Scripts.Runtime.Structs;
using SFramework.ECS.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Client_.Scripts.Runtime.Settings
{
    [CreateAssetMenu]
    public class GameSettings : ScriptableObject
    {
        public HexLayout HexLayout => hexLayout;
        public int InitialPawns => _initialPawns;
        public float DragRadius => _dragRadius;
        public PawnTypeProbability[] PawnTypeProbabilities => _pawnTypeProbabilities;
        public PawnData[] PawnsData => _pawnsData;
        public float SwapDuration => _swapDuration;
        public Vector3 PawnScale => _pawnScale;
        public Vector3 DraggedPawnScale => _draggedPawnScale;
        public float MaxMoraleAmount => _maxMoraleAmount;
        public RectGrid GridRect => _gridRect;
        public float PawnGenerationTimeout => _pawnGenerationTimeout;
        public string PawnsMatchingFormula => _pawnsMatchingFormula;
        public string PawnsTypeMatchingFormula => _pawnsTypeMatchingFormula;
        public SFEntity CellPrefab => _cellPrefab;
        public SFEntity PawnPrefab => _pawnPrefab;

        [SerializeField]
        private SFEntity _cellPrefab;

        [SerializeField]
        private SFEntity _pawnPrefab;

        [SerializeField]
        private HexLayout hexLayout;

        [SerializeField]
        private RectGrid _gridRect;

        [Min(1)]
        [SerializeField]
        private int _initialPawns;

        [Min(0f)]
        [SerializeField]
        private float _pawnGenerationTimeout;

        [Min(0f)]
        [SerializeField]
        private float _dragRadius;

        [SerializeField]
        private PawnTypeProbability[] _pawnTypeProbabilities;

        [SerializeField]
        private PawnData[] _pawnsData;

        [Min(0f)]
        [SerializeField]
        private float _swapDuration;

        [SerializeField]
        private Vector3 _pawnScale;

        [SerializeField]
        private Vector3 _draggedPawnScale;

        [Min(0f)]
        [SerializeField]
        private float _maxMoraleAmount;

        [SerializeField]
        private string _pawnsMatchingFormula = "5 * [COUNT] * Sqrt([COUNT] - 3)";

        [SerializeField]
        private string _pawnsTypeMatchingFormula = "Pow(2,[COUNT] - 1)";
    }
}