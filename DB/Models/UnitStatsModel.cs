using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodyBoss.DB.Models
{
    internal class UnitStatsModel
    {
        public float PhysicalCriticalStrikeChance { get; set; }
        public float PhysicalCriticalStrikeDamage { get; set; }
        public float SpellCriticalStrikeChance { get; set; }
        public float SpellCriticalStrikeDamage { get; set; }
        public float PhysicalPower { get; set; }
        public float SpellPower { get; set; }
        public float ResourcePower { get; set; }
        public float SiegePower { get; set; }
        public float ResourceYieldModifier { get; set; }
        public float ReducedResourceDurabilityLoss { get; set; }
        public float PhysicalResistance { get; set; }
        public float SpellResistance { get; set; }
        public int SunResistance { get; set; }
        public int FireResistance { get; set; }
        public int HolyResistance { get; set; }
        public int SilverResistance { get; set; }
        public int SilverCoinResistance { get; set; }
        public int GarlicResistance { get; set; }
        public float PassiveHealthRegen { get; set; }
        public int CCReduction { get; set; }
        public float HealthRecovery { get; set; }
        public float DamageReduction { get; set; }
        public float HealingReceived { get; set; }
        public float ShieldAbsorbModifier { get; set; }
        public float BloodEfficiency { get; set; }

        public void SetStats(UnitStats stats)
        {
            PhysicalCriticalStrikeChance = stats.PhysicalCriticalStrikeChance.Value;
            PhysicalCriticalStrikeDamage = stats.PhysicalCriticalStrikeDamage.Value;
            SpellCriticalStrikeChance = stats.SpellCriticalStrikeChance.Value;
            SpellCriticalStrikeDamage = stats.SpellCriticalStrikeDamage.Value;
            PhysicalPower = stats.PhysicalPower.Value;
            SpellPower = stats.SpellPower.Value;
            ResourcePower = stats.ResourcePower.Value;
            SiegePower = stats.SiegePower.Value;
            ResourceYieldModifier = stats.ResourceYieldModifier.Value;
            ReducedResourceDurabilityLoss = stats.ReducedResourceDurabilityLoss.Value;
            PhysicalResistance = stats.PhysicalResistance.Value;
            SpellResistance = stats.SpellResistance.Value;
            SunResistance = stats.SunResistance.Value;
            FireResistance = stats.FireResistance.Value;
            HolyResistance = stats.HolyResistance.Value;
            SilverResistance = stats.SilverResistance.Value;
            SilverCoinResistance = stats.SilverCoinResistance.Value;
            GarlicResistance = stats.GarlicResistance.Value;
            PassiveHealthRegen = stats.PassiveHealthRegen.Value;
            CCReduction = stats.CCReduction.Value;
            HealthRecovery = stats.HealthRecovery.Value;
            DamageReduction = stats.DamageReduction.Value;
            HealingReceived = stats.HealingReceived.Value;
            ShieldAbsorbModifier = stats.ShieldAbsorbModifier.Value;
            BloodEfficiency = stats.BloodEfficiency.Value;
        }

        public UnitStats FillStats(UnitStats stats)
        {
            stats.PhysicalCriticalStrikeChance._Value = PhysicalCriticalStrikeChance;
            stats.PhysicalCriticalStrikeDamage._Value = PhysicalCriticalStrikeDamage;
            stats.SpellCriticalStrikeChance._Value = SpellCriticalStrikeChance;
            stats.SpellCriticalStrikeDamage._Value = SpellCriticalStrikeDamage;
            stats.PhysicalPower._Value = PhysicalPower;
            stats.SpellPower._Value = SpellPower;
            stats.ResourcePower._Value = ResourcePower;
            stats.SiegePower._Value = SiegePower;
            stats.ResourceYieldModifier._Value = ResourceYieldModifier;
            stats.ReducedResourceDurabilityLoss._Value = ReducedResourceDurabilityLoss;
            stats.PhysicalResistance._Value = PhysicalResistance;
            stats.SpellResistance._Value = SpellResistance;
            stats.SunResistance._Value = SunResistance;
            stats.FireResistance._Value = FireResistance;
            stats.HolyResistance._Value = HolyResistance;
            stats.SilverResistance._Value = SilverResistance;
            stats.SilverCoinResistance._Value = SilverCoinResistance;
            stats.GarlicResistance._Value = GarlicResistance;
            stats.PassiveHealthRegen._Value = PassiveHealthRegen;
            stats.CCReduction._Value = CCReduction;
            stats.HealthRecovery._Value = HealthRecovery;
            stats.DamageReduction._Value = DamageReduction;
            stats.HealingReceived._Value = HealingReceived;
            stats.ShieldAbsorbModifier._Value = ShieldAbsorbModifier;
            stats.BloodEfficiency._Value = BloodEfficiency;

            return stats;
        }
    }
}
