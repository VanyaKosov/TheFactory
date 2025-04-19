using Dev.Kosov.Factory.Core;
using UnityEngine;

namespace Dev.Kosov.Factory.Graphics
{
    public class InserterController : MonoBehaviour, IEntityInitializer
    {
        private Inserter inserter;

        public GameObject ArmBase;

        void Update()
        {
            //var ea = ArmBase.transform.eulerAngles;
            //ea.z = inserter.GetTotalDegrees();
            //ArmBase.transform.eulerAngles = ea;

            ArmBase.transform.eulerAngles = new(0, 0, inserter.GetTotalDegrees());
        }

        public void Init(Entity entity)
        {
            inserter = (Inserter)entity;
            //print("Initialized inserter");
        }
    }
}
