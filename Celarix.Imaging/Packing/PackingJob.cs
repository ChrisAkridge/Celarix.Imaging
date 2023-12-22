using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Celarix.Imaging.JobRecovery;
using SixLabors.ImageSharp;
using static Celarix.Imaging.Utilities.BinaryJobHelpers;

namespace Celarix.Imaging.Packing
{
    internal sealed class PackingJob : IBinaryJob
    {
        public PackingOptions Options { get; set; }
        public Dictionary<string, Size> ImagesAndSizes { get; set; }
        public List<Block> Blocks { get; set; }

        public void Save(BinaryWriter writer)
        {
            writer.Write(new byte[]
            {
                0x43, 0x4C, 0x58, 0x4A  // CLXJ
            });
            
            WriteLengthPrefixedString(writer, Options.OutputPath);
            writer.Write((byte)(Options.Multipicture ? 1 : 0));
            WriteLengthPrefixedString(writer, Options.InputPath);
            writer.Write((byte)(Options.InputNamesPathListFile ? 1 : 0));
            writer.Write((byte)(Options.Recursive ? 1 : 0));
            
            writer.Write(ImagesAndSizes.Count);

            foreach (var kvp in ImagesAndSizes)
            {
               WriteLengthPrefixedString(writer, kvp.Key);
               writer.Write(kvp.Value.Width);
               writer.Write(kvp.Value.Height);
            }
            
            writer.Write(Blocks.Count);

            foreach (var block in Blocks)
            {
                writer.Write(block.Size.Width);
                writer.Write(block.Size.Height);
                WriteLengthPrefixedString(writer, block.ImageFilePath);
                SaveNodeTreeRecursive(writer, block.Fit);
            }
        }

        private static void SaveNodeTreeRecursive(BinaryWriter writer, Node node)
        {
            byte hasDownNodeFlag = node.Down != null ? (byte)0x80 : (byte)0;
            byte hasRightNodeFlag = node.Right != null ? (byte)0x40 : (byte)0;
            writer.Write((byte)(hasDownNodeFlag | hasRightNodeFlag));

            if (hasDownNodeFlag != 0)
            {
                SaveNodeTreeRecursive(writer, node.Down);
            }

            if (hasRightNodeFlag != 0)
            {
                SaveNodeTreeRecursive(writer, node.Right);
            }
            
            writer.Write((byte)(node.Used ? 1 : 0));
            writer.Write(node.Size.Width);
            writer.Write(node.Size.Height);
            writer.Write(node.Location.X);
            writer.Write(node.Location.Y);
        }

        public IBinaryJob Load(BinaryReader reader)
        {
            var _ = reader.ReadBytes(4);
            var outputPath = ReadLengthPrefixedString(reader);
            var multipicture = reader.ReadByte() == 1;
            var inputPath = ReadLengthPrefixedString(reader);
            var inputNamesPathListFile = reader.ReadByte() == 1;
            var recursive = reader.ReadByte() == 1;
            
            var imageCount = reader.ReadInt32();
            var imagesAndSizes = new Dictionary<string, Size>(imageCount);

            for (int i = 0; i < imageCount; i++)
            {
                var imagePath = ReadLengthPrefixedString(reader);
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                imagesAndSizes.Add(imagePath, new Size(width, height));
            }
            
            var blockCount = reader.ReadInt32();
            var blocks = new List<Block>(blockCount);

            for (int i = 0; i < blockCount; i++)
            {
               var width = reader.ReadInt32();
               var height = reader.ReadInt32();
               var imageFilePath = ReadLengthPrefixedString(reader);
               var fit = LoadNodeTreeRecursive(reader);
               blocks.Add(new Block
               {
                   Fit = fit,
                   ImageFilePath = imageFilePath,
                   Size = new Size(width, height)
               });
            }
            
            return new PackingJob
            {
               Options = new PackingOptions
               {
                  OutputPath = outputPath,
                  Multipicture = multipicture,
                  InputPath = inputPath,
                  InputNamesPathListFile = inputNamesPathListFile,
                  Recursive = recursive
               },
               ImagesAndSizes = imagesAndSizes,
               Blocks = blocks
            };
        }

        private static Node LoadNodeTreeRecursive(BinaryReader reader)
        {
           var flags = reader.ReadByte();
           var hasDownNode = (flags & 0x80) != 0;
           var hasRightNode = (flags & 0x40) != 0;
           Node down = null;
           Node right = null;

           if (hasDownNode) { down = LoadNodeTreeRecursive(reader); }

           if (hasRightNode) { right = LoadNodeTreeRecursive(reader); }

           var used = (flags & 0x01) != 0;
           var width = reader.ReadInt32();
           var height = reader.ReadInt32();
           var x = reader.ReadInt32();
           var y = reader.ReadInt32();

           var node = new Node(new Point(x, y), new Size(width, height))
           {
               Used = used,
               Down = down,
               Right = right
           };
                                                                                  
           return node;
        }
    }
}
