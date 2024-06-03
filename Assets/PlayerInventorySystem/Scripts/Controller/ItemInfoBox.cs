using UnityEngine;
using UnityEngine.UI;

namespace PlayerInventorySystem
{
    /// <summary>
    /// Component methods and parameters for displaying information about items under the cursor
    /// </summary>
    public class ItemInfoBox : MonoBehaviour
    {

        public Image image;
        public Text itemName;
        public Text description;
        public GameObject statsPanel;

        // public Text durability;

        public Text health;
        public Text mana;
        public Text stamina;
        public Text dexterity;
        public Text IQ;
        public Text armor;
        public Text speed;
        public Text strength;
        public Text luck;
        public Text damage;
        public Text itemID;
        public Text ItemType;
        public Text SlotType;
        public Text BaseMaterial;

        public Slider dSlider;


        /// <summary>
        /// method to display the item info box
        /// </summary>
        /// <param name="item"></param>
        public void Show(Item item)
        {
            if (item != null)
            {
                gameObject.SetActive(true);
                this.itemName.text = item.Data.name;
                this.description.text = item.Data.description;

                // add a list of item names that use this item in crafting
                string[] uses = item.Uses;
                if (uses.Length > 0)
                {
                    this.description.text += "\n" + "Required to make: ";

                    for (int i = 0; i < uses.Length; i++)
                    {
                        this.description.text += uses[i];
                        if (i + 1 < uses.Length)
                        {
                            this.description.text += ", ";
                        }
                    }
                }

                this.image.sprite = item.Data.sprite;

                if (item.Data.itemType == ITEMTYPE.CONSUMABLE)
                {
                    // hide durability slider
                    this.dSlider.enabled = false;
                    // gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 110);
                    statsPanel.SetActive(false);
                }
                else if (item.Data.itemType == ITEMTYPE.USABLE || item.Data.itemType == ITEMTYPE.WEARABLE)
                {
                    statsPanel.SetActive(true);

                    //gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 135);
                    this.damage.text = item.Data.damage.ToString();

                    this.speed.text = item.Data.Speed.ToString();
                    this.strength.text = item.Data.Strength.ToString();
                    this.health.text = item.Data.health.ToString();
                    this.stamina.text = item.Data.stamina.ToString();
                    this.mana.text = item.Data.mana.ToString();
                    this.armor.text = item.Data.armor.ToString();
                    this.IQ.text = item.Data.IQ.ToString();
                    this.dexterity.text = item.Data.Dexterity.ToString();
                    this.itemID.text = item.Data.id.ToString();
                    this.ItemType.text = ((ITEMTYPE)item.Data.itemType).ToString();
                    this.SlotType.text = item.Data.slotType.ToString();
                    this.BaseMaterial.text = ((MATERIALTYPE)item.Data.BaseMaterial).ToString();
                    // set durability
                    this.dSlider.enabled = true;
                    // this.durability.text = item.Durability.ToString() + "/" + item.Data.maxDurability.ToString();
                    this.dSlider.maxValue = item.Data.maxDurability;
                    this.dSlider.value = item.Durability;

                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}