/*
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "creature_protos", DatabaseName = "World")]
    [Serializable]
    public class Creature_proto : DataObject
    {
        private uint _Entry;
        private string _Name;
        private UInt16 _Model1;
        private UInt16 _Model2;
        private UInt16 _MinScale;
        private UInt16 _MaxScale;
        private byte _MinLevel;
        private byte _MaxLevel;
        private byte _Faction;
        private byte _CreatureType;
        private byte _CreatureSubType;
        private byte _Ranged;
        private string _Bytes;
        private byte _Icone;
        private byte _Emote;
        private UInt16 _Title;
        public UInt16[] _Unks = new UInt16[7];
        private string _Flag;
        private string _ScriptName;

        public void SetCreatureTypesAndSubTypes()
        {
            // Sets the class, family and speciees of creatures
            switch (_Model1)
            {
                case 1: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 2: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 3: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 4: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 5: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 6: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 7: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 8: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 9: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 10: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 11: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DISCIPLE_OF_KHAINE; break;
                case 12: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_BLACK_ORC; break;
                case 13: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_CHOPPA; break;
                case 14: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SHAMAN; break;
                case 15: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG_HERDER; break;
                case 16: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_IRONBREAKER; break;
                case 17: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_IRONBREAKER; break;
                case 18: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_HAMMERER; break;
                case 19: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_HAMMERER; break;
                case 20: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_ENGINEER; break;
                case 21: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_ENGINEER; break;
                case 22: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_RUNEPRIEST; break;
                case 23: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_RUNEPRIEST; break;
                case 24: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHOSEN; break;
                case 25: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MARAUDER; break;
                case 26: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_ZEALOT; break;
                case 27: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_ZEALOT; break;
                case 28: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 29: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 30: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_KNIGHT_OF_THE_BLAZING_SUN; break;
                case 31: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_KNIGHT_OF_THE_BLAZING_SUN; break;
                case 32: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 33: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 34: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WITCH_HUNTER; break;
                case 35: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WITCH_HUNTER; break;
                case 36: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WARRIOR_PRIEST; break;
                case 37: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WARRIOR_PRIEST; break;
                case 38: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DISCIPLE_OF_KHAINE; break;
                case 39: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_BLACK_GUARD; break;
                case 40: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_BLACK_GUARD; break;
                case 41: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 42: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 43: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 44: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 45: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 46: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 47: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 48: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SWORDMASTER; break;
                case 49: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SWORDMASTER; break;
                case 50: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 51: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 52: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_IRONBREAKER; break;
                case 53: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_IRONBREAKER; break;
                case 54: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WARRIOR_PRIEST; break;
                case 55: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WARRIOR_PRIEST; break;
                case 56: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 57: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 58: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_BLACK_ORC; break;
                case 59: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG_HERDER; break;
                case 60: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHOSEN; break;
                case 61: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 62: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 63: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_BLACK_GUARD; break;
                case 64: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 65: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 66: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 67: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 68: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 69: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 70: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 71: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 72: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 73: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 74: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 75: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 76: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 77: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 78: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 79: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 80: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 81: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 82: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 83: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 84: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 85: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 86: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 87: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 88: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 89: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 90: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 91: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 92: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 93: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 94: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 95: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 96: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 97: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 98: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 99: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 100: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 101: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 102: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 103: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 104: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 105: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 106: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 107: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 113: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 114: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 115: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 116: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 117: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 118: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 119: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 120: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 121: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 122: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 123: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 124: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 125: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 126: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 127: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 128: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 129: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 130: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 131: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 132: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 133: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 134: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 135: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 136: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 137: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 138: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 139: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 140: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 141: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_HORROR; break;
                case 142: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_HORROR; break;
                case 143: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_FLAMER; break;
                case 144: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_FIREWYRM; break;
                case 145: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 146: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 147: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 148: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 149: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 150: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 151: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 152: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 153: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 154: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_NURGLE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_NURGLE_NURGLING; break;
                case 155: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 156: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 157: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 158: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 159: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 160: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 161: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 162: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 163: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 164: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 165: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 166: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 167: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 168: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 169: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 170: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 171: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 172: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 173: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 174: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 175: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 176: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 177: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 178: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 179: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 180: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 181: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 182: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 183: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 184: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 185: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 186: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 187: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 188: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 189: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 190: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 191: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 192: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 193: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 194: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 195: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 196: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 197: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 198: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 199: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 200: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 201: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 202: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 203: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 204: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 205: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 206: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 207: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 208: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 209: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 210: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 211: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 212: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 213: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 214: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 215: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 216: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 217: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 218: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 219: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 220: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 221: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 222: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 223: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 224: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 225: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 226: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 227: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 228: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 229: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 230: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 231: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 232: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 233: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 234: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 235: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 236: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 237: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 238: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 239: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 240: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 241: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 242: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 243: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 244: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 245: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 246: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 247: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 248: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 249: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 250: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 251: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 252: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 253: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 254: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 255: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 256: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 257: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 258: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 259: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 260: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 261: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 262: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 263: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 264: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 265: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 266: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 267: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 268: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 269: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 270: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 271: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 272: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 273: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 274: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 275: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 276: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 277: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 278: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 279: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 280: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 281: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 282: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 283: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 284: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 285: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 286: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 287: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 288: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 289: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 290: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 291: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 292: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 293: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 294: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 295: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 296: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 297: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 298: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 299: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 300: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 301: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 302: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 303: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 304: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 305: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 306: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 307: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 308: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 309: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 310: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 311: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 312: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 313: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 314: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 315: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 316: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 317: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 318: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 319: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 320: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 321: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 322: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 323: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 324: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 325: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 326: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 327: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 328: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 329: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 330: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 331: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 332: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 333: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 334: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 335: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 336: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 337: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 338: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 339: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 340: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 341: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 342: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 343: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 344: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 345: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 346: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 347: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 348: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 349: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 400: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 401: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 402: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 403: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 404: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 666: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 777: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 999: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1000: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1001: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1002: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1003: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1004: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1005: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1006: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1007: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1008: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1009: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1010: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1011: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1012: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1013: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1014: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_OGRE; break;
                case 1015: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1016: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_UNGOR; break;
                case 1017: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_GOR; break;
                case 1018: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_BESTIGOR; break;
                case 1019: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_DOOMBULL; break;
                case 1020: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GNOBLAR; break;
                case 1021: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1022: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1023: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1024: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1025: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1026: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1027: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1028: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1029: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1030: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1031: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1032: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1033: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1034: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1035: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1036: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1037: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1038: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1039: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1040: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1041: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1042: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1043: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1044: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1045: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1046: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1047: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1048: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1049: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1050: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1051: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1052: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1053: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1054: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1055: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_GORGER; break;
                case 1056: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1057: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_GOR; break;
                case 1058: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1059: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1060: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1061: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1062: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1063: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1064: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1065: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1066: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1067: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1068: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1069: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1070: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_TROLLS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_TROLLS_TROLL; break;
                case 1071: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_TROLLS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_TROLLS_RIVER_TROLL; break;
                case 1072: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_TROLLS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_TROLLS_STONE_TROLL; break;
                case 1073: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_TROLLS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_TROLLS_CHAOS_TROLL; break;
                case 1074: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1075: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1076: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1077: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1078: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1079: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1080: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_GIANTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_GIANTS_GIANT; break;
                case 1081: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_GIANTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_GIANTS_CHAOS_GIANT; break;
                case 1082: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1083: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1084: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1085: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1086: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_HARPY; break;
                case 1087: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1088: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_DRYAD; break;
                case 1089: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_CHAOS_FURY; break;
                case 1090: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_NURGLE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_NURGLE_PLAGUEBEARER; break;
                case 1091: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_NURGLE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_NURGLE_NURGLING; break;
                case 1092: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_SLAANESH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_SLAANESH_DAEMONETTE; break;
                case 1093: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_KHORNE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_KHORNE_FLESH_HOUND; break;
                case 1094: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_KHORNE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_KHORNE_BLOODLETTER; break;
                case 1095: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_KHORNE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_KHORNE_JUGGERNAUT_OF_KHORNE; break;
                case 1096: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_HORROR; break;
                case 1097: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_FLAMER; break;
                case 1098: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_SCREAMER; break;
                case 1099: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_SPITE; break;
                case 1100: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_CHAOS_HOUND; break;
                case 1101: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1102: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GIANT_BAT; break;
                case 1103: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_SPIRIT_HOST; break;
                case 1104: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_WRAITH; break;
                case 1105: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_BANSHEE; break;
                case 1106: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_LICHE; break;
                case 1107: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_GHOUL; break;
                case 1108: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_FLAYERKIN; break;
                case 1109: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_SLAYER; break;
                case 1110: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_TUSKGOR; break;
                case 1111: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1112: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1113: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1114: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1115: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1116: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1117: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1118: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1119: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1120: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1121: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1122: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1123: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1124: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1125: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1126: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1127: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1128: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1129: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_UNICORN; break;
                case 1130: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_PEGASUS; break;
                case 1131: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BAT; break;
                case 1132: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_COCKATRICE; break;
                case 1133: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 1134: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 1135: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 1136: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1137: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1138: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1139: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1140: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BASILISK; break;
                case 1141: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1142: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1143: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_DEER; break;
                case 1144: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1145: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_YHETEE; break;
                case 1146: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_GORGER; break;
                case 1147: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_CONSTRUCT; break;
                case 1148: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_RAT_OGRE; break;
                case 1149: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1150: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1151: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1152: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1153: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1154: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1155: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_GRIFFON; break;
                case 1156: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_MANTICORE; break;
                case 1157: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_CHAOS_SPAWN; break;
                case 1158: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_KHORNE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_KHORNE_BLOODBEAST; break;
                case 1159: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_NURGLE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_NURGLE_PLAGUEBEAST; break;
                case 1160: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_WATCHER; break;
                case 1161: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_SLAANESH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_SLAANESH_FIEND; break;
                case 1162: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_RHINOX; break;
                case 1163: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1164: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_CENTIGOR; break;
                case 1165: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_TREEKIN; break;
                case 1166: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_WINGED_NIGHTMARE; break;
                case 1167: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOIDS_WYVERN; break;
                case 1168: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_DRAGON_OGRE; break;
                case 1169: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOID_DRAGON; break;
                case 1170: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1171: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1172: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHOSEN; break;
                case 1173: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_SLAANESH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_SLAANESH_KEEPER_OF_SECRETS; break;
                case 1174: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1175: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1176: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1177: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1178: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1179: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1180: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1181: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1182: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1183: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1184: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1185: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SHAMAN; break;
                case 1186: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1187: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1188: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1189: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1190: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1191: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1192: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1193: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1194: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1195: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1196: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1197: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_SHEEP; break;
                case 1198: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_COW; break;
                case 1199: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_COW; break;
                case 1200: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_LIZARD; break;
                case 1201: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_DEER; break;
                case 1202: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_DEER; break;
                case 1203: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_DEER; break;
                case 1204: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1205: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1206: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1207: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_TROLLS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_TROLLS_TROLL; break;
                case 1208: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_HARE; break;
                case 1209: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_PIG; break;
                case 1210: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1211: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_RAT; break;
                case 1212: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_CHAOS_MUTANT; break;
                case 1213: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_CHAOS_MUTANT; break;
                case 1214: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1215: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1216: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1217: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1218: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1219: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1220: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1221: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1222: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1223: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1224: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1225: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1226: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1227: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 1228: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 1229: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1230: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1231: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1232: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 1233: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 1234: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1235: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1236: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_OGRE_TYRANT; break;
                case 1237: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1238: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1239: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_SLAYER; break;
                case 1240: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1241: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1242: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1243: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOIDS_WYVERN; break;
                case 1244: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_KHORNE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_KHORNE_BLOODTHIRSTER; break;
                case 1245: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_DAEMONVINE; break;
                case 1246: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1247: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_CHICKEN; break;
                case 1248: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_CHICKEN; break;
                case 1249: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_OGRE_BULL; break;
                case 1250: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1251: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_DAEMON_PRINCE; break;
                case 1252: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_KHORNE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_KHORNE_BLOODLETTER; break;
                case 1253: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_VAMPIRE; break;
                case 1254: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SAVAGE_ORC; break;
                case 1255: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHOSEN; break;
                case 1256: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_NURGLE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_NURGLE_GREAT_UNCLEAN_ONE; break;
                case 1257: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOID_DRAGON; break;
                case 1258: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SAVAGE_ORC; break;
                case 1259: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1260: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1261: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1262: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1263: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1264: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1265: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1266: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1267: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1268: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1269: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1270: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_DOG; break;
                case 1271: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_CAT; break;
                case 1272: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOIDS_HYDRA; break;
                case 1273: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1274: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1275: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOID_DRAGON; break;
                case 1276: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOID_DRAGON; break;
                case 1277: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOID_DRAGON; break;
                case 1278: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1279: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1280: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1281: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1282: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1283: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1284: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1285: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1286: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_DRAGONOIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_DRAGONOID_DRAGON; break;
                case 1287: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_CRAB; break;
                case 1288: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1289: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1290: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1291: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1292: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1293: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1294: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1295: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1296: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1297: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1298: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1299: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1300: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1301: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1302: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1303: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1304: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1305: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1306: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1307: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1308: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1309: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1310: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1311: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1312: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1313: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1314: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1315: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1316: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1317: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1318: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1319: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1320: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1321: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1322: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1323: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1324: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1325: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1326: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1327: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1328: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1329: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1330: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1331: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1332: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1333: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1334: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1335: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1336: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1337: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1338: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1339: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_VAMPIRE; break;
                case 1340: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1341: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1342: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_HARPY; break;
                case 1343: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1344: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1345: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1346: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1347: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1348: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1349: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1350: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1351: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1352: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_WINGED_NIGHTMARE; break;
                case 1353: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 1354: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 1355: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1356: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1357: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_BLACK_GUARD; break;
                case 1358: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_BLACK_GUARD; break;
                case 1359: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_BLACK_GUARD; break;
                case 1360: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_BLACK_GUARD; break;
                case 1361: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_BANSHEE; break;
                case 1362: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_BANSHEE; break;
                case 1363: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_DRYAD; break;
                case 1364: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_DRYAD; break;
                case 1365: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_DRYAD; break;
                case 1366: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_DRYAD; break;
                case 1367: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_CHAOS_FURY; break;
                case 1368: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_CHAOS_FURY; break;
                case 1369: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_HARPY; break;
                case 1370: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_HARPY; break;
                case 1371: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_HARPY; break;
                case 1372: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1373: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1374: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_GIANT_LIZARD; break;
                case 1375: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1376: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1377: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1378: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1379: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1380: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1381: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_SPITE; break;
                case 1382: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_SPITE; break;
                case 1383: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_SPITE; break;
                case 1384: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_SPITE; break;
                case 1385: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1386: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1387: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1388: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1389: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SWORDMASTER; break;
                case 1390: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1391: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1392: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1393: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1394: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_MANTICORE; break;
                case 1395: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_MANTICORE; break;
                case 1396: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_MANTICORE; break;
                case 1397: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_WINGED_NIGHTMARE; break;
                case 1398: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_WINGED_NIGHTMARE; break;
                case 1399: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_WINGED_NIGHTMARE; break;
                case 1400: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1401: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1402: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1403: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1404: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1405: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1406: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1407: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1408: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1409: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1410: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1411: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1412: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1413: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1414: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1415: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1416: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1417: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1418: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1419: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1420: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1421: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1422: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1423: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1424: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1425: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1426: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1427: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1428: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1429: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1430: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1431: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1432: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1433: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1434: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1435: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1436: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1437: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1438: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1439: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1440: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1441: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1442: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1443: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1444: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1445: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1446: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1447: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1448: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1449: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1450: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1451: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1452: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1453: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1454: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1455: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1456: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1457: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1458: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1459: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1460: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1461: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1462: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1463: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1464: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1465: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1466: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1467: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1468: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1469: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1470: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1471: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1472: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1473: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1474: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1475: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1476: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1477: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1478: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1479: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1480: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1481: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1482: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1483: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1484: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1485: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1486: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1487: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1488: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1489: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1490: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1491: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1492: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1493: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1494: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1495: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1496: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_SORCERESS; break;
                case 1497: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1498: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1499: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1500: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1501: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1502: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1503: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1504: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1505: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 1506: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 1507: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1508: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1509: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1510: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1511: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1512: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1513: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1514: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1515: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1516: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1517: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1518: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1519: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1520: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 1521: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_ARCHMAGE; break;
                case 1522: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1523: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1524: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1525: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1526: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1527: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_SHADOW_WARRIOR; break;
                case 1528: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1529: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1530: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1531: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1532: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1533: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1534: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1535: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1536: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1537: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1538: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1539: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_GRIFFON; break;
                case 1540: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1541: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1542: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_WALKER; break;
                case 1543: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WARRIOR_PRIEST; break;
                case 1544: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_WARRIOR_PRIEST; break;
                case 1545: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 1546: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 1547: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_NIGHT_GOBLIN; break;
                case 1548: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1549: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1550: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1551: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1552: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1553: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1554: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1555: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1556: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1557: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1558: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1559: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1560: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1561: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1562: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1563: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1564: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1565: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1566: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1567: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1568: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1569: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1570: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1571: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1572: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1573: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1574: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1575: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_MAGGOT; break;
                case 1576: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1577: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1578: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1579: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1580: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_GHOUL; break;
                case 1581: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_GHOUL; break;
                case 1582: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1583: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1584: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_WALKER; break;
                case 1585: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_WALKER; break;
                case 1586: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1587: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1588: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1589: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1590: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1591: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1592: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1593: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1594: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1595: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1596: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1597: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1598: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1599: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1600: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1601: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1602: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1603: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1604: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1605: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1606: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_DAEMONVINE; break;
                case 1607: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1608: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_BRIGHT_WIZARD; break;
                case 1609: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1610: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_SPIRIT_HOST; break;
                case 1611: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_SPIRIT_HOST; break;
                case 1612: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_SPIRIT_HOST; break;
                case 1613: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_SPIRIT_HOST; break;
                case 1614: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1615: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1616: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1617: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GREAT_CAT; break;
                case 1618: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_YHETEE; break;
                case 1619: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1620: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_UNGOR; break;
                case 1621: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_GOR; break;
                case 1622: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_BESTIGOR; break;
                case 1623: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_ZEALOT; break;
                case 1624: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MARAUDER; break;
                case 1625: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_NURGLE; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_NURGLE_SLIME_HOUND; break;
                case 1626: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_LORD_OF_CHANGE; break;
                case 1627: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_WALKER; break;
                case 1628: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1629: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1630: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1631: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1632: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1633: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1634: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1635: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1636: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1637: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1638: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1639: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1640: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1641: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1642: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_BONE_GIANT; break;
                case 1643: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1644: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1645: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1646: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1647: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1648: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1649: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1650: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1651: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1652: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1653: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_UNMARKED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_UNMARKED_DAEMONS_CHAOS_HOUND; break;
                case 1654: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_CENTIGOR; break;
                case 1655: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_TUSKGOR; break;
                case 1656: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_GORGER; break;
                case 1657: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_BEASTMEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_BEASTMEN_DOOMBULL; break;
                case 1658: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_CHAOS_BREEDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_CHAOS_BREEDS_FLAYERKIN; break;
                case 1659: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1660: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1661: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1662: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1663: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1664: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SPIDER; break;
                case 1665: this.CreatureType = (byte)GameData.CreatureTypes.PLANTS_FOREST_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.PLANTS_FOREST_SPIRITS_TREEMAN; break;
                case 1666: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1667: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1668: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1669: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1670: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1671: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1672: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1673: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1674: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1675: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1676: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1677: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1678: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1679: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1680: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1681: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_SPIDER; break;
                case 1682: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_OGRES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_OGRES_OGRE_BULL; break;
                case 1683: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1684: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1685: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1686: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1687: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1688: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1689: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1690: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1691: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1692: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_WITCH_ELVES; break;
                case 1693: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1694: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1695: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1696: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1697: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1698: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1699: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1700: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1701: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_ORC; break;
                case 1702: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_GOBLIN; break;
                case 1703: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1704: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_EMPIRE; break;
                case 1705: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1706: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1707: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_CHAOS; break;
                case 1708: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1709: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_HIGH_ELF; break;
                case 1710: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1711: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DARK_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DARK_ELVES_DARK_ELF; break;
                case 1712: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_DWARFS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_DWARFS_DWARF; break;
                case 1713: this.CreatureType = (byte)GameData.CreatureTypes.MONSTERS_TROLLS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_TROLLS_STONE_TROLL; break;
                case 1714: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_CONSTRUCT; break;
                case 1715: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_PRESERVED_DEAD; break;
                case 1716: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_PRESERVED_DEAD; break;
                case 1717: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_LICHE; break;
                case 1718: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1719: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_BONE_GIANT; break;
                case 1720: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_CARRION; break;
                case 1721: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1722: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1723: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1724: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_SCARAB_BONE_CONSTRUCT; break;
                case 1725: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1726: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_MANTICORE; break;
                case 1727: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_GRIFFON; break;
                case 1728: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_USHABTI; break;
                case 1729: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1730: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1731: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1732: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1733: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_ASP_BONE_CONSTRUCT; break;
                case 1734: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1735: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1736: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1737: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_LIVING_ARMOR; break;
                case 1738: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_SCARAB_BONE_CONSTRUCT; break;
                case 1739: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1740: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1741: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1742: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1743: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1744: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1745: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1746: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1747: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1748: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1749: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1750: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1751: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_MANTICORE; break;
                case 1752: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_MANTICORE; break;
                case 1753: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1754: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1755: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1756: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1757: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1758: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1759: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1760: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_TOMB_SWARM; break;
                case 1761: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1762: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1763: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_ASP_BONE_CONSTRUCT; break;
                case 1764: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1765: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1766: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1767: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1768: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_MAGUS; break;
                case 1769: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_USHABTI; break;
                case 1770: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_USHABTI; break;
                case 1771: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_SCARAB_BONE_CONSTRUCT; break;
                case 1772: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_BONE_GIANT; break;
                case 1773: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1774: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_CARRION; break;
                case 1775: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCARAB; break;
                case 1776: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_DEER; break;
                case 1777: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_DEER; break;
                case 1778: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1779: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1780: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1781: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1782: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_DEER; break;
                case 1783: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1784: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 1785: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 1786: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 1787: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_ELVES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_ELVES_WHITE_LION; break;
                case 1788: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1789: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1790: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1791: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1792: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1793: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1794: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_CARRION; break;
                case 1795: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCORPION; break;
                case 1796: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_INSECTS_ARACHNIDS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_INSECTS_ARACHNIDS_GIANT_SCARAB; break;
                case 1797: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1798: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1799: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1800: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_LICHE; break;
                case 1801: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1802: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_VAMPIRE; break;
                case 1803: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_VAMPIRE; break;
                case 1804: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_VAMPIRE; break;
                case 1805: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_VAMPIRE; break;
                case 1806: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1807: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1808: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_GHOUL; break;
                case 1809: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1810: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1811: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1812: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1813: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_WOLF; break;
                case 1814: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_WRAITH; break;
                case 1815: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_WRAITH; break;
                case 1816: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_BANSHEE; break;
                case 1817: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SPIRITS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SPIRITS_SPIRIT_HOST; break;
                case 1818: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GIANT_BAT; break;
                case 1819: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_GIANT_BAT; break;
                case 1820: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1821: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1822: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1823: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1824: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_GREATER_UNDEAD; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_GREATER_UNDEAD_VAMPIRE; break;
                case 1825: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1826: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_ZOMBIES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_ZOMBIES_ZOMBIE; break;
                case 1827: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_CONSTRUCTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_CONSTRUCTS_WINGED_NIGHTMARE; break;
                case 1828: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1829: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1830: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1831: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1832: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1833: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1834: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1835: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1836: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1837: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1838: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1839: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 1840: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 1841: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 1842: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BEAR; break;
                case 1843: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1844: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1845: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1846: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_IMP; break;
                case 1847: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1848: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1849: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1850: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1851: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1852: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1853: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1854: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1855: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1856: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1857: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1858: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1859: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1860: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_BOAR; break;
                case 1861: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_GRIFFON; break;
                case 1862: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_GRIFFON; break;
                case 1863: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_GRIFFON; break;
                case 1864: this.CreatureType = (byte)GameData.CreatureTypes.MAMONSTERS_GICAL_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.MONSTERS_MAGICAL_BEASTS_GRIFFON; break;
                case 1865: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1866: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1867: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1868: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1869: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1870: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1871: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1872: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1873: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1874: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1875: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1876: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1877: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1878: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1879: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1880: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SQUIG; break;
                case 1881: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1882: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1883: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1884: this.CreatureType = (byte)GameData.CreatureTypes.UNDEAD_SKELETONS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNDEAD_SKELETONS_SKELETON; break;
                case 1885: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1886: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1887: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1888: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1889: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1890: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1891: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1892: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_HUMANS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_HUMANS_HUMAN; break;
                case 1893: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1894: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1895: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1896: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_LIVESTOCK; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_LIVESTOCK_HORSE; break;
                case 1897: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1898: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1899: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1900: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_CRITTER; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_CRITTER_BIRD; break;
                case 1901: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1902: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1903: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1904: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_GREENSKINS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_GREENSKINS_SNOTLING; break;
                case 1905: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1906: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1907: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1908: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1909: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1910: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1911: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1912: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_BEASTS; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_BEASTS_HOUND; break;
                case 1913: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_HORROR; break;
                case 1914: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_HORROR; break;
                case 1915: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_HORROR; break;
                case 1916: this.CreatureType = (byte)GameData.CreatureTypes.DAEMONS_TZEENTCH; this.CreatureSubType = (byte)GameData.CreatureSubTypes.DAEMONS_TZEENTCH_HORROR; break;
                case 1917: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1918: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1919: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1920: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1921: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1922: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1923: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1924: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1925: this.CreatureType = (byte)GameData.CreatureTypes.ANIMALS_REPTILES; this.CreatureSubType = (byte)GameData.CreatureSubTypes.ANIMALS_REPTILES_COLD_ONE; break;
                case 1926: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1927: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1928: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1929: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1930: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1931: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1932: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1933: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1934: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1935: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1936: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1937: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1938: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1939: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1940: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1941: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1942: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1943: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1944: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1945: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
                case 1946: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1947: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1948: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1949: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1950: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1951: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1952: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                case 1953: this.CreatureType = (byte)GameData.CreatureTypes.HUMANOIDS_SKAVEN; this.CreatureSubType = (byte)GameData.CreatureSubTypes.HUMANOIDS_SKAVEN_SKAVEN; break;
                default: this.CreatureType = (byte)GameData.CreatureTypes.UNCLASSIFIED; this.CreatureSubType = (byte)GameData.CreatureSubTypes.UNCLASSIFIED; break;
            }
        }

        [DataElement(Unique=true,AllowDbNull = false)]
        public uint Entry
        {
            get { return _Entry; }
            set { _Entry = value; Dirty = true; }
        }

        [DataElement(Varchar=255,AllowDbNull = false)]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Model1
        {
            get { return _Model1; }
            set { _Model1 = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Model2
        {
            get { return _Model2; }
            set { _Model2 = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 MinScale
        {
            get { return _MinScale; }
            set { _MinScale = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 MaxScale
        {
            get { return _MaxScale; }
            set { _MaxScale = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte MinLevel
        {
            get { return _MinLevel; }
            set { _MinLevel = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte MaxLevel
        {
            get { return _MaxLevel; }
            set { _MaxLevel = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Faction
        {
            get { return _Faction; }
            set { _Faction = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte CreatureType
        {
            get { return _CreatureType; }
            set { _CreatureType = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte CreatureSubType
        {
            get { return _CreatureSubType; }
            set { _CreatureSubType = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Ranged
        {
            get { return _Ranged; }
            set { _Ranged = value; Dirty = true; }
        }

        [DataElement(Varchar=255,AllowDbNull = false)]
        public string Bytes
        {
            get { return _Bytes; }
            set { _Bytes = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Icone
        {
            get { return _Icone; }
            set { _Icone = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Emote
        {
            get { return _Emote; }
            set { _Emote = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Title
        {
            get { return _Title; }
            set { _Title = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk
        {
            get { return _Unks[0]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[0] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk1
        {
            get { return _Unks[1]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[1] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk2
        {
            get { return _Unks[2]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[2] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk3
        {
            get { return _Unks[3]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[3] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk4
        {
            get { return _Unks[4]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[4] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk5
        {
            get { return _Unks[5]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[5] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk6
        {
            get { return _Unks[6]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[6] = value; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Flag
        {
            get { return _Flag; }
            set { _Flag = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string ScriptName
        {
            get { return _ScriptName; }
            set { _ScriptName = value; Dirty = true; }
        }

        public byte[] bBytes
        {
            get
            {
                List<byte> Btes = new List<byte>();
                string[] Strs = _Bytes.Split(';');
                foreach (string Str in Strs)
                    if (Str.Length > 0)
                        Btes.Add(byte.Parse(Str));

                Btes.Remove(4);
                Btes.Remove(5);
                Btes.Remove(7);

                return Btes.ToArray();
            }
        }

        public List<Quest> StartingQuests;
        public List<Quest> FinishingQuests;
    }
}
