namespace Tetris.Game
{
    public enum GameEvent
    {
        MoveRightSuccessful,
        MoveRightFailed,
        MoveLeftSuccessful,
        MoveLeftFailed,
        RotationSuccessful,
        RotationFailed,
        HoldSuccessful,
        HoldFailed,
        TetrominoTouchDown,
        TetrominoLocked,
        OneLineCleared,
        TwoLinesCleared,
        ThreeLinesCleared,
        FourLinesCleared,
        HardDrop,
        HardDropWithOneLineClear,
        HardDropWithTwoLinesClear,
        HardDropWithThreeLinesClear,
        HardDropWithFourLinesClear,
        LevelUp,
        PerfectClear
    }
}