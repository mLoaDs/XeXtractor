using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XeXtractor
{
    public class XSTR
    {

        protected EndianIo io;
        int magic;
        int version;
        int unknow;
        short nbString;
        public XSTR(byte[] data)
        {
            io = new EndianIo(data, EndianType.BigEndian);
        }
        public XSTR(EndianIo io)
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

        public virtual void Read(bool DoNotAddOutput,string outputName)
        {
            Read(io.In, DoNotAddOutput, outputName);
        }
        private string originalFile = "";
        public XSTR(string filePath)
        {
            originalFile = filePath;
            io = new EndianIo(filePath, EndianType.BigEndian);
        }
        public string GetStringId(ushort stringId)
        {
            string retVal = "";
            foreach (StringEntry entr in stringList)
            {
                if (entr.id == stringId)
                {
                    retVal = entr.theString;
                    break;
                }

            }
            return retVal;
        }

        System.Collections.ArrayList stringList = new System.Collections.ArrayList();
        public virtual void Read(EndianReader er,bool DoNotAddOutput,string outputName)
        {
            magic = er.ReadInt32();
            if (magic != 0x58535452)
            {
                throw new Exception("File is not an XSTR file");
            }
            version = er.ReadInt32();
            unknow = er.ReadInt32();
            nbString = er.ReadInt16();
            
            for (int x = 0; x < nbString; x++)
            {
                stringList.Add(new StringEntry(er));
            }
            System.IO.MemoryStream mstr = new System.IO.MemoryStream();
            System.IO.TextWriter fstr = new System.IO.StreamWriter(mstr,Encoding.UTF8);
            for (int x = 0; x < nbString; x++)
            {
                StringEntry entr = (StringEntry)stringList[x];
                fstr.WriteLine(entr.id.ToString("X") + " - " + entr.theString);
            }
            fstr.Close();
            if (!DoNotAddOutput)
            {
                FileEntry fentr = new FileEntry();
                fentr.Data = mstr.ToArray();
                fentr.fileName = outputName + ".txt";
                fentr.type = "XSTR";
                fentr.folder = "";
                InnerFileStructure.getInstance().AddFileEntry(fentr);
            }
            
        }
      
        #region Nested type: EntryData

        public class StringEntry
        {
            public ushort id;
            public ushort length;
            public string theString;


            public StringEntry()
            {
            }

            public StringEntry(EndianReader er)
            {
                Read(er);
            }
            public void Read(EndianReader er)
            {
                id = er.ReadUInt16();
                length = er.ReadUInt16();
                theString = er.ReadAsciiString(length);
            }
        }
        #endregion
    }
}
