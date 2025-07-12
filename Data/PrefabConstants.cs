using Bloody.Core.Helper.v1;
using Stunlock.Core;

namespace BloodyBoss.Data
{
    /// <summary>
    /// Central location for all PrefabGUID constants used in BloodyBoss.
    /// This class uses BloodyCore's Prefabs to avoid hardcoded values.
    /// </summary>
    public static class PrefabConstants
    {
        // Buff Effects - Status (Using real game prefabs)
        public static readonly PrefabGUID PhysicalWeakness = Prefabs.Buff_General_Weaken;
        public static readonly PrefabGUID SpellWeakness = Prefabs.Buff_General_SpellBlock;
        public static readonly PrefabGUID ArmorReduction = Prefabs.Buff_General_Weaken_Cursed_Ghosts;
        public static readonly PrefabGUID HealingReduction = Prefabs.Illusion_Vampire_Buff_Weaken;
        public static readonly PrefabGUID Fear = Prefabs.Buff_General_Fear;
        public static readonly PrefabGUID Stun = Prefabs.Buff_General_Stun;
        public static readonly PrefabGUID StunImpact = Prefabs.Buff_General_Stun_ImpactFX;
        public static readonly PrefabGUID Slow = Prefabs.Buff_General_Slow;

        // Visual Effects - Marking
        public static readonly PrefabGUID FloatingEyeMark = Prefabs.AB_Militia_HoundMaster_QuickShot_Buff;
        public static readonly PrefabGUID HolyBeamTarget = Prefabs.AB_Militia_BishopOfDunley_HolyBeam_TargetBuff_Buff;
        public static readonly PrefabGUID HolyNuke = Prefabs.AB_Paladin_HolyNuke_Buff;

        // Shield and Absorption
        public static readonly PrefabGUID BloodRageShield = Prefabs.AB_Blood_BloodRage_SpellMod_Buff_Shield;
        public static readonly PrefabGUID VampireLeechHeal = Prefabs.Blood_Vampire_Buff_Leech_SelfHeal;

