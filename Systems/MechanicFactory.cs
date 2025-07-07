using System;
using System.Collections.Generic;
using BloodyBoss.Systems.Mechanics;

namespace BloodyBoss.Systems
{
    internal static class MechanicFactory
    {
        private static readonly Dictionary<string, IMechanic> _mechanics = new Dictionary<string, IMechanic>();

        static MechanicFactory()
        {
            // Register all available mechanics (25 total)
            RegisterMechanic(new EnrageMechanic());
            RegisterMechanic(new ShieldMechanic());
            RegisterMechanic(new SummonMechanic());
            RegisterMechanic(new HealMechanic());
            RegisterMechanic(new TeleportMechanic());
            RegisterMechanic(new PhaseMechanic());
            RegisterMechanic(new AoeMechanic());
            RegisterMechanic(new StunMechanic());
            RegisterMechanic(new FearMechanic());
            RegisterMechanic(new SlowMechanic());
            RegisterMechanic(new PullMechanic());
            RegisterMechanic(new CloneMechanic());
            RegisterMechanic(new RootMechanic());
            RegisterMechanic(new SilenceMechanic());
            RegisterMechanic(new KnockbackMechanic());
            RegisterMechanic(new DotMechanic());
            RegisterMechanic(new BuffStealMechanic());
            RegisterMechanic(new VisionMechanic());
            RegisterMechanic(new MindControlMechanic());
            RegisterMechanic(new ReflectMechanic());
            RegisterMechanic(new AbsorbMechanic());
            RegisterMechanic(new DispelMechanic());
            RegisterMechanic(new WeakenMechanic());
            RegisterMechanic(new CurseMechanic());
            RegisterMechanic(new TrapMechanic());
        }

        private static void RegisterMechanic(IMechanic mechanic)
        {
            _mechanics[mechanic.Type.ToLower()] = mechanic;
        }

        public static IMechanic GetMechanic(string type)
        {
            if (_mechanics.TryGetValue(type.ToLower(), out var mechanic))
            {
                return mechanic;
            }
            return null;
        }

        public static List<string> GetAvailableMechanics()
        {
            return new List<string>(_mechanics.Keys);
        }
    }
}