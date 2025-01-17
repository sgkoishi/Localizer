using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using MonoMod.Utils;
using Terraria;
using Terraria.ModLoader;
using static Localizer.Utils;
using static Localizer.Helpers.ReflectionHelper;

namespace Localizer.Package.Export
{
    public sealed class LdstrFileExport : IFileExportService
    {
        private static List<MethodBase> _blackList1 = new List<MethodBase>
        {
            GetMethodBase<ModTranslation>(
                "System.Void Terraria.ModLoader.ModTranslation::SetDefault(System.String)"),
            GetMethodBase<ModTranslation>(
                "System.String Terraria.ModLoader.ModTranslation::GetTranslation(System.String)"),
            GetMethodBase<ModTranslation>(
                "System.Void Terraria.ModLoader.ModTranslation::AddTranslation(System.Int32,System.String)"),
            GetMethodBase<ModTranslation>(
                "System.Void Terraria.ModLoader.ModTranslation::AddTranslation(System.String,System.String)"),
            GetMethodBase<ModTranslation>(
                "System.Void Terraria.ModLoader.ModTranslation::AddTranslation(Terraria.Localization.GameCulture,System.String)"),
            GetMethodBase<Mod>(
                "Microsoft.Xna.Framework.Graphics.Texture2D Terraria.ModLoader.Mod::GetTexture(System.String)"),
            GetMethodBase<Mod>(
                "Microsoft.Xna.Framework.Audio.SoundEffect Terraria.ModLoader.Mod::GetSound(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.Audio.Music Terraria.ModLoader.Mod::GetMusic(System.String)"),
            GetMethodBase<Mod>("ReLogic.Graphics.DynamicSpriteFont Terraria.ModLoader.Mod::GetFont(System.String)"),
            GetMethodBase<Mod>(
                "Microsoft.Xna.Framework.Graphics.Effect Terraria.ModLoader.Mod::GetEffect(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.Config.ModConfig Terraria.ModLoader.Mod::GetConfig(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.GlobalProjectile Terraria.ModLoader.Mod::GetGlobalProjectile(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModNPC Terraria.ModLoader.Mod::GetNPC(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::NPCType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalNPC Terraria.ModLoader.Mod::GetGlobalNPC(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModPlayer Terraria.ModLoader.Mod::GetPlayer(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModBuff Terraria.ModLoader.Mod::GetBuff(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::BuffType(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.GlobalBuff Terraria.ModLoader.Mod::GetGlobalBuff(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModMountData Terraria.ModLoader.Mod::GetMount(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::MountType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModWorld Terraria.ModLoader.Mod::GetModWorld(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.ModUgBgStyle Terraria.ModLoader.Mod::GetUgBgStyle(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.ModSurfaceBgStyle Terraria.ModLoader.Mod::GetSurfaceBgStyle(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetSurfaceBgStyleSlot(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.GlobalBgStyle Terraria.ModLoader.Mod::GetGlobalBgStyle(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.ModWaterStyle Terraria.ModLoader.Mod::GetWaterStyle(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.ModWaterfallStyle Terraria.ModLoader.Mod::GetWaterfallStyle(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetWaterfallStyleSlot(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetGoreSlot(System.String)"),
            GetMethodBase<Mod>("System.Void Terraria.ModLoader.Mod::AddBackgroundTexture(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetBackgroundSlot(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.ModTranslation Terraria.ModLoader.Mod::CreateTranslation(System.String)"),
            GetMethodBase<Mod>("System.Byte[] Terraria.ModLoader.Mod::GetFileBytes(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModItem Terraria.ModLoader.Mod::GetItem(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::ItemType(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.GlobalItem Terraria.ModLoader.Mod::GetGlobalItem(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModPrefix Terraria.ModLoader.Mod::GetPrefix(System.String)"),
            GetMethodBase<Mod>("System.Byte Terraria.ModLoader.Mod::PrefixType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModDust Terraria.ModLoader.Mod::GetDust(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::DustType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModTile Terraria.ModLoader.Mod::GetTile(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::TileType(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.GlobalTile Terraria.ModLoader.Mod::GetGlobalTile(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.ModTileEntity Terraria.ModLoader.Mod::GetTileEntity(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::TileEntityType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModWall Terraria.ModLoader.Mod::GetWall(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::WallType(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.GlobalWall Terraria.ModLoader.Mod::GetGlobalWall(System.String)"),
            GetMethodBase<Mod>(
                "Terraria.ModLoader.ModProjectile Terraria.ModLoader.Mod::GetProjectile(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::ProjectileType(System.String)"),
            typeof(ModContent).FindMethod(
                "Microsoft.Xna.Framework.Audio.SoundEffect Terraria.ModLoader.ModContent::GetSound(System.String)"),
            typeof(ModContent).FindMethod("System.Byte[] Terraria.ModLoader.ModContent::GetFileBytes(System.String)"),
            typeof(ModContent).FindMethod("System.Boolean Terraria.ModLoader.ModContent::FileExists(System.String)"),
            typeof(ModContent).FindMethod(
                "Microsoft.Xna.Framework.Graphics.Texture2D Terraria.ModLoader.ModContent::GetTexture(System.String)"),
            typeof(ModContent).FindMethod("System.Boolean Terraria.ModLoader.ModContent::TextureExists(System.String)"),
            typeof(ModContent).FindMethod(
                "Microsoft.Xna.Framework.Audio.SoundEffect Terraria.ModLoader.ModContent::GetSound(System.String)"),
            typeof(ModContent).FindMethod("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            typeof(ModContent).FindMethod(
                "Terraria.ModLoader.Audio.Music Terraria.ModLoader.ModContent::GetMusic(System.String)"),
            typeof(ModContent).FindMethod("System.Boolean Terraria.ModLoader.ModContent::MusicExists(System.String)"),
            typeof(ModContent).FindMethod(
                "System.Int32 Terraria.ModLoader.ModContent::GetModBossHeadSlot(System.String)"),
            typeof(ModContent).FindMethod("System.Int32 Terraria.ModLoader.ModContent::GetModHeadSlot(System.String)"),
            typeof(ModContent).FindMethod(
                "System.Int32 Terraria.ModLoader.ModContent::GetModBackgroundSlot(System.String)"),
            typeof(ModContent).FindMethod("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            typeof(ModContent).FindMethod("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            typeof(ModContent).FindMethod("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            GetMethodBase<ModRecipe>(
                "System.Void Terraria.ModLoader.ModRecipe::AddTile(Terraria.ModLoader.Mod,System.String)")
        };

        private static List<MethodBase> _blackList2 = new List<MethodBase>
        {
            GetMethodBase<ModRecipe>(
                "System.Void Terraria.ModLoader.ModRecipe::AddIngredient(Terraria.ModLoader.Mod,System.String,System.Int32)")
        };

        public void Export(IPackage package, IExportConfig config)
        {
            if (package?.Mod == null)
            {
                return;
            }

            var asm = package.Mod.Code;

            var file = new LdstrFile
            {
                LdstrEntries = new Dictionary<string, LdstrEntry>()
            };

            foreach (var type in asm.ManifestModule.GetTypes())
            {
                if (type.Namespace == null)
                {
                    continue;
                }

                var methodBases = new List<MethodBase>();
                methodBases.AddRange(type.GetMethods(All | BindingFlags.DeclaredOnly));
                methodBases.AddRange(type.GetConstructors(All | BindingFlags.DeclaredOnly));
                foreach (var method in methodBases)
                {
                    if (method.DeclaringType?.Namespace == null || method.IsAbstract)
                    {
                        continue;
                    }

                    try
                    {
                        Utils.LogDebug($"Exporting method: [{method.GetID()}]");
                        var entry = GetEntryFromMethod(method);
                        if (entry != null && !file.LdstrEntries.ContainsKey(method.GetID()))
                        {
                            file.LdstrEntries.Add(method.GetID(), entry);
                        }
                    }
                    catch (Exception e)
                    {
                        Localizer.Log.Error(e.ToString());
                    }
                }
            }

            package.AddFile(file);
        }

        private LdstrEntry GetEntryFromMethod(MethodBase method)
        {
            var instructions = GetInstructions(method);

            if (instructions == null)
            {
                return null;
            }

            var entry = new LdstrEntry { Instructions = new List<BaseEntry>() };
            for (var i = 0; i < instructions.Count; i++)
            {
                var ins = instructions[i];
                if (ins.opcode == OpCodes.Ldstr && !string.IsNullOrWhiteSpace(ins.operand.ToString()))
                {
                    // Filter methods in blacklist1
                    if (i < instructions.Count - 1)
                    {
                        var next = instructions[i + 1];
                        if (next.opcode == OpCodes.Call || next.opcode == OpCodes.Calli ||
                            next.opcode == OpCodes.Callvirt)
                        {
                            if (_blackList1.Any(m => (next.operand as MethodBase).GetID() == m.GetID()))
                            {
                                continue;
                            }
                        }
                    }

                    // Filter methods in blacklist2
                    if (i < instructions.Count - 2)
                    {
                        var afterNext = instructions[i + 2];
                        if (afterNext.opcode == OpCodes.Call || afterNext.opcode == OpCodes.Calli ||
                            afterNext.opcode == OpCodes.Callvirt)
                        {
                            if (_blackList2.Any(m => (afterNext.operand as MethodBase).GetID() == m.GetID()))
                            {
                                continue;
                            }
                        }
                    }

                    // No need to add a same string
                    if (entry.Instructions.Exists(e => e.Origin == ins.operand.ToString()))
                    {
                        continue;
                    }

                    entry.Instructions.Add(new BaseEntry
                    {
                        Origin = ins.operand.ToString(),
                        Translation = ""
                    });
                }
            }

            return entry.Instructions.Count == 0 ? null : entry;
        }

        public void Dispose()
        {
            _blackList1 = null;
            _blackList2 = null;
        }
    }
}
