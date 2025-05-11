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
        public float PhysicalPower { get; set; }
        public float SpellPower { get; set; }
        public float ResourcePower { get; set; }
        public float SiegePower { get; set; }
        public float PhysicalResistance { get; set; }
        public float SpellResistance { get; set; }
        public int FireResistance { get; set; }
        public float PassiveHealthRegen { get; set; }
        public int CCReduction { get; set; }
        public float HealthRecovery { get; set; }
        public float DamageReduction { get; set; }
        public float HealingReceived { get; set; }

        public void SetStats(UnitStats stats)
        {
            PhysicalPower = stats.PhysicalPower.Value;
            SpellPower = stats.SpellPower.Value;
            ResourcePower = stats.ResourcePower.Value;
            SiegePower = stats.SiegePower.Value;
            PhysicalResistance = stats.PhysicalResistance.Value;
            SpellResistance = stats.SpellResistance.Value;
            FireResistance = stats.FireResistance.Value;
            PassiveHealthRegen = stats.PassiveHealthRegen.Value;
            CCReduction = stats.CCReduction.Value;
            HealthRecovery = stats.HealthRecovery.Value;
            DamageReduction = stats.DamageReduction.Value;
            HealingReceived = stats.HealingReceived.Value;
        }

        public UnitStats FillStats(UnitStats stats)
        {
            stats.PhysicalPower._Value = PhysicalPower;
            stats.SpellPower._Value = SpellPower;
            stats.ResourcePower._Value = ResourcePower;
            stats.SiegePower._Value = SiegePower;
            stats.PhysicalResistance._Value = PhysicalResistance;
            stats.SpellResistance._Value = SpellResistance;
            stats.FireResistance._Value = FireResistance;
            stats.PassiveHealthRegen._Value = PassiveHealthRegen;
            stats.CCReduction._Value = CCReduction;
            stats.HealthRecovery._Value = HealthRecovery;
            stats.DamageReduction._Value = DamageReduction;
            stats.HealingReceived._Value = HealingReceived;

            return stats;
        }
    }
}
