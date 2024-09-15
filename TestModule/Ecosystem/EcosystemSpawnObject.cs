using UnityEngine;

namespace Assets.StandardQualityControlLibary.TestModule.Ecosystem
{
    public class EcosystemSpawnObject : MonoBehaviour
    {
        public EcoSystemSpawnArea ecosystemSpawnArea;

        public void Init(EcoSystemSpawnArea ecosystemSpawnArea)
        {
            this.ecosystemSpawnArea = ecosystemSpawnArea;
            ecosystemSpawnArea.curAmount++;
        }

        private void OnDisable()
        {
            if(ecosystemSpawnArea == null)
            {
                return;
            }
            ecosystemSpawnArea.curAmount--;
            ecosystemSpawnArea = null;
        }
    }
}