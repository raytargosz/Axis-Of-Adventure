using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistantLands.Cozy
{

    public class LightListener : MonoBehaviour
    {

        public Material onMat;
        public Material offMat;
        public Light myLight;
        private Renderer render;

        public void TurnOnLight()
        {

            if (myLight == null)
                myLight = GetComponent<Light>();
            if (render == null)
                render = GetComponent<Renderer>();

            render.material = onMat;
            myLight.enabled = true;

        }

        public void TurnOffLight()
        {

            if (myLight == null)
                myLight = GetComponent<Light>();
            if (render == null)
                render = GetComponent<Renderer>();

            render.material = offMat;
            myLight.enabled = false;
        }
    }
}