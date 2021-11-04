using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ProgressCondition", menuName = "Tutorial Objects/Progress Condition")]
public class ProgressCondition : ScriptableObject
{
    public enum Type
    {
        PRESS_CONTINUE,
        TARGET_HEX,
        CONFIRM_ACTION,
        SELECT_MODE,
        WAIT_FOR_TRIGGER,
        ACTION_COMPLETE
    }

    public Type type;
    public int[] paramaters;
}
