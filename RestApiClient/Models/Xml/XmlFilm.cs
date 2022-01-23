using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RestApiClient.Models
{
    [XmlRoot("Film")]
    public class XmlFilm
    {
        /// <summary>
        /// Id
        /// </summary> 
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("NameFilm")]
        public string NameFilm { get; set; }

        /// Возрастное ограничение
        /// </summary>  
        [XmlElement("FilmYears")]
        public FilmYears FilmYears { get; set; }

        ///// <summary>
        ///// Форматы
        ///// </summary> 
        [XmlArray("XmlFormats")]
        [XmlArrayItem(typeof(XmlFormat), ElementName = "XmlFormat")]
        public virtual List<XmlFormat> XmlFormats { get; set; }

        /// <summary>
        /// Обложка
        /// </summary>    
        [XmlElement("FilmCover")]
        public XmlFilmCover FilmCover { get; set; }

    }
}