using System.Collections.Generic;
using UnityEngine;


public interface IAction
{
    // Perform the action
    // Returns the number of action points consumed
    // if the number returned is -1, the action is turn-ending
    int Perform();


    // Determine whether the action is allowed
    bool Possible(int actionsLeft);
}


public class Movement : IAction
{
    private Mover mover;
    private List<Hex> path;

    public Movement(Mover mover, List<Hex> path)
    {
        this.mover = mover;
        this.path = path;
    }

    public int Perform()
    {
        mover.FollowPath(path);
        return 1;
    }


    public bool Possible(int actionsLeft)
    {
        if (actionsLeft <= 0) return false;
        else return mover.CanMove(path.Count - 1);
    }
}


public class Dash : IAction
{
    private Mover mover;
    private List<Hex> path;

    public Dash(Mover mover, List<Hex> path)
    {
        this.mover = mover;
        this.path = path;
    }

    public int Perform()
    {
        mover.FollowPath(path);
        return 2;
    }

    public bool Possible(int actionsLeft)
    {
        if (actionsLeft <= 1) return false;
        else return mover.CanDash(path.Count - 1);
    }
}


public class Attack : IAction
{
    private Fighter attacker;
    private Fighter victim;

    public Attack(Fighter attacker, Fighter victim)
    {
        this.attacker = attacker;
        this.victim = victim;
    }

    public int Perform()
    {
        attacker.Attack(victim, false);
        return -1;
    }

    public bool Possible(int actionsLeft)
    {
        if (actionsLeft <= 0) return false;
        else return attacker.CanAttack(victim, 0);
    }
}


public class FarmerWobblyPitchfork : IAction
{
    private Fighter attacker;
    private Fighter victim;

    public FarmerWobblyPitchfork(Fighter attacker, Fighter victim)
    {
        this.attacker = attacker;
        this.victim = victim;
    }

    public int Perform()
    {
        attacker.Attack(victim, true);
        return -1;
    }

    public bool Possible(int actionsLeft)
    {
        if (actionsLeft <= 0) return false;
        else return attacker.CanAttack(victim, 2);
    }
}

