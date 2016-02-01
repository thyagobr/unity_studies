using UnityEngine;

public class QuestManager : MonoBehaviour
{

  public static QuestManager instance;
  public Quest shoot_first;

  void Awake()
  {
    instance = this;
  }

  void Start()
  {
    shoot_first = new Quest();
    Objective first_objective = new EliminationObjective("Hover Droid", 3);
    shoot_first.objectives.Add(first_objective);
    shoot_first.startQuest();
  }

  void Update()
  {
    if ((shoot_first.questState == Quest.QuestState.Active) && (shoot_first.checkCompletion() == true))
    {
      shoot_first.questState = Quest.QuestState.Complete;
      Debug.Log("Quest completed.");
    }

    if (Input.GetKeyDown(KeyCode.A))
    {
      Actor actor = new Actor();
      actor.name = "Hover Droid";
      actor.Die();
    }
  }

  public void addQuestTo(Player player, Quest quest)
  {
    Debug.Log(string.Format("-> Adding {0} to player", quest.name));
    player.addQuest(quest);
  }

}