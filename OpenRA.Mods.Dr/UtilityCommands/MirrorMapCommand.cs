#region Copyright & License Information
/*
 * Copyright 2007-2018 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OpenRA.FileSystem;
using OpenRA.Graphics;
using OpenRA.Mods.Dr.FileFormats;

namespace OpenRA.Mods.Dr.UtilityCommands
{
    enum MirrorType
    {
        Horizontal,
        Vertical,
        HorizontalAndVertical
    }

    class TileTransform
    {
        public MirrorType MirrorType;
        public TerrainTile Tile;
        public CPos Position;

        public IEnumerable<TileTransform> GetTransforms(Map map)
        {
            var horizontalTransform = new TileTransform()
            {
                Position = new CPos(map.MapSize.X - Position.X - 1, Position.Y),
                Tile = Tile
            };

            var verticalTransform = new TileTransform()
            {
                Position = new CPos(Position.X, map.MapSize.Y - Position.Y - 1),
                Tile = Tile
            };

            var horizontalAndVerticalTransform = new TileTransform()
            {
                Position = new CPos(map.MapSize.X - Position.X - 1, map.MapSize.Y - Position.Y - 1),
                Tile = Tile
            };

            switch (MirrorType)
            {
                case MirrorType.Horizontal:
                    yield return horizontalTransform;
                    break;
                case MirrorType.Vertical:
                    yield return verticalTransform;
                    break;
                case MirrorType.HorizontalAndVertical:
                    yield return horizontalTransform;
                    yield return verticalTransform;
                    yield return horizontalAndVerticalTransform;
                    break;
            }
        }
    }

    class ResourceTransform
    {
        public MirrorType MirrorType;
        public ResourceTile Tile;
        public CPos Position;

        public IEnumerable<ResourceTransform> GetTransforms(Map map)
        {
            var horizontalTransform = new ResourceTransform()
            {
                Position = new CPos(map.MapSize.X - Position.X - 1, Position.Y),
                Tile = Tile
            };

            var verticalTransform = new ResourceTransform()
            {
                Position = new CPos(Position.X, map.MapSize.Y - Position.Y - 1),
                Tile = Tile
            };

            var horizontalAndVerticalTransform = new ResourceTransform()
            {
                Position = new CPos(map.MapSize.X - Position.X - 1, map.MapSize.Y - Position.Y - 1),
                Tile = Tile
            };

            switch (MirrorType)
            {
                case MirrorType.Horizontal:
                    yield return horizontalTransform;
                    break;
                case MirrorType.Vertical:
                    yield return verticalTransform;
                    break;
                case MirrorType.HorizontalAndVertical:
                    yield return horizontalTransform;
                    yield return verticalTransform;
                    yield return horizontalAndVerticalTransform;
                    break;
            }
        }
    }

    class ActorTransform
    {
        static Dictionary<string, int2> mirrorOffsets = new Dictionary<string, int2>()
        {
            { "aotre000", new int2(0, -1) },
            { "aotre001", new int2(0, -1) },
            { "aotre002", new int2(0, -1) },
            { "aotre003", new int2(0, -1) },
            { "aotre005", new int2(0, -1) },
            { "aoroc003", new int2(1, 2) },
            { "aoroc004", new int2(1, 1) },
            { "aoroc005", new int2(1, 1) },
            { "aoclf000", new int2(1, 2) },
            { "aoclf001", new int2(1, 2) },
            { "aoclf002", new int2(1, 2) },
            { "aoclf003", new int2(2, 3) },
            { "aoclf004", new int2(2, 3) },
            { "aoclf005", new int2(3, 3) },

            { "aotre000.snow", new int2(0, -1) },
            { "aotre001.snow", new int2(0, -1) },
            { "aotre002.snow", new int2(0, -1) },
            { "aotre003.snow", new int2(0, -1) },
            { "aotre005.snow", new int2(0, -1) },
            { "aoroc003.snow", new int2(1, 2) },
            { "aoroc004.snow", new int2(1, 1) },
            { "aoroc005.snow", new int2(1, 1) },
            { "aoclf000.snow", new int2(1, 2) },
            { "aoclf001.snow", new int2(1, 2) },
            { "aoclf002.snow", new int2(1, 2) },
            { "aoclf003.snow", new int2(2, 3) },
            { "aoclf004.snow", new int2(2, 3) },
            { "aoclf005.snow", new int2(3, 3) },

            { "aotre000.base", new int2(0, -1) },
            { "aotre001.base", new int2(0, -1) },
            { "aotre002.base", new int2(0, -1) },
            { "aotre003.base", new int2(0, -1) },
            { "aotre005.base", new int2(0, -1) },
            { "aoroc003.base", new int2(1, 2) },
            { "aoroc004.base", new int2(1, 1) },
            { "aoroc005.base", new int2(1, 1) },
            { "aoclf000.base", new int2(1, 2) },
            { "aoclf001.base", new int2(1, 2) },
            { "aoclf002.base", new int2(1, 2) },
            { "aoclf003.base", new int2(2, 3) },
            { "aoclf004.base", new int2(2, 3) },
            { "aoclf005.base", new int2(3, 3) },
        };

        public MirrorType MirrorType;
        public ActorReference Actor;
        public CPos Position;

        public IEnumerable<ActorTransform> GetTransforms(Map map)
        {
            var offset = int2.Zero;
            if (mirrorOffsets.ContainsKey(Actor.Type))
                offset = mirrorOffsets[Actor.Type];

            var horizontalTransform = new ActorTransform()
            {
                Position = new CPos(map.MapSize.X - Position.X - offset.X - 1, Position.Y),
                Actor = Actor
            };

            var verticalTransform = new ActorTransform()
            {
                Position = new CPos(Position.X, map.MapSize.Y - Position.Y - offset.Y - 1),
                Actor = Actor
            };

            var horizontalAndVerticalTransform = new ActorTransform()
            {
                Position = new CPos(map.MapSize.X - Position.X - offset.X - 1, map.MapSize.Y - Position.Y - offset.Y - 1),
                Actor = Actor
            };

            switch (MirrorType)
            {
                case MirrorType.Horizontal:
                    yield return horizontalTransform;
                    break;
                case MirrorType.Vertical:
                    yield return verticalTransform;
                    break;
                case MirrorType.HorizontalAndVertical:
                    yield return horizontalTransform;
                    yield return verticalTransform;
                    yield return horizontalAndVerticalTransform;
                    break;
            }
        }
    }

    class MirrorMapCommand : IUtilityCommand
    {
        string IUtilityCommand.Name { get { return "--mirror-map"; } }
        bool IUtilityCommand.ValidateArguments(string[] args) { return ValidateArguments(args); }

        [Desc("FILENAME", "Mirror an OpenRA Dark Reign map.")]
        void IUtilityCommand.Run(Utility utility, string[] args) { Run(utility, args); }

        public ModData ModData;
        public Map Map;
        int actorIndex = 0;

        protected bool ValidateArguments(string[] args)
        {
            return args.Length >= 2;
        }

        protected void Run(Utility utility, string[] args)
        {
            // HACK: The engine code assumes that Game.modData is set.
            Game.ModData = ModData = utility.ModData;

            var filename = args[1];
            var flag = args[2];
            if (string.IsNullOrWhiteSpace(flag))
                flag = "VH";

            bool flipHorizontal = flag.Contains("H");
            bool flipVertical = flag.Contains("V");

            MirrorType mirrorType = MirrorType.Horizontal;
            if (flipVertical)
                mirrorType = MirrorType.Vertical;
            if (flipHorizontal && flipVertical)
                mirrorType = MirrorType.HorizontalAndVertical;

            var targetPath = "..\\mods\\dr\\maps";

            var package = new Folder(targetPath).OpenPackage(filename, ModData.ModFiles);
            if (package == null)
            {
                Console.WriteLine("Couldn't find map file: " + filename);
                return;
            }

            Map = new Map(ModData, package);
            var size = Map.MapSize;
            switch (mirrorType)
            {
                case MirrorType.Horizontal:
                    size = size.WithX(size.X / 2);
                    break;
                case MirrorType.Vertical:
                    size = size.WithY(size.Y / 2);
                    break;
                case MirrorType.HorizontalAndVertical:
                    size = size / 2;
                    break;
            }

            // Tiles
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    var pos = new CPos(x, y);
                    var transformTile = new TileTransform()
                    {
                        Tile = Map.Tiles[pos],
                        MirrorType = mirrorType,
                        Position = pos
                    };

                    foreach (var tt in transformTile.GetTransforms(Map))
                    {
                        var newPos = tt.Position;
                        Map.Tiles[newPos] = tt.Tile;
                    }
                }
            }

            // Actors
            actorIndex = GetHighestActorIndex();
            var actorDefs = new List<ActorReference>();
            foreach (var a in Map.ActorDefinitions)
            {
                var existing = new ActorReference(a.Value.Value, a.Value.ToDictionary());
                var pos = existing.InitDict.Get<LocationInit>().Value(null);
                var owner = existing.InitDict.Get<OwnerInit>();

                if (pos.X < 0 || pos.X >= size.X ||
                    pos.Y < 0 || pos.Y >= size.Y)
                    continue;

                var actor = new ActorTransform()
                {
                    Actor = existing,
                    Position = pos,
                    MirrorType = mirrorType,
                };

                foreach (var at in actor.GetTransforms(Map))
                {
                    var ar = new ActorReference(actor.Actor.Type)
                    {
                        new LocationInit(at.Position),
                        owner
                    };

                    actorDefs.Add(ar);
                }
            }

            foreach (var a in actorDefs)
            {
                Map.ActorDefinitions.Add(new MiniYamlNode("Actor" + ++actorIndex, a.Save()));
            }

            // Resources
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    var pos = new CPos(x, y);
                    var resource = new ResourceTransform()
                    {
                        Tile = Map.Resources[pos],
                        MirrorType = mirrorType,
                        Position = pos
                    };

                    foreach (var rt in resource.GetTransforms(Map))
                    {
                        var newPos = rt.Position;
                        Map.Resources[newPos] = rt.Tile;
                    }
                }
            }

            var dest = Path.Combine(targetPath, Path.GetFileNameWithoutExtension(filename) + "-mirrored.oramap");

            Map.Save(ZipFileLoader.Create(dest));
            Console.WriteLine(dest + " saved.");
        }

        int GetHighestActorIndex()
        {
            var pattern = new Regex(@"Actor(?<actorId>\d$)");
            return Map.ActorDefinitions
                .Select(x => pattern.Match(x.Key).Groups["actorId"])
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => int.Parse(x.Value))
                .Max();
        }
    }
}
