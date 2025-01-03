using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/*
 * 
 * Thanks to Anthony for most of the XBDF code. He made all of it except the extraction code!
 * 
 * */
namespace XeXtractor
{
    public class XDBF
    {
        protected int entryCurrent;
        protected int entryMax;
        protected List<EntryData> entryTable = new List<EntryData>();
        protected int freeCurrent = 1;
        protected int freeMax;
        protected List<FileLoc> freeTable = new List<FileLoc>();
        protected EndianIo io;
        protected int magic = 0x58444246;
        protected int version = 0x10000;
        protected long startpos = 0;
        public XDBF(EndianIo io)
        {
            this.io = io;
        }

        public XDBF(byte[] data)
        {
            io = new EndianIo(data, EndianType.BigEndian);
        }
        public XDBF(string filePath)
        {
            io = new EndianIo(filePath, EndianType.BigEndian);
        }

        public int Magic
        {
            get { return magic; }
            set { magic = value; }
        }

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        public int EntryMax
        {
            get { return entryMax; }
            set { entryMax = value; }
        }

        public int EntryCurrent
        {
            get { return entryCurrent; }
            set { entryCurrent = value; }
        }

        public int FreeMax
        {
            get { return freeMax; }
            set { freeMax = value; }
        }

        public int FreeCurrent
        {
            get { return freeCurrent; }
            set { freeCurrent = value; }
        }

        public List<EntryData> EntryTable
        {
            get { return entryTable; }
            set { entryTable = value; }
        }

        public List<FileLoc> FreeTable
        {
            get { return freeTable; }
            set { freeTable = value; }
        }

        public int HeaderSize
        {
            get { return 0x18 + (entryMax * 0x12) + (freeMax * 0x08); }
        }

