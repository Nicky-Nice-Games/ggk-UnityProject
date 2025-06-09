using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    internal class ButtonBehavior : MonoBehaviour
    {
        [HideInInspector]
        public string buttonClickedName = null;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnClick()
        {
            buttonClickedName = EventSystem.current.currentSelectedGameObject.name;
            Debug.Log(buttonClickedName);
        }
    }
}
