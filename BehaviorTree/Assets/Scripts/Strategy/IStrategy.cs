using UnityEngine;
using System.Linq;
public interface IStrategy
{
    /// <summary>
    /// This is like the update fuction of the current strategy/action at play
    /// </summary>
    /// <returns></returns>
    PacoNode.Status Process();

    /// <summary>
    /// This is to reset when strategy/action is failed
    /// </summary>
    public void Reset()
    {
        //Default
    }
}
