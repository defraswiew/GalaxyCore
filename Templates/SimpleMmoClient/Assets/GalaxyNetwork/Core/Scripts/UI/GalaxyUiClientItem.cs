using UnityEngine;
using UnityEngine.UI;
namespace GalaxyCoreLib
{
    public class GalaxyUiClientItem : MonoBehaviour
    {
        [SerializeField]
        private Text label;
        public void Init(int id, string login)
        {
            label.text = "(Id:" + id + ") " + login;
        }
    }
}
