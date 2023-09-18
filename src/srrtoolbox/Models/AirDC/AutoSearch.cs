using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace srrtoolbox.Models.AirDC
{
    [XmlRoot(ElementName = "Autosearch")]
    public class AutoSearchRoot
    {
        [XmlElement(ElementName = "Autosearch")]
        public AutoSearchMain AutoSearchMain { get; set; }
    }

    [XmlRoot(ElementName = "Autosearch")]
    public class AutoSearchMain
    {
        [XmlElement(ElementName = "Groups")]
        public Groups Groups { get; set; }

        [XmlElement(ElementName = "Autosearch")]
        public List<AutoSearchObj> AutoSearchObjs { get; set; }
    }

    [XmlRoot(ElementName = "Groups")]
    public class Groups
    {
        [XmlElement(ElementName = "Group")]
        public List<Group> Group { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public int Version { get; set; }
    }

    [XmlRoot(ElementName = "Group")]
    public class Group
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "AutoSearch")]
    public class AutoSearchObj
    {
        [XmlAttribute(AttributeName = "Enabled")]
        public int Enabled { get; set; }

        [XmlAttribute(AttributeName = "SearchString")]
        public string SearchString { get; set; }

        [XmlAttribute(AttributeName = "FileType")]
        public int FileType { get; set; }

        [XmlAttribute(AttributeName = "Action")]
        public int Action { get; set; }

        [XmlAttribute(AttributeName = "Remove")]
        public int Remove { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "MatcherType")]
        public int MatcherType { get; set; }

        [XmlAttribute(AttributeName = "MatcherString")]
        public string MatcherString { get; set; }

        [XmlAttribute(AttributeName = "UserMatch")]
        public string UserMatch { get; set; }

        [XmlAttribute(AttributeName = "ExpireTime")]
        public int ExpireTime { get; set; }

        [XmlAttribute(AttributeName = "CheckAlreadyQueued")]
        public int CheckAlreadyQueued { get; set; }

        [XmlAttribute(AttributeName = "CheckAlreadyShared")]
        public int CheckAlreadyShared { get; set; }

        [XmlAttribute(AttributeName = "SearchDays")]
        public int SearchDays { get; set; } = 1111111;

        [XmlAttribute(AttributeName = "StartTime")]
        public int StartTime { get; set; }

        [XmlAttribute(AttributeName = "EndTime")]
        public int EndTime { get; set; }

        [XmlAttribute(AttributeName = "LastSearchTime")]
        public int LastSearchTime { get; set; }

        [XmlAttribute(AttributeName = "MatchFullPath")]
        public int MatchFullPath { get; set; }

        [XmlAttribute(AttributeName = "ExcludedWords")]
        public string ExcludedWords { get; set; }

        [XmlAttribute(AttributeName = "ItemType")]
        public int ItemType { get; set; }

        [XmlAttribute(AttributeName = "Token")]
        public string Token { get; set; } //int?

        [XmlAttribute(AttributeName = "TimeAdded")]
        public int TimeAdded { get; set; }

        [XmlAttribute(AttributeName = "Group")]
        public string Group { get; set; }

        [XmlAttribute(AttributeName = "UserMatcherExclude")]
        public int UserMatcherExclude { get; set; }

        [XmlElement(ElementName = "FinishedPaths")]
        public List<AutoSearchPath> FinishedPaths { get; set; }

        [XmlElement(ElementName = "Bundles")]
        public Bundles Bundles { get; set; }

        [XmlElement(ElementName = "Params")]
        public Params Params { get; set; }
    }

    [XmlRoot(ElementName = "Bundles")]
    public class Bundles
    {
        [XmlElement(ElementName = "Bundle")]
        public uint Bundle { get; set; }
    }

    [XmlRoot(ElementName = "Params")]
    public class Params
    {
        [XmlAttribute(AttributeName = "Enabled")]
        public int Enabled { get; set; }

        [XmlAttribute(AttributeName = "CurNumber")]
        public int CurNumber { get; set; }

        [XmlAttribute(AttributeName = "MaxNumber")]
        public int MaxNumber { get; set; }

        [XmlAttribute(AttributeName = "MinNumberLen")]
        public int MinNumberLen { get; set; }

        [XmlAttribute(AttributeName = "LastIncFinish")]
        public int LastIncFinish { get; set; }
    }

    [XmlRoot(ElementName = "Path")]
    public class AutoSearchPath
    {
        public int FinishTime { get; set; }
    }
}
