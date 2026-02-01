using Manager.Status;
using SharedUI.HUD.InGameTime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SharedUI.HUD.Infection
{
    public class InfectionRepElementHUD : MonoBehaviour
    {
        [SerializeField] Image infectionRepImage;
        [SerializeField] TMP_Text infectionNameText;
        [SerializeField] MinutesTillNextInfectionPb infectionRepProgressBar;

        public void SetNewInfection(InfectionManager.OngoingInfection infectionInfo)
        {
            infectionNameText.text = infectionInfo.infectionName;
            infectionRepProgressBar.UpdateUI(infectionInfo.progressionTowardSupplantation, 1f);
            switch (infectionInfo.infectionSiteID)
            {
                case "Skin01":
                    infectionRepImage.sprite = InfectionManager.Instance.iconRepository.skinIcon;
                    break;
                case "Eyes01":
                    infectionRepImage.sprite = InfectionManager.Instance.iconRepository.eyesIcon;
                    break;
                case "Lungs01":
                    infectionRepImage.sprite = InfectionManager.Instance.iconRepository.lungsIcon;
                    break;
                case "Heart01":
                    infectionRepImage.sprite = InfectionManager.Instance.iconRepository.heartIcon;
                    break;
                case "Brain01":
                    infectionRepImage.sprite = InfectionManager.Instance.iconRepository.brainIcon;
                    break;
            }
        }
    }
}
