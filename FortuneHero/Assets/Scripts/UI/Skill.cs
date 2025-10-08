using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.GPUSort;

public class Skill : MonoBehaviour
{
    [SerializeField] SkillComponent skillComponent;


    [SerializeField]  TMP_Text skillPointsText;

    [SerializeField]  Button addAttackBtn;
    [SerializeField]  Button removeAttackBtn;
    [SerializeField]  TMP_Text attackUsedText;
    [SerializeField]  TMP_Text attackValueText;

    //[SerializeField] Button addDefenseBtn;
    //[SerializeField]  Button removeDefenseBtn;
    //[SerializeField]  TMP_Text defenseUsedText;
    //[SerializeField]  TMP_Text defenseValueText;

    //[SerializeField]  Button addHPBtn;
    //[SerializeField]  Button removeHPBtn;
    //[SerializeField]  TMP_Text hpUsedText;
    //[SerializeField]  TMP_Text hpValueText;

    //[SerializeField]  Button addDashBtn;
    //[SerializeField]  Button removeDashBtn;
    //[SerializeField]  TMP_Text dashUsedText;
    //[SerializeField]  TMP_Text dashValueText;

    //[SerializeField]  Button addSpeedBtn;
    //[SerializeField]  Button removeSpeedBtn;
    //[SerializeField]  TMP_Text speedUsedText;
    //[SerializeField]  TMP_Text speedValueText;

    //[SerializeField]  Button addDistanceBtn;
    //[SerializeField]  Button removeDistanceBtn;
    //[SerializeField]  TMP_Text distanceUsedText;
    //[SerializeField]  TMP_Text distanceValueText;

    void OnEnable()
    {
        // --- Attaque (Melee) ---
        addAttackBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.MeleeAtkPlus, false); RefreshUI(); });
        removeAttackBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.MeleeAtkPlus, true); RefreshUI(); });

        // --- Défense ---
        //addDefenseBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.ShieldBlockTime, false); RefreshUI(); });
        //removeDefenseBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.ShieldBlockTime, true); RefreshUI(); });

        //// --- HP ---
        //addHPBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.MaxHealthPlus, false); RefreshUI(); });
        //removeHPBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.MaxHealthPlus, true); RefreshUI(); });

        //// --- Dash ---
        //addDashBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.DashCooldownMinus, false); RefreshUI(); });
        //removeDashBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.DashCooldownMinus, true); RefreshUI(); });

        //// --- Speed ---
        //addSpeedBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.SpeedPlus, false); RefreshUI(); });
        //removeSpeedBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.SpeedPlus, true); RefreshUI(); });

        //// --- Distance ---
        //addDistanceBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.RangeAtkPlus, false); RefreshUI(); });
        //removeDistanceBtn.onClick.AddListener(() => { skillComponent.ChooseSkill(SkillComponent.SkillType.RangeAtkPlus, true); RefreshUI(); });

        RefreshUI();
    }

    

    private void RefreshUI()
    {
        // Points restants
        skillPointsText.text = $" {skillComponent.skillPoints}";

        // --- Melee ---
        attackUsedText.text = $"Utilisés: {GetBuys(SkillComponent.SkillType.MeleeAtkPlus)}";
        attackValueText.text = $"ATK+: {skillComponent.meleeAtkBonus:F1}";

        // --- Défense ---
        //defenseUsedText.text = $"Utilisés: {GetBuys(SkillComponent.SkillType.ShieldBlockTime)}";
        //defenseValueText.text = $"Durée blocage: {skillComponent.shieldBlockTimeBonus:F1}s";

        //// --- HP ---
        //hpUsedText.text = $"Utilisés: {GetBuys(SkillComponent.SkillType.MaxHealthPlus)}";
        //hpValueText.text = $"HP+: {skillComponent.maxHealthBonus}";

        //// --- Dash ---
        //dashUsedText.text = $"Utilisés: {GetBuys(SkillComponent.SkillType.DashCooldownMinus)}";
        //dashValueText.text = $"CD dash-: {skillComponent.dashCooldownReduction:F1}s";

        //// --- Speed ---
        //speedUsedText.text = $"Utilisés: {GetBuys(SkillComponent.SkillType.SpeedPlus)}";
        //speedValueText.text = $"Vitesse+: {skillComponent.speedBonus:F1}";

        //// --- Distance ---
        //distanceUsedText.text = $"Utilisés: {GetBuys(SkillComponent.SkillType.RangeAtkPlus)}";
        //distanceValueText.text = $"ATK Dist+: {skillComponent.rangeAtkBonus:F1}";
    }

    // Fonction pour accéder au nombre d’achats internes
    private int GetBuys(SkillComponent.SkillType type)
    {
        return skillComponent.GetBuys(type);
    }
}
