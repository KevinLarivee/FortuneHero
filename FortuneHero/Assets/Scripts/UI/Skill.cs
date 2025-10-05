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

    [SerializeField] Button addDefenseBtn;
    [SerializeField]  Button removeDefenseBtn;
    [SerializeField]  TMP_Text defenseUsedText;
    [SerializeField]  TMP_Text defenseValueText;

    [SerializeField]  Button addHPBtn;
    [SerializeField]  Button removeHPBtn;
    [SerializeField]  TMP_Text hpUsedText;
    [SerializeField]  TMP_Text hpValueText;

    [SerializeField]  Button addDashBtn;
    [SerializeField]  Button removeDashBtn;
    [SerializeField]  TMP_Text dashUsedText;
    [SerializeField]  TMP_Text dashValueText;

    [SerializeField]  Button addSpeedBtn;
    [SerializeField]  Button removeSpeedBtn;
    [SerializeField]  TMP_Text speedUsedText;
    [SerializeField]  TMP_Text speedValueText;

    [SerializeField]  Button addDistanceBtn;
    [SerializeField]  Button removeDistanceBtn;
    [SerializeField]  TMP_Text distanceUsedText;
    [SerializeField]  TMP_Text distanceValueText;

     void OnEnable()
    {
        addAttackBtn.onClick.AddListener(() => { if (skillComponent.TryBuySkill(SkillComponent.SkillType.MeleeAtkPlus)) RefreshUI(); });
        //removeAttackBtn.onClick.AddListener(() => { if (skillComponent.TryRemoveSkill(SkillComponent.SkillType.MeleeAtkPlus)) RefreshUI(); });

        addDefenseBtn.onClick.AddListener(() => { if (skillComponent.TryBuySkill(SkillComponent.SkillType.ShieldBlockTime)) RefreshUI(); });
        //removeDefenseBtn.onClick.AddListener(() => { if (skillComponent.TryRemoveSkill(SkillComponent.SkillType.ShieldBlockTime)) RefreshUI(); });

        //addHPBtn.onClick.AddListener(() => { if (skillComponent.TryBuySkill(SkillComponent.SkillType.MaxHP)) RefreshUI(); });
        //removeHPBtn.onClick.AddListener(() => { if (skillComponent.TryRemoveSkill(SkillComponent.SkillType.MaxHP)) RefreshUI(); });

        //addDashBtn.onClick.AddListener(() => { if (skillComponent.TryBuySkill(SkillComponent.SkillType.DashCooldown)) RefreshUI(); });
        //removeDashBtn.onClick.AddListener(() => { if (skillComponent.TryRemoveSkill(SkillComponent.SkillType.DashCooldown)) RefreshUI(); });

        //addSpeedBtn.onClick.AddListener(() => { if (skillComponent.TryBuySkill(SkillComponent.SkillType.Speed)) RefreshUI(); });
        //removeSpeedBtn.onClick.AddListener(() => { if (skillComponent.TryRemoveSkill(SkillComponent.SkillType.Speed)) RefreshUI(); });

        //addDistanceBtn.onClick.AddListener(() => { if (skillComponent.TryBuySkill(SkillComponent.SkillType.DistanceAtkPlus)) RefreshUI(); });
        //removeDistanceBtn.onClick.AddListener(() => { if (skillComponent.TryRemoveSkill(SkillComponent.SkillType.DistanceAtkPlus)) RefreshUI(); });

        RefreshUI();
    }

    private void RefreshUI()
    {
        skillPointsText.text = $"Points restants: {skillComponent.skillPoints}";

        //attackUsedText.text = $"Utilisés: {skillComponent.G(SkillComponent.SkillType.MeleeAtkPlus)}";
        //attackValueText.text = $"ATK: {skillComponent.currentAttack}";

        //defenseUsedText.text = $"Utilisés: {skillComponent.GetUsedPoints(SkillComponent.SkillType.ShieldBlockTime)}";
        //defenseValueText.text = $"DEF: {skillComponent.currentDefense:F1}";

        //hpUsedText.text = $"Utilisés: {skillComponent.GetUsedPoints(SkillComponent.SkillType.MaxHealthPlus)}";
        //hpValueText.text = $"HP: {skillComponent.currentHP}";

        //dashUsedText.text = $"Utilisés: {skillComponent.GetUsedPoints(SkillComponent.SkillType.DashCooldownMinus)}";
        //dashValueText.text = $"DASH CD: {skillComponent.currentDashCd:F1}s";

        //speedUsedText.text = $"Utilisés: {skillComponent.GetUsedPoints(SkillComponent.SkillType.SpeedPlus)}";
        //speedValueText.text = $"VIT: {skillComponent.currentSpeed}%";

        //distanceUsedText.text = $"Utilisés: {skillComponent.GetUsedPoints(SkillComponent.SkillType.DistanceAtkPlus)}";
        //distanceValueText.text = $"DIST: {skillComponent.currentDistanceAtk}";
    }
}
