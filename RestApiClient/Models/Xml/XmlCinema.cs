using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RestApiClient.Models
{
    [XmlRoot("Cinema")]
    public class XmlCinema
    {
        /// <summary>
        /// Id
        /// </summary> 
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("CinemaPlace")]
        public string CinemaPlace { get; set; }

        [XmlElement("NumberOfBilets")]
        public int NumberOfBilets { get; set; }

        /// QRcode
        /// </summary>  
        [XmlElement("QRcode")]
        public QRcode QRcode { get; set; }

    }
}