        public EndianIo Io
        {
            get { return io; }
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

        public virtual void Read(EndianReader er)
        {
            if ((magic = er.ReadInt32()) != 0x58444246)
                throw new Exception("Invalid magic!");
            version = er.ReadInt32();
            entryMax = er.ReadInt32();
            entryCurrent = er.ReadInt32();
            freeMax = er.ReadInt32();
            freeCurrent = er.ReadInt32();
            for (int x = 0; x < entryMax; x++)
                entryTable.Add(new EntryData(er));
            for (int x = 0; x < freeCurrent; x++)
                freeTable.Add(new FileLoc(er));
            for (int x = 0; x < freeMax - freeCurrent; x++)
            {
                new FileLoc(er);
            }
            startpos = er.BaseStream.Position;
        }
        public string ExtractFileLoc(string folder)
        {
            string retVal = "";
            string fileLocFolder = System.IO.Path.Combine(folder, "FileLoc");
           
            EndianReader er = io.In;
            for (int x = 0; x < freeTable.Count; x++)
            {
                FileLoc dt = freeTable[x];
                if (dt.Size != 0)
                {
                    er.SeekTo(dt.Offset + startpos);
                    byte[] file = er.ReadBytes((int)dt.Size);
                    if (!System.IO.Directory.Exists(fileLocFolder))
                    {
                        System.IO.Directory.CreateDirectory(fileLocFolder);
                    }
                    retVal += "Extracted FileLoc\\" + x.ToString() + ".bin";
                    string fullFileName = System.IO.Path.Combine(fileLocFolder, x.ToString() + ".bin");
                    System.IO.File.WriteAllBytes(fullFileName, file);
                }
            }
            return retVal;

        }
        private string StringIdtoString(long id)
        {
            string retVal = id.ToString("X");
            switch(id)
            {
                case 1 :
                    retVal = "en-US";
                    break;
                case 2:
                    retVal = "ja-JP";
                    break;
                case 3:
                    retVal = "de-DE";
                    break;
                case 4:
                    retVal = "fr-FR";
                    break;
                case 5:
                    retVal = "es-ES";
                    break;
                case 6 :
                    retVal = "it-IT";
                    break;
                case 7 :
                    retVal = "ko-KR";
                    break;
                case 8 :
                    retVal = "zh-TW";
                    break;
                case 0xa:
                    retVal = "zh-CN";
                    break;
                case 0x0b:
                   retVal = "pl-PL";
                    break;
                case 0x0c:
                    retVal = "ru-RU";
                    break;
                default :
                    break;
            }
            return retVal;
        }
        public void ExtractEntryData()
        {
            Log.getInstance().AddEntry("Extract XDBF" );
          /*  string stringsFolder = System.IO.Path.Combine(folder, "Strings");
            string binsFolder = System.IO.Path.Combine(folder, "Bins");
            string pngFolder = System.IO.Path.Combine(folder, "Png");*/
            EndianReader er = io.In;
            for (int x = 0; x < entryTable.Count; x++)
            {
                EntryData dt = entryTable[x];
                if (dt.Namespace != 2)
                {

                    if (dt.Namespace == 3)
                    {
                        er.SeekTo(dt.Offset + startpos);
                        byte[] file = er.ReadBytes(dt.Size);

                        string fileName = StringIdtoString(dt.ID) + ".xstr";
                        Log.getInstance().AddEntry("Extract " + "Strings\\" + fileName);
              
                   
                        FileEntry fentr = new FileEntry();
                        fentr.fileName = fileName;
                        fentr.folder = "Strings";
                        fentr.type = "XDBF";
                        fentr.Data = file;
                        InnerFileStructure.getInstance().AddFileEntry(fentr);
            
                    }
                    else
                    {
                      
                        er.SeekTo(dt.Offset + startpos);
                        byte[] file = er.ReadBytes(dt.Size);
                        io.Out.SeekTo(dt.Offset + startpos);
                       

                        string fileName = dt.ID.ToString("X") + ".";
                        string ext = System.Text.Encoding.UTF8.GetString(file, 0, 4);
                        fileName = fileName + ext;
                        Log.getInstance().AddEntry("Extract " + "Bins\\" + fileName);
                        FileEntry fentr = new FileEntry();
                        fentr.fileName = fileName;
                        fentr.folder = "Bins";
                        fentr.type = "XDBF";
                        fentr.Data = file;
                        InnerFileStructure.getInstance().AddFileEntry(fentr);
                     
                    }
                }
                else
                {

                    er.SeekTo(dt.Offset + startpos);
                    byte[] file = er.ReadBytes(dt.Size);
                  

                    string fileName = pngIdToName(dt.ID) + ".png";
                    Log.getInstance().AddEntry("Extract " + "Png\\" + fileName);
                    string ffile = "";
                    string folder = "";
                    int lastSlash = fileName.LastIndexOf("\\");
                    if (lastSlash != -1)
                    {
                        ffile = fileName.Substring(lastSlash + 1);
                        folder = fileName.Substring(0, lastSlash);
                    }
                    else
                    {
                        ffile = fileName;
                    }
                    FileEntry fentr = new FileEntry();
                    fentr.fileName = ffile;
                    fentr.folder = "Png\\" + folder;
                    fentr.type = "XDBF";
                    fentr.Data = file;
                    InnerFileStructure.getInstance().AddFileEntry(fentr);

                }
            }
            Log.getInstance().AddSeperator();
          
        }

        public string pngIdToName(Int64 id)
        {
            string retVal = "" ;
            if (id == 0x8000)
            {
                retVal = "GameIcon\\" + id.ToString("X");
            }
            else
            {
                if (id < 0x8000)
                {
                    retVal = "Achievement\\" + id.ToString("X");
                }
                else
                {
                    if (id > 0x20000)
                    {
                        retVal = "GamerPic\\Large-" + id.ToString("X");
                    }
                    else
                    {
                        if (id > 0x10000)
                        {
                            retVal = "GamerPic\\Small-" + id.ToString("X");
                        }
                        else
                        {
                            retVal = "Unknow\\" + id.ToString("X");
                        }
                    }
                }
            }

            return retVal;
        }
        public virtual void Write()
        {
            Write(io.Out);
        }

        public virtual void Write(EndianWriter ew)
        {
            ew.Write(magic);
            ew.Write(version);
            ew.Write(entryMax);
            ew.Write(entryCurrent);
            ew.Write(freeMax);
            ew.Write(freeCurrent);
            for (int x = 0; x < entryMax; x++)
                entryTable[x].Write(ew);
            for (int x = 0; x < freeCurrent; x++)
                freeTable[x].Write(ew);
        }

        #region Nested type: EntryData

        public class EntryData
        {
            private Int64 id;
            private short ns;
            private int offset;
            private int size;

            public EntryData()
            {
            }

            public EntryData(EndianReader er)
            {
                Read(er);
            }

            public short NamespaceShort
            {
                get { return ns; }
                set { ns = value; }
            }

            public bool IsEmpty
            {
                get
                {
                    return
                        ns == 0 ||
                        offset == 0 ||
                        size == 0;
                }
            }

            public Int64 ID
            {
                get { return id; }
                set { id = value; }
            }

            public int Offset
            {
                get { return offset; }
                set { offset = value; }
            }

            public int Size
            {
                get { return size; }
                set { size = value; }
            }

            public void Null()
            {

                ns = 0;
                offset = 0;
                size = 0;
            }

            public short Namespace
            {
                get { return ns; }
                set { ns = (short)value; }
            }

            public void Read(EndianReader er)
            {
                ns = er.ReadInt16();
                id = er.ReadInt64();
                offset = er.ReadInt32();
                size = er.ReadInt32();
            }

            public void Write(EndianWriter ew)
            {
                ew.Write(ns);
                ew.Write(id);
                ew.Write(offset);
                ew.Write(size);
            }


        }

        #endregion

        #region Nested type: FileLoc

        public class FileLoc
        {
            private int offset;
            private uint size;

            public FileLoc()
            {
            }

            public FileLoc(EndianReader er)
            {
                Read(er);
            }

            public int Offset
            {
                get { return offset; }
                set { offset = value; }
            }

            public uint Size
            {
                get { return size; }
                set { size = value; }
            }

            public void Read(EndianReader er)
            {
                offset = er.ReadInt32();
                size = er.ReadUInt32();
            }

            public void Write(EndianWriter ew)
            {
                ew.Write(offset);
                ew.Write(size);
            }

            public override string ToString()
            {
                return string.Format("0x{0:X} - 0x{1:X}", offset, size);
            }
        }

        #endregion
    }
}
