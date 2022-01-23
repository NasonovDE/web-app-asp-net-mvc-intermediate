using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RestApiClient.Models
{
    [XmlRoot("FilmCover")]
    public class XmlFilmCover
    {
        [XmlElement("Data")]
        public string Data { get; set; }

        [XmlElement("ContentType")]
        public string ContentType { get; set; }
        [XmlElement("FileName")]
        public string FileName { get; set; }
    }
}