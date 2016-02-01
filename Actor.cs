using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Actor
{

  public string name;
  public bool isDead;

  public delegate void ActorEventHandler(Actor target);
  public static event ActorEventHandler OnDestroy;

  void Start()
  {
    isDead = false;
  }

  public void Die()
  {
    if (!isDead)
    {
      isDead = true;
      if (OnDestroy != null)
        OnDestroy(this);
    }
  }
}