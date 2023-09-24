using System.Collections.Generic;
using System.Xml.Serialization;

namespace srrtoolbox.Models.AirDC
{
    [XmlRoot(ElementName = "Source")]
    public class Source
    {
        [XmlAttribute(AttributeName = "CID")]
        public string CID { get; set; }

        [XmlAttribute(AttributeName = "Nick")]
        public string Nick { get; set; }

        [XmlAttribute(AttributeName = "HubHint")]
        public string HubHint { get; set; }
    }

    [XmlRoot(ElementName = "Finished")]
    public class Finished
    {

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Size")]
        public int Size { get; set; }

        [XmlAttribute(AttributeName = "Added")]
        public int Added { get; set; }

        [XmlAttribute(AttributeName = "TTH")]
        public string TTH { get; set; }

        [XmlAttribute(AttributeName = "TimeFinished")]
        public int TimeFinished { get; set; }

        [XmlAttribute(AttributeName = "LastSource")]
        public string LastSource { get; set; }
    }

    [XmlRoot(ElementName = "File")]
    public class FileBundle
    {
        [XmlElement(ElementName = "Finished")]
        public Finished Finished { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public int Version { get; set; }

        [XmlAttribute(AttributeName = "Token")]
        public int Token { get; set; }

        [XmlAttribute(AttributeName = "Date")]
        public int Date { get; set; }

        [XmlAttribute(AttributeName = "AddedByAutoSearch")]
        public int AddedByAutoSearch { get; set; }
    }


    [XmlRoot(ElementName = "Download")]
    public class Download
    {
        [XmlElement(ElementName = "Source")]
        public List<Source> Source { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Size")]
        public long Size { get; set; }

        [XmlAttribute(AttributeName = "Added")]
        public int Added { get; set; }

        [XmlAttribute(AttributeName = "TTH")]
        public string TTH { get; set; }

        [XmlAttribute(AttributeName = "Priority")]
        public int Priority { get; set; }

        [XmlAttribute(AttributeName = "AutoPriority")]
        public int AutoPriority { get; set; }

        [XmlAttribute(AttributeName = "MaxSegments")]
        public int MaxSegments { get; set; }
    }

    [XmlRoot(ElementName = "Bundle")]
    public class Bundle
    {
        [XmlElement(ElementName = "Download")]
        public List<Download> Download { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public int Version { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Token")]
        public string Token { get; set; }

        [XmlAttribute(AttributeName = "Added")]
        public int Added { get; set; }

        [XmlAttribute(AttributeName = "Date")]
        public int Date { get; set; }

        [XmlAttribute(AttributeName = "AddedByAutoSearch")]
        public int AddedByAutoSearch { get; set; } //bool 0/1

        [XmlAttribute(AttributeName = "Priority")]
        public int Priority { get; set; }
    }
}