        // DoT Effects (Using real game prefabs)
        public static readonly PrefabGUID PoisonDoT = Prefabs.Buff_General_Poison;
        public static readonly PrefabGUID BurnDoT = Prefabs.Buff_General_Ignite;
        public static readonly PrefabGUID BleedDoT = Prefabs.AB_BatVampire_BatSwarm_BleedBuff;
        public static readonly PrefabGUID CorruptionDoT = Prefabs.Buff_General_Corruption_Area_Debuff_T01;
        public static readonly PrefabGUID FrostDoT = Prefabs.AB_Manticore_Frost_Debuff;
        // Trap Types
        public static readonly PrefabGUID SpikeTrap = new PrefabGUID(1901522191);
        public static readonly PrefabGUID FireTrap = new PrefabGUID(-1426222885);
        public static readonly PrefabGUID IceTrap = new PrefabGUID(-355466479);
        public static readonly PrefabGUID PoisonTrap = new PrefabGUID(652614258);
        public static readonly PrefabGUID ExplosiveTrap = new PrefabGUID(-1248239739);
        // Vision Effects (Using real game prefabs)
        public static readonly PrefabGUID Darkness = Prefabs.Buff_General_Fear;
        public static readonly PrefabGUID Blur = Prefabs.Buff_General_Daze;
        public static readonly PrefabGUID Hallucination = Prefabs.AB_Illusion_Curse_Debuff;
        public static readonly PrefabGUID Fog = Prefabs.Buff_General_Garlic_Area_Inside;
        public static readonly PrefabGUID ScreenDistortion = new PrefabGUID(-106264639); // Keep custom
        // Mechanic Effects (Using real game prefabs where possible)
        public static readonly PrefabGUID IllusionVisual = new PrefabGUID(-1464851863); // Keep custom
        public static readonly PrefabGUID Cleanse = Prefabs.SpellMod_Shared_DispellDebuffs;
        public static readonly PrefabGUID CleanseVisual = new PrefabGUID(-1044788755); // Keep custom
        public static readonly PrefabGUID DispelEffect = Prefabs.SpellMod_Shared_DispellDebuffs;
        public static readonly PrefabGUID MindControl = Prefabs.AB_Charm_Active_Human_Buff;
        public static readonly PrefabGUID Confusion = Prefabs.AB_Illusion_Curse_Debuff;
        public static readonly PrefabGUID SpeedBoost = Prefabs.Buff_General_Haste;
        public static readonly PrefabGUID PurpleGlow = new PrefabGUID(-893140707); // Keep custom
        // Root Types (Using real game prefabs)
        public static readonly PrefabGUID VineRoot = Prefabs.Buff_General_Entangled;
        public static readonly PrefabGUID IceRoot = Prefabs.Buff_General_Freeze;
        public static readonly PrefabGUID ChainsRoot = Prefabs.Buff_General_Immobilize;
        public static readonly PrefabGUID BloodRoot = Prefabs.AB_Blood_HeartStrike_Debuff;
        // Silence Types (Using real game prefabs)
        public static readonly PrefabGUID Silence = Prefabs.Buff_General_Silence;
        public static readonly PrefabGUID ArcaneDisruption = Prefabs.Buff_General_SpellBlockSilence;
        public static readonly PrefabGUID PowerBlock = new PrefabGUID(-1430666906); // Keep custom
        // Pull Effects
        public static readonly PrefabGUID VacuumPull = new PrefabGUID(-1450903252);
        public static readonly PrefabGUID ChainPull = new PrefabGUID(910327738);
        public static readonly PrefabGUID MagneticPull = new PrefabGUID(-893140707);
        // Knockback Effects
        public static readonly PrefabGUID ExplosiveKnockback = new PrefabGUID(1519158884);
        public static readonly PrefabGUID WindKnockback = new PrefabGUID(-1574840929);
        public static readonly PrefabGUID ShockwaveKnockback = new PrefabGUID(1519158884);
        // Shield Types
        public static readonly PrefabGUID MagicShield = Prefabs.AB_Monster_LightningShield_Buff;
        public static readonly PrefabGUID HolyShield = Prefabs.AB_ChurchOfLight_Paladin_HolyBubble_BeamSpawnerBuff;
        public static readonly PrefabGUID ElementalShield = Prefabs.AB_Unholy_Soulburn_SpellMod_ConsumeSkeletonEmpowerBuff;
        public static readonly PrefabGUID BloodShield = Prefabs.AB_Blood_BloodRage_SpellMod_Buff_Shield;

        // Heal Types
        public static readonly PrefabGUID InstantHeal = new PrefabGUID(1411255462);
        public static readonly PrefabGUID LifeDrain = Prefabs.Blood_Vampire_Buff_Leech_SelfHeal;
        public static readonly PrefabGUID RegenerationHeal = new PrefabGUID(1035222161);
        public static readonly PrefabGUID HolyHeal = new PrefabGUID(-108025589);
        // Curse Types
        public static readonly PrefabGUID Weakness = Prefabs.Illusion_Vampire_Buff_Weaken;
        public static readonly PrefabGUID Doom = Prefabs.AB_Vampire_Dracula_RingOfBlood_ChannelBuff;
        public static readonly PrefabGUID BadLuck = new PrefabGUID(451158128);
        public static readonly PrefabGUID ElementalCurse = new PrefabGUID(-2064870933);
        // Enrage Visual Types
        public static readonly PrefabGUID BloodRageVisual = Prefabs.AB_Blood_BloodRage_Buff_MagicSource;
        public static readonly PrefabGUID FireAura = new PrefabGUID(-1576893213);
        public static readonly PrefabGUID SpeedLines = new PrefabGUID(788443104);
        // Reflect Effects
        public static readonly PrefabGUID MagicReflect = new PrefabGUID(-1693243738);
        public static readonly PrefabGUID PhysicalReflect = Prefabs.AB_Blood_BloodRage_SpellMod_Buff_Shield;
        public static readonly PrefabGUID ThornsReflect = new PrefabGUID(-149089805);
        // Other Effects
        public static readonly PrefabGUID WarningCircle = new PrefabGUID(405284549);
        public static readonly PrefabGUID PhaseShift = new PrefabGUID(1313008051);
        public static readonly PrefabGUID BuffSteal = new PrefabGUID(1183622977);
    }
}