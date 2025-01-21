using Kingmaker.Blueprints;
using System.Collections.Generic;
using System;
using System.Linq;
using Owlcat.Runtime.Core.Logging;
using Kingmaker.ElementsSystem;
using Kingmaker.Blueprints.JsonSystem.Helpers;
using Kingmaker;
using Kingmaker.Designers.EventConditionActionSystem;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using System.Reflection;
using Kingmaker.Blueprints.Area;
using Kingmaker.Globalmap.Blueprints.CombatRandomEncounters;

[TypeId("bee18b6a31d4fdf4cad2be0d360bebc1")]
class jhfhAnswerCheck : GameAction
{
    public override string GetCaption() => nameof(jhfhAnswerCheck);
    protected override void RunAction()
    {
        var log = PFLog.Mods;
        log.Log("jhfh: PostLoad assetGUID: " + this.Owner.AssetGuid);

        switch (this.Owner.AssetGuid)
        {
            case "cc07e68dea774614af5ff7deb88586d8":
                GenerateRetinueCombat();
                break;

            case "5dae2ca527734fb49f6725672a9ceb5f":
                GenerateSpaceCombat();
                break;

            default:
                return;
        }
    }

    private static void GenerateRetinueCombat()
    {
        var log = PFLog.Mods;
        log.Log("jhfh: Generating retinue battle encounter");

        string[] areas = {
            "d7f1b46a86d14055b2f29ad2a667582e", //RE_SpaceHulk
            "a7e9004c1b17448da51e97578b10c501", //RE_LowerDecks
            "3b94a148c3d74279a0ed6aacc18b320a", //RE_Macrocannon
            "66a2e6c66ab546839157aaa600f8924c", //RE_DrukhariAttack
            "df8c8823bbee45b0ba773737a7298585" //RE_Hold
        };

        string[][] entries = new string[areas.Length][];

        entries[0] = new string[] { "61337960c31a4c6ca47cb6023622d5d6" }; //RE_SpaceHulk
        entries[1] = new string[] { "48c37d52a1c44992a2ea48f44cac574f", "070a957d0d2a4811907de3708bdf4b06", "4aad6d74f31c4a14bae34c4d6c03dd6d" }; //RE_LowerDecks
        entries[2] = new string[] { "8fc4ee88caa74ccebb67d52f005a71cc", "55750ab51ad446948e9f1a1d880ad840" }; //RE_Macrocannon
        entries[3] = new string[] { "be93376a740e42edab5246d8292823e0" }; //RE_DrukhariAttack
        entries[4] = new string[] { "f71e953feb8f47a28a6a493bbef98e96", "41dd01028f994bbaad7cd8439bd969df", "60294666634442f8b19789e914c9d112", "0f7dfebf98754bc2b816eefb7ef7dc42" }; //RE_Hold

        Tuple<string, string>[][] enemies = new Tuple<string, string>[][] {
            new Tuple<string, string>[] {
                Tuple.Create("488db3e26aca4e7198e5ca14c97b18e3", ""), // TzeentchHorrorsSpaceHulkGroup_1
                Tuple.Create("e1e80b0b555746d0b388f372aeb3eaa1", ""), // NurgleLowerDecksGroup_1
                Tuple.Create("004aa8a6185645589b19d199c86073cb", "")  // BloodlettersMacrocannonGroup_1
            },
            new Tuple<string, string>[] {
                Tuple.Create("488db3e26aca4e7198e5ca14c97b18e3", ""), // TzeentchHorrorsSpaceHulkGroup_1
                Tuple.Create("e1e80b0b555746d0b388f372aeb3eaa1", ""), // NurgleLowerDecksGroup_1
                Tuple.Create("004aa8a6185645589b19d199c86073cb", ""), // BloodlettersMacrocannonGroup_1
                Tuple.Create("f3653fa119664d22b76e1921862bea6f", ""), // PsykersDrukhariAttackGroup_1
                Tuple.Create("9d8956e784ad4f4c9612771e91e951fd", ""), // BloodlettersControlledMacrocannonGroup
                Tuple.Create("bcad57ba2c45457482ca546628f9f58c", ""), // DemonetteDrukhariAttackGroup_1
                Tuple.Create("ce9587a2383c43c7b3b76de3d5280d52", "") // ServetorsAndDemonGroup
            },
            new Tuple<string, string>[] {
                Tuple.Create("488db3e26aca4e7198e5ca14c97b18e3", ""), // TzeentchHorrorsSpaceHulkGroup_1
                Tuple.Create("e1e80b0b555746d0b388f372aeb3eaa1", ""), // NurgleLowerDecksGroup_1
                Tuple.Create("004aa8a6185645589b19d199c86073cb", ""), // BloodlettersMacrocannonGroup_1
                Tuple.Create("f3653fa119664d22b76e1921862bea6f", ""), // PsykersDrukhariAttackGroup_1
                Tuple.Create("9d8956e784ad4f4c9612771e91e951fd", ""), // BloodlettersControlledMacrocannonGroup
                Tuple.Create("bcad57ba2c45457482ca546628f9f58c", ""), // DemonetteDrukhariAttackGroup_1
                Tuple.Create("ce9587a2383c43c7b3b76de3d5280d52", ""), // ServetorsAndDemonGroup
                Tuple.Create("e998c5102c064cb78d62f4e472ab61a6", ""), // TzeentchPortalsLowerDecksGroup_1
                Tuple.Create("39a6bd7e30ea413eae3ab11dc095b7c2", ""), // KhornHoldGroup_1
                Tuple.Create("fde19db3589745b58157a9c2ee27a36f", "") // TzintchHoldGroup_1
            }
        };

        int difficultyIndex = GetDifficultyIndex();

        GenerateCombatRandomEncounterAndTeleportReflected gen = new GenerateCombatRandomEncounterAndTeleportReflected();

        Random rnd = new Random();
        int areaIndex = rnd.Next(areas.Length);
        string thisArea = areas[areaIndex];
        string thisEntry = entries[areaIndex][rnd.Next(entries[areaIndex].Length)];

        var selectedEnemyTuple = enemies[difficultyIndex][rnd.Next(enemies[difficultyIndex].Length)];
        string thisGroup = selectedEnemyTuple.Item1;
        string thisFlag = string.IsNullOrEmpty(selectedEnemyTuple.Item2) ? "" : selectedEnemyTuple.Item2;

        var area = ResourcesLibrary.TryGetBlueprint<BlueprintArea>(thisArea).ToReference<BlueprintAreaReference>();
        FieldInfo areaField = typeof(GenerateCombatRandomEncounterAndTeleport).GetField("m_Area", BindingFlags.NonPublic | BindingFlags.Instance);

        var entry = ResourcesLibrary.TryGetBlueprint<BlueprintAreaEnterPoint>(thisEntry).ToReference<BlueprintAreaEnterPointReference>();
        FieldInfo entryField = typeof(GenerateCombatRandomEncounterAndTeleport).GetField("m_OverrideEnterPoint", BindingFlags.NonPublic | BindingFlags.Instance);

        var group = new Object(); ;
        var blueprint = ResourcesLibrary.TryGetBlueprint<BlueprintRandomGroupOfUnits>(thisGroup);
        if (blueprint != null)
        {
            group = blueprint.ToReference<BlueprintRandomGroupOfUnits.Reference>();
        }
        FieldInfo groupField = typeof(GenerateCombatRandomEncounterAndTeleport).GetField("m_OverrideRandomGroup", BindingFlags.NonPublic | BindingFlags.Instance);

        areaField.SetValue(gen, area);
        entryField.SetValue(gen, entry);
        groupField.SetValue(gen, group);

        if (thisFlag != "")
        {
            var flag = ResourcesLibrary.TryGetBlueprint<BlueprintUnlockableFlag>(thisFlag).ToReference<BlueprintUnlockableFlagReference>();
            FieldInfo flagField = typeof(GenerateCombatRandomEncounterAndTeleport).GetField("m_UnlockFlag", BindingFlags.NonPublic | BindingFlags.Instance);
            flagField.SetValue(gen, flag);
        }

        log.Log("jhfh: level" + difficultyIndex);
        log.Log("jhfh: area" + area);
        log.Log("jhfh: entry" + entry);
        log.Log("jhfh: group" + group);

        if (area.Get().GetComponent<AreaRandomEncounter>() == null) { GenerateRetinueCombat(); } //recuuuuurrrrrrrssssionnnnn!!!!!

        gen.RunActionLocal();
    }

