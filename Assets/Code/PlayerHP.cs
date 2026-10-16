using UnityEngine;

public class PlayerHP : StaminaControl
{
    protected override void Die()
    {
        base.Die();

        Debug.Log("Game Over");
    }
}