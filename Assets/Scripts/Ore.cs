using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : UnderwaterEntity
{
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);

        if (col.name == "SubmarineSprite")
        {
            GameManager.instance.CollectOre();
            Destroy(this.gameObject);
        }
    }
}
