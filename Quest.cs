// First: grab the quest, talk to the guy again, quest completes
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Quest
{
  public enum QuestState { Inactive, Active, Complete }

  public QuestState questState;
  public string name;
  public int id;
  public string description;
  public int xp;
  public bool completed;
  public int reward; // for now, integer gold value
  public List<Objective> objectives;

  public Quest()
  {
    this.objectives = new List<Objective>();
  }

  public void startQuest()
  {
    this.questState = QuestState.Active;
    foreach(Objective objective in objectives)
      objective.state = QuestState.Active;
  }

  public bool checkCompletion()
  {
    foreach(Objective objective in objectives)
    {
      if (objective.state == QuestState.Active)
        return false;
    }
    return true;
  }
}