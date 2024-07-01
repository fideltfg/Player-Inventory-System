using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerInventorySystem
{
    public class FurnaceController : Interactive
    {

        internal Slot FuelSlot = new Slot(0);

        internal Slot MaterialSlot = new Slot(1);

        internal Slot OutputSlot = new Slot(2);

        internal bool isProcessing = false;

        public void Start()
        {
            gameObject.tag = "Furnace";
        }

        public void Update()
        {
            if (!isProcessing && MaterialSlot.StackCount > 0 && FuelSlot.StackCount > 0)
            {
                StartCoroutine(ProcessMaterials());
            }
        }

        internal float processTimer = 0;
        internal float fuelTimer = 0;

        private IEnumerator ProcessMaterials()
        {
            isProcessing = true;

            if (processTimer < MaterialSlot.Item.Data.smeltTime)
            {
                if (fuelTimer < FuelSlot.Item.Data.fuelValue)
                {
                    fuelTimer += Time.deltaTime;
                }
                else
                {
                    FuelSlot.IncermentStackCount(-1);
                    fuelTimer = 0;
                }

                processTimer += Time.deltaTime;
            }
            else
            {
                processTimer = 0;
            }

            isProcessing = false;

            yield return null;
        }


        public override void Interact(PlayerInventoryController playerInventoryController)
        {
            OpenPanel();
        }
    }
}