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
        public Text damage;
        public Text durability;
        public Text speed;
        public Text health;
        public Text stamina;
        public Text mana;
        public Text armor;
        public Text intelligencs;
        public Text dexterity;
        public Text luck;
        public Slider dSlider;

        /// <summary>
        /// method to display the item info box
        /// </summary>
        /// <param name="item"></param>
        public void Show (Item item)
        {
            if (item != null)
            {
                gameObject.SetActive(true);
                this.itemName.text = item.data.name;
                this.description.text = item.data.description;

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

                this.image.sprite = item.data.sprite;

                if (item.data.itemType == ITEMTYPE.CONSUMABLE)
                {
                    this.dSlider.enabled = false;
                    gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 110);
                    statsPanel.SetActive(false);
                }
                else if (item.data.itemType == ITEMTYPE.USABLE || item.data.itemType == ITEMTYPE.WEARABLE)
                {
                    statsPanel.SetActive(true);

                    gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 135);
                    this.damage.text = item.data.damage.ToString();

                    this.speed.text = item.data.speed.ToString();
                    this.health.text = item.data.health.ToString();
                    this.stamina.text = item.data.stamina.ToString();
                    this.mana.text = item.data.mana.ToString();
                    this.armor.text = item.data.armor.ToString();
                    this.intelligencs.text = item.data.intelligence.ToString();
                    this.dexterity.text = item.data.dexterity.ToString();

                    this.dSlider.enabled = true;
                    this.durability.text = item.durability.ToString() + "/" + item.data.maxDurability.ToString();
                    this.dSlider.maxValue = item.data.maxDurability;
                    this.dSlider.value = item.durability;

                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}