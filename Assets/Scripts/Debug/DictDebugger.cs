using UnityEngine;

public class DictDebugger : MonoBehaviour
{
  void Start()
  {
    Logger.Log("j1", TechIdNameDict.Dict[TechIDs.j1]);
    Logger.Log("ss", TechIdNameDict.Dict[TechIDs.ss]);
    Logger.Log("da", TechIdNameDict.Dict[TechIDs.da]);
    Logger.Log("DA", TechIdNameDict.Dict[TechIDs.DA]);
  }
}