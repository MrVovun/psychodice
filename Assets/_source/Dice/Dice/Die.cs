using UnityEngine;
using BlackboardSource;

public class Die : BaseDie
{
    public DieSides Sides => sides;
    [SerializeField] DieSides sides;

    public DieSideComponent[] SideComponents => sideComponents;
    [SerializeField] DieSideComponent[] sideComponents;

    public BB_DiceImpulse impulseSettings => _impulseSettings;
    [SerializeField] BB_DiceImpulse _impulseSettings;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    for (int i = 0; i < collision.contacts.Length; i++)
    //    {
    //        var c = collision.contacts[i];
    //        if(c.thisCollider.CompareTag("high-friction") && c.otherCollider.CompareTag("table-surface"))
    //            c.thisCollider.enabled = false;
    //    }
    //}
}
