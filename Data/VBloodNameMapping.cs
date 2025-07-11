using System.Collections.Generic;

namespace BloodyBoss.Data
{
    public static class VBloodNameMapping
    {
        // Mapeo de GUID a nombre amigable basado en https://wiki.vrisingmods.com/prefabs/VBloodNames
        private static readonly Dictionary<int, string> _guidToFriendlyName = new Dictionary<int, string>
        {
            // Act 1 VBloods
            [-1905691330] = "Alpha the White Wolf",
            [-1365931036] = "Errol the Stonebreaker",
            [-2025101517] = "Keely the Frost Archer",
            [2122229952] = "Rufus the Foreman",
            [-1659822956] = "Grayson the Armourer",
            [1106149033] = "Goreswine the Ravager",
            [-577221462] = "Lidia the Chaos Archer",
            [763273073] = "Putrid Rat",
            [-1391546313] = "Clive the Firestarter",
            [-1464869978] = "Polora the Feywalker",
            [-1208888966] = "Nicholaus the Fallen",
            [-1007062401] = "Quincey the Bandit King",
            [-1449631170] = "Beatrice the Tailor",
            [613251918] = "Vincent the Frostbringer",
            [-1968372384] = "Christina the Sun Priestess",
            [-99012450] = "Tristan the Vampire Hunter",
            
            // Act 2 VBloods
            [-29797003] = "Terah the Geomancer",
            [1688478381] = "Meredith the Bright Archer",
            [-680831417] = "Octavian the Militia Captain",
            [-548489519] = "Raziel the Shepherd",
            [-1942352521] = "Ungora the Spider Queen",
            [-484556888] = "The Duke of Balaton",
            [-1936575244] = "Jade the Vampire Hunter",
            [939467639] = "Foulrot the Soultaker",
            [114912615] = "Willfred the Werewolf Chief",
            [-1347412392] = "Mairwyn the Elementalist",
            [-910296704] = "Morian the Stormwing Matriarch",
            [-2013903325] = "Azariel the Sunbringer",
            
            // Act 3 VBloods
            [-393555055] = "Matka the Curse Weaver",
            [153390636] = "Voltatia the Power Master",
            [-1065970933] = "Nightmarshal Styx the Sunderer",
            [1112948824] = "Gorecrusher the Behemoth",
            [-1007062401] = "The Winged Horror",
            [-740796338] = "Solarus the Immaculate",
            
            // Gloomrot VBloods
            [106480588] = "Adam the Firstborn",
            [-910296704] = "Angram the Purifier",
            [-1936575244] = "Ziva the Engineer",
            [-548489519] = "Henry Blackbrew the Doctor",
            [-680831417] = "Domina the Blade Dancer",
            
            // Additional mappings from the database that might have different names
            [814083983] = "Bane the Shadowblade",
            [685266977] = "Cyril the Cursed Smith",
            [326378955] = "Errol the Stonebreaker",
            [-1391546313] = "Grayson the Armorer",
            [1106149033] = "Goreswine the Ravager",
            [-2122682556] = "Kodia the Ferocious Bear",
            [-1779164929] = "Leandra the Shadow Priestess",
            [-203043163] = "Lidia the Chaos Archer",
            [850622034] = "Magnus the Overlord",
            [24378719] = "Mairwyn the Elementalist",
            [-435912559] = "Matka the Curse Weaver",
            [-548489519] = "Maja the Dark Savant",
            [-29797003] = "Meredith the Bright Archer",
            [-910296704] = "Morian the Stormwing Matriarch",
            [-99012450] = "Nicholaus the Fallen",
            [-680831417] = "Octavian the Militia Captain",
            [-1464869978] = "Polora the Feywalker",
            [763273073] = "Putrid Rat",
            [-1234481170] = "Quincey the Bandit King",
            [-548489519] = "Raziel the Shepherd",
            [2122229952] = "Rufus the Foreman",
            [-1065970933] = "Sir Magnus the Overseer",
            [-203043163] = "Terah the Geomancer",
            [-1659822956] = "The Duke of Balaton",
            [-327335305] = "The Monster",
            [161318849] = "The Winged Horror",
            [-99012450] = "Tristan the Vampire Hunter",
            [-1942352521] = "Ungora the Spider Queen",
            [613251918] = "Vincent the Frostbringer",
            [1688478381] = "Voltatia the Power Master",
            [114912615] = "Willfred the Werewolf Chief"
        };

        // Mapeo inverso para búsquedas rápidas
        private static readonly Dictionary<string, int> _friendlyNameToGuid = new Dictionary<string, int>();

        static VBloodNameMapping()
        {
            // Construir el mapeo inverso
            foreach (var kvp in _guidToFriendlyName)
            {
                var name = kvp.Value.ToLower();
                if (!_friendlyNameToGuid.ContainsKey(name))
                {
                    _friendlyNameToGuid[name] = kvp.Key;
                }
            }
        }

        public static string GetFriendlyName(int guidHash)
        {
            return _guidToFriendlyName.TryGetValue(guidHash, out var name) ? name : null;
        }

        public static int? GetGuidFromFriendlyName(string friendlyName)
        {
            var lowerName = friendlyName.ToLower();
            return _friendlyNameToGuid.TryGetValue(lowerName, out var guid) ? guid : (int?)null;
        }

        public static Dictionary<string, int> GetAllFriendlyNames()
        {
            var result = new Dictionary<string, int>();
            foreach (var kvp in _guidToFriendlyName)
            {
                result[kvp.Value] = kvp.Key;
            }
            return result;
        }
    }
}