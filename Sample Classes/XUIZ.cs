using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XeXtractor
{
    public class XUIZ
    {

        protected EndianIo io;
        int magic;
        int version;
        int size;
        int unknow;
        int dataOffset;
        short nbEntries;

        public XUIZ(EndianIo io)
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

        public void Read()
        {
             Read(io.In );
        }
        public XUIZ(byte[] data)
        {
            io = new EndianIo(data, EndianType.BigEndian);
        }
        public XUIZ(string filePath)
        {
            io = new EndianIo(filePath, EndianType.BigEndian);
        }

        public void Read(EndianReader er)
        {
            Log.getInstance().AddEntry("Extracting XUIZ");
            
            magic = er.ReadInt32();
            if (magic != 0x5855495a)
            {
                throw new Exception("Selected file is not a XUIZ!");
            }
            version = er.ReadInt32();
            size = er.ReadInt32();
            unknow = er.ReadInt32();
            dataOffset = er.ReadInt32();
            nbEntries = er.ReadInt16();


            System.Collections.ArrayList stringList = new System.Collections.ArrayList();
            for (int x = 0; x < nbEntries; x++)
            {
                stringList.Add(new XUIZEntry(er));
            }
            long realStartOffset = dataOffset + 22 ;
            //  er.BaseStream.Position
            for (int x = 0; x < stringList.Count; x++)
            {
                XUIZEntry entr = (XUIZEntry)stringList[x];
               
                er.SeekTo(realStartOffset + entr.offset);
                string file = "";
                string folder = "";
                int lastSlash = entr.fileName.LastIndexOf("\\");
                if(lastSlash != -1)
                {
                    file = entr.fileName.Substring(lastSlash+1);
                    folder = entr.fileName.Substring(0,lastSlash);
                }
                else
                {
                    file = entr.fileName;
                }
               // string[] splittedFileName = entr.fileName.(new string[] { "\\" },StringSplitOptions.RemoveEmptyEntries);
             
                byte[] buffer = er.ReadBytes(entr.fileLength);
                FileEntry newFEntry = new FileEntry();
                newFEntry.Data = buffer;
                newFEntry.fileName = file;
                newFEntry.folder = folder;
                newFEntry.type = "XUIZ";
                InnerFileStructure.getInstance().AddFileEntry(newFEntry);
               
                Log.getInstance().AddEntry("Extracted " + entr.fileName);
                

            }
            Log.getInstance().AddSeperator();
            
        }

        #region Nested type: XUIZEntry

        public class XUIZEntry
        {
         
            public int fileLength;
            public int offset;
            public short fileNameLength;
            public string fileName;
            public XUIZEntry()
            {
            }

            public XUIZEntry(EndianReader er)
            {
                Read(er);
            }

            public void Read(EndianReader er)
            {
                fileLength = er.ReadInt32();
                offset = er.ReadInt32();
                fileNameLength = er.ReadByte();
                fileName = er.ReadUTF16String(fileNameLength, EndianType.BigEndian);

            }



        }

        #endregion


    }
}