    private static void GenerateSpaceCombat()
    {
        var log = PFLog.Mods;
        log.Log("jhfh: Generating space battle encounter");

        string[][] battles = new string[3][];

        battles[0] = new string[] { // Yellow Battles
            "057f42c5bb7840eabbdef9d0e6e60279",
            "2839d44ace7547b5ace4531be3171f67",
            "035e5110bdb0424cb731ce1807e29d90",
            "2839d44ace7547b5ace4531be3171f67",
            "1227d881701e41a2bb8d246f0a1f72c8",
            "2ad064881c164601825c03eb2d6e7eae"
        };
        battles[1] = new string[] { // Orange Battles
            "057f42c5bb7840eabbdef9d0e6e60279",
            "2839d44ace7547b5ace4531be3171f67",
            "035e5110bdb0424cb731ce1807e29d90",
            "2839d44ace7547b5ace4531be3171f67",
            "1227d881701e41a2bb8d246f0a1f72c8",
            "2ad064881c164601825c03eb2d6e7eae",
            "f12d2775ed76412b8d8526c6cfa23a6d",
            "52c8c0830bf343e68b4a12ab60f7b2b5",
            "e10f1f4ce7d54dc2a44c3ee24ce84ef2",
            "1d2e53fdcb1c41f082ca24e3d57b12bc"
        };
        battles[2] = new string[] { // Red Battles
            "f12d2775ed76412b8d8526c6cfa23a6d",
            "52c8c0830bf343e68b4a12ab60f7b2b5",
            "e10f1f4ce7d54dc2a44c3ee24ce84ef2",
            "1d2e53fdcb1c41f082ca24e3d57b12bc",
            "1af525c3b4fa4d7f918b135ef177b115",
            "9992e0c5569b453c9b7611dd1c3eaa85"
        };

        int difficultyIndex = GetDifficultyIndex();
        log.Log("jhfh: difficultyIndex" + difficultyIndex);
        Random rnd = new Random();
        string battle = battles[difficultyIndex][rnd.Next(battles[difficultyIndex].Length)];
        log.Log("jhfh: battle" + battle);

        var exit = ResourcesLibrary.TryGetBlueprint<BlueprintAreaEnterPoint>(battle).ToReference<BlueprintAreaEnterPointReference>();
        FieldInfo exitField = typeof(TeleportParty).GetField("m_exitPositon", BindingFlags.NonPublic | BindingFlags.Instance);

        TeleportPartyReflected gen = new TeleportPartyReflected();
        gen.AutoSaveMode = Kingmaker.EntitySystem.Persistence.AutoSaveMode.BeforeExit;
        exitField.SetValue(gen, exit);

        gen.RunActionLocal();
    }

    public class GenerateCombatRandomEncounterAndTeleportReflected : GenerateCombatRandomEncounterAndTeleport
    {
        public void RunActionLocal()
        {
            RunAction();
        }
    }

    public class TeleportPartyReflected : TeleportParty
    {
        public void RunActionLocal()
        {
            RunAction();
        }
    }

    public static int GetDifficultyIndex()
    {
        string[] difficulties = {
            "Yellow",
            "Orange",
            "Red"
        };

        int level = Game.Instance.Player.PartyLevel;
        if (level <= 20)
        {
            return 0; // Yellow
        }
        else if (level > 20 && level <= 35)
        {
            return 1; // Orange
        }
        else
        {
            return 2; // Red
        }
    }

}

class ClassesWithGuid
{
    public static List<(Type, string)> Classes = new List<(Type, string)>()
        {
            (typeof(jhfhAnswerCheck), "bee18b6a31d4fdf4cad2be0d360bebc1")
        };
}

