namespace junglestate;

///<summary>A blank monkey that stays on the spot.</summary>
public class Monkey : BaseMonkey {
    private Direction lastDir = Direction.NONE;
    public override Move nextMove(GameState state) {





        return new Move(Action.STAY, Direction.NONE);
    }





}