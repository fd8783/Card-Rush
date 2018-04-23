using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface health {

    void Hurt(float damage);

    void Hurt(float damage, Vector2 vel);

    void ShieldUp(float amount);
}
