using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XeXtractor
{
    public class XACH
    {

        protected EndianIo io;
        int magic;
        int version;
        int size;
       
        public XACH(EndianIo io)
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

        public virtual void Read(XSTR xstr)
        {
            Read(io.In, xstr);
        }
        public XACH(byte[] data)
        {
            io = new EndianIo(data, EndianType.BigEndian);
        }
        public XACH(string filePath)
        {
            io = new EndianIo(filePath, EndianType.BigEndian);
        }



        public virtual void Read(EndianReader er ,XSTR xstr)
        {
            magic = er.ReadInt32();
            if (magic != 0x58414348)
            {
                throw new Exception("Invalid magic!");
            }
            string xmlOutput = "<?xml version=\"1.0\">\r\n";

            version = er.ReadInt32();
            size = er.ReadInt32();
            short nbAchievement = er.ReadInt16();
            for (int x = 0; x < nbAchievement; x++)
            {
                short achievementId = er.ReadInt16();
                ushort titleStringId = er.ReadUInt16();
                ushort descriptionStringId = er.ReadUInt16();
                ushort unachievedStringId = er.ReadUInt16();
                int imageId = er.ReadInt32();
                short gamerCred = er.ReadInt16();
                short unused = er.ReadInt16();
                int flag = er.ReadInt32();
                int unused2 = er.ReadInt32();
                unused2 = er.ReadInt32();
                unused2 = er.ReadInt32();
                unused2 = er.ReadInt32();
                
                string title = xstr.GetStringId(titleStringId);// XSTR.GetStringId(titleStringId, stringFile);
                string description = xstr.GetStringId(descriptionStringId);
                string unachieved = xstr.GetStringId(unachievedStringId);
              
                xmlOutput += "\t<achievement id=\"" + achievementId.ToString("X") + "\" gamercred=\"" + gamerCred.ToString() + "\" flag=\"" + flag.ToString() + "\">\r\n";
                xmlOutput += "\t\t<title>" + title + "</title>\r\n";
                xmlOutput += "\t\t<description>" + description + "</description>\r\n";
                xmlOutput += "\t\t<unachieved>" + unachieved + "</unachieved>\r\n";
                xmlOutput += "\t\t<image>" + imageId.ToString("X") + ".png</image>\r\n";
                xmlOutput += "\t</achievement>\r\n";
            }
            xmlOutput += "</achievements>";
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.TextWriter wri = new System.IO.StreamWriter(ms);//System.IO.Path.Combine(outputFolder, "achievements.xml"));
            wri.Write(xmlOutput);
            wri.Close();
            FileEntry entr = new FileEntry();
            entr.Data = ms.ToArray();
            entr.fileName = "achievements.xml";
            entr.folder = "";
            entr.type = "XACH";
            InnerFileStructure.getInstance().AddFileEntry(entr);
        }



    }
}
