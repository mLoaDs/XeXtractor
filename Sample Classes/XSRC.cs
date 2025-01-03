using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 * 
 * Thanks to Anthony for providing the XSRC Mapping
 * 
 */
namespace XeXtractor
{
    public class XSRC
    {
        protected EndianIo io;
        int magic;
        int version;
        int size;

        public XSRC(EndianIo io)
        {
            this.io = io;
        }
        public void Open()
        {
            io.Open();
        }

        public void Close()
        {
            io.Close();
        }

        public virtual void Read()
        {
            Read(io.In);
        }
        public XSRC(byte[] data)
        {
            io = new EndianIo(data, EndianType.BigEndian);
        }
        public XSRC(string filePath)
        {
            io = new EndianIo(filePath, EndianType.BigEndian);
        }
  
        public virtual void Read(EndianReader er)
        {
            magic = er.ReadInt32();
            if (magic != 0x58535243)
            {
                throw new Exception("Invalid magic!");
            }
            version = er.ReadInt32();
            size = er.ReadInt32();
            int fileNameLength = er.ReadInt32();
            string fileName = er.ReadAsciiString(fileNameLength);
            int originalLength = er.ReadInt32();
            int compressedLength = er.ReadInt32();
            byte[] compressed = er.ReadBytes(compressedLength);
            System.IO.MemoryStream mstr = new System.IO.MemoryStream(compressed);
            System.IO.Compression.GZipStream str = new System.IO.Compression.GZipStream(mstr, System.IO.Compression.CompressionMode.Decompress, true);
            byte[] decompressed = new byte[originalLength];
            str.Read(decompressed, 0, originalLength);
            str.Close();
            
                FileEntry entr = new FileEntry();
                entr.fileName = fileName;
                entr.Data = decompressed;
                entr.folder = "";
                entr.type = "XSRC";
                InnerFileStructure.getInstance().AddFileEntry(entr);
            
          


        }


    }
}
