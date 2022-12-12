using System.Collections;
using System.Collections.Generic;
using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormDreams
{
    public class LobbyView : View
    {
        [SerializeField]
        private Button _readyButton;
        [SerializeField]
        private Button _exitButton;

        [SerializeField]
        private Button _skillOneSlotButton;
        [SerializeField]
        private Button _skillTwoSlotButton;
        [SerializeField]
        private Image _skillOneSlotIcon;
        [SerializeField]
        private Image _skillTwoSlotIcon;
        [SerializeField]
        private TMP_Text _skillOneSlotName;
        [SerializeField]
        private TMP_Text _skillTwoSlotName;

        [SerializeField]
        private GameObject _skillListPanel;
        [SerializeField]
        private Transform _skillListContentTransform;

        private List<GameObject> _skillListItems = new();
        private int _selectedSlot = 0;

        public override void Initialize()
        {
            _readyButton.onClick.AddListener(() =>
            {
                Player.Instance.RpcToggleReadyState();
            });

            _exitButton.onClick.AddListener(() =>
            {
                if (InstanceFinder.IsClient)
                {
                    InstanceFinder.ClientManager.StopConnection();
                }

                if (InstanceFinder.IsServer)
                {
                    InstanceFinder.ServerManager.StopConnection(true);
                }
            });

            if (InstanceFinder.IsClient)
            {
                _skillOneSlotButton.onClick.AddListener(() => OnClickSkillSlot(1));
                _skillTwoSlotButton.onClick.AddListener(() => OnClickSkillSlot(2));

                UpdateSkillSlots();

                InitializeSkillList();
            }

            base.Initialize();
        }

        public void UpdateSkillSlots()
        {           
            _skillOneSlotIcon.sprite = ResourceManager.Instance.Skills[Player.Instance.SkillOneName].Icon;
            _skillTwoSlotIcon.sprite = ResourceManager.Instance.Skills[Player.Instance.SkillTwoName].Icon;
            _skillOneSlotName.text = ResourceManager.Instance.Skills[Player.Instance.SkillOneName].Name;
            _skillTwoSlotName.text = ResourceManager.Instance.Skills[Player.Instance.SkillTwoName].Name;
        }

        private void InitializeSkillList()
        {
            _skillListItems.Clear();
            foreach (KeyValuePair<string, Skill> skill in ResourceManager.Instance.Skills)
            {
                GameObject skillListItem = Instantiate(ResourceManager.Instance.SkillListItemPrefab, _skillListContentTransform);
                skillListItem.GetComponent<Image>().sprite = skill.Value.Icon;
                skillListItem.GetComponent<Button>().onClick.AddListener(() => OnClickSkillListItem(skill.Key));
                _skillListItems.Add(skillListItem);
            }
        }

        private void UpdateSkillList()
        {
            if (Player.Instance.SkillOneName == null && Player.Instance.SkillTwoName == null)
            {
                return;
            }

            foreach (GameObject skillListItem in _skillListItems)
            {
                Sprite skillListItemIcon = skillListItem.GetComponent<Image>().sprite;
                Button skillListItemButton = skillListItem.GetComponent<Button>();

                if (skillListItemIcon == ResourceManager.Instance.Skills[Player.Instance.SkillOneName].Icon
                    || skillListItemIcon == ResourceManager.Instance.Skills[Player.Instance.SkillTwoName].Icon)
                {
                    skillListItemButton.interactable = false;
                }
                else
                {
                    skillListItemButton.interactable = true;
                }
            }
        }

        private void OnClickSkillSlot(int slot)
        {
            if (slot != 1 && slot != 2)
            {
                return;
            }

            _selectedSlot = slot;

            UpdateSkillList();

            _skillListPanel.SetActive(true);
        }

        private void OnClickSkillListItem(string skillName)
        {
            if (_selectedSlot == 0 || string.IsNullOrEmpty(skillName))
            {
                return;
            }

            Player.Instance.RpcSetPlayerSkill(_selectedSlot, skillName);

            _skillListPanel.SetActive(false);
        }
    }
}
