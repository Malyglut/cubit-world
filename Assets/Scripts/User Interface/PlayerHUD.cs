using Malyglut.CubitWorld.Utilties;
using UnityEngine;

namespace Malyglut.CubitWorld.UserInterface
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hudObject;
        
        [SerializeField]
        private GameEvent _inventoryOpened;
        
        [SerializeField]
        private GameEvent _inventoryClosed;

        private void Awake()
        {
            _inventoryOpened.Subscribe(() => _hudObject.SetActive(false));
            _inventoryClosed.Subscribe(() => _hudObject.SetActive(true));
        }
    }
}