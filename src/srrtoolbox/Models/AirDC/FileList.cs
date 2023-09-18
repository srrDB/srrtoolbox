using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace srrtoolbox.Models.AirDC
{
    [XmlRoot(ElementName = "File")]
    public class ShareFile
    {
        [XmlAttribute(AttributeName = "Name")]
        public string? Name { get; set; }

        [XmlAttribute(AttributeName = "Size")]
        public long Size { get; set; }

        [XmlAttribute(AttributeName = "TTH")]
        public string? TTH { get; set; }

        public override string ToString()
        {
            return Name + " (" + Size + " bytes)";
        }
    }

    [XmlRoot(ElementName = "Directory")]
    public class ShareDirectory
    {

        [XmlElement(ElementName = "File")]
        public List<ShareFile> File_ { get; set; } //Files?

        [XmlAttribute(AttributeName = "Name")]
        public string? Name { get; set; }

        [XmlAttribute(AttributeName = "Date")]
        public long Date { get; set; }

        public DateTime DateDateTime
        {
            get
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(this.Date).ToLocalTime();

                return dateTime;
            }
        }

        //TODO: detect p2p groups from configuration etc. (TWA,PANDEMONiUM,EGEN,DAWGS,RAPiDCOWS,RTBYTES,P2P,HDChina,KraBBA,WiKi,ESiR,KraBBA,m0nkrus,PTNK)

        public bool IsRelease
        {
            get
            {
                bool hasSfv = this.File_.Any(x => x.Name.ToLower().EndsWith(".sfv"));
                bool hasParts = this.Directory_.Any(x => x.Name.ToLower() == "cd1" || x.Name.ToLower() == "dvd1" || x.Name.ToLower() == "episode.1");
                bool requiredChar1 = this.Name.Contains(".") || Name.Contains("_");
                bool requiredChar2 = this.Name.Contains("-");
                bool isExtras = Name.ToLower() == "Extras".ToLower();

                if (isExtras || !requiredChar1 || !requiredChar2)
                {
                    return false;
                }

                if (hasParts)
                {
                    return true;
                }

                if (hasSfv)
                {
                    return true;
                }

                return false;
            }
        }

        public long? ReleaseSize
        {
            get
            {
                long? fileSum = File_.Sum(x => x.Size);
                long? dirSize = Directory_.Sum(x => x.ReleaseSize);

                return fileSum + dirSize;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        [XmlElement(ElementName = "Directory")]
        public List<ShareDirectory> Directory_ { get; set; }
    }

    [XmlRoot(ElementName = "FileListing")]
    public class FileListing
    {

        [XmlElement(ElementName = "Directory")]
        public List<ShareDirectory> Directory_ { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public int Version { get; set; }

        [XmlAttribute(AttributeName = "CID")]
        public string? CID { get; set; }

        [XmlAttribute(AttributeName = "Base")]
        public string? Base { get; set; }

        [XmlAttribute(AttributeName = "BaseDate")]
        public long BaseDate { get; set; }

        public DateTime BaseDateDateTime
        {
            get
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(BaseDate).ToLocalTime();

                return dateTime;
            }
        }

        public long TotalShareSize
        {
            get
            {
                return this.Directory_.Sum(x => x.ReleaseSize ?? 0);
            }
        }

        [XmlAttribute(AttributeName = "Generator")]
        public string? Generator { get; set; }
    }
}
