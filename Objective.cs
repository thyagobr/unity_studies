using UnityEngine;

public abstract class Objective
{
  public Quest.QuestState state;
}

public class EliminationObjective : Objective
{
  public string target_name = "Hover Droid";
  public int quantity = 3;

  public EliminationObjective(string target_name, int quantity) : base()
  {
    this.target_name = target_name;
    this.quantity = quantity;

    Actor.OnDestroy += ActorDestroyed;
  }

  public void ActorDestroyed(Actor target)
  {
    if ((state == Quest.QuestState.Active) && (target.name == target_name) && (target.isDead))
    {
      quantity--;
      if (quantity <= 0)
      {
        state = Quest.QuestState.Complete;
        Debug.Log("Objective completed.");
      }
      else
      {
        Debug.Log(string.Format("{0} more to complete...", quantity));
      }
    }
  }

}