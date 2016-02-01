using UnityEngine;
using System.Collections;

public class QuestGiver : MonoBehaviour
{

  Quest first_quest;

  public void Awake()
  {
    first_quest = new Quest();
    first_quest.name = "First Quest";
    print("I'm awake, I'm awake");
  }

  public void OnMouseDown()
  {
    QuestManager.instance.addQuestTo(Overlord.instance.current_player, first_quest);
  }

}