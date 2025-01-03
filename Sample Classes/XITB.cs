using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XeXtractor
{
    public class XITB
    {

        protected EndianIo io;
       

        public XITB(EndianIo io)
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
        public XITB(byte[] data)
        {
            io = new EndianIo(data, EndianType.BigEndian);
        }
        public XITB(string filePath)
        {
            io = new EndianIo(filePath, EndianType.BigEndian);
        }



        public virtual void Read(EndianReader er)
        {
            int magic = er.ReadInt32();
            if (magic != 0x58495442)
            {
                throw new Exception("Invalid magic!");
            }
            int version = er.ReadInt32();
            int fileSize = er.ReadInt32();
            int numberOfEntries = er.ReadInt32();
            for (int x = 0; x < numberOfEntries; x++)
            {
                int imageId = er.ReadInt32();
                int filenameLength = er.ReadInt32();
                string fileName = er.ReadAsciiString(filenameLength);
                FileEntry[] images = InnerFileStructure.getInstance().getFiles("XDBF","Png", imageId,false);
                foreach (FileEntry img in images)
                {
                    img.fileName = fileName;
                }
   
            }
        }



    }
}
