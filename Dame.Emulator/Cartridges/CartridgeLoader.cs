using System;
using System.IO;

namespace Dame.Emulator.Cartridges
{
    public static class CartridgeLoader
    {
        public static ICartridge Load(Stream stream)
        {
            using var memoryStream = new MemoryStream();

            stream.CopyTo(memoryStream);

            var data = new ReadOnlySpan<byte>(memoryStream.GetBuffer(), 0, (int)memoryStream.Position); // TODO: check for overflow

            // TODO: infer/guess the type of the cartridge and return an adequate type

            // currently supporting only the basic cartridge type
            
            return new BasicCartridge(data);
        }
    }
}
