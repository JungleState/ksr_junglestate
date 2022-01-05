using Program;
using System.Random;
using System;
using System.Collections.Generic;


namespace junglecamp {
    /// A not-so-smart monkey that moves in a random direction (but not a wall).
    class MySimpleMonkey : Monkey {
        protected override Move nextMove(GameState state) {
            Direction direction = selectRandomDirection(state);
            return new Action{action = Action.MOVE, direction = direction, nextRound = state.round};
        }

        private Direction selectRandomDirection(GameState state) {
            List<Direction> freeDirs = computeFreeDirections(state);
            Random random = new System.Random();
            int r = random.NextInt(freeDirs.Size());
            return freeDirs[r];
        }

        private List<Direction> computeFreeDirs(GameState state) {
            List<Direction> results = new List<>();
            foreach (Direction dir in Enum.GetValues(typeof(Direction))) {
                Cell cell = dir.GetCell(state);
                if (cell.isMoveable()) {
                    result.Add(dir);
                }
            }
            return result;
        }
    }
}