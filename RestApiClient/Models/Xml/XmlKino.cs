using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RestApiClient.Models
{
    [XmlRoot("Kino")]
    public class XmlKino
    {
        /// <summary>
        /// Id
        /// </summary> 
        [XmlElement("Id")]
        public int Id { get; set; }

        ///// <summary>
        ///// Фильмы
        ///// </summary> 
        [XmlArray("Films")]
        [XmlArrayItem(typeof(XmlFilm), ElementName = "Film")]
        public virtual List<XmlFilm> Films { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }


        ///// <summary>
        ///// Кинотеатр
        ///// </summary> 
        [XmlArray("Cinemas")]
        [XmlArrayItem(typeof(XmlCinema), ElementName = "Cinema")]
        public virtual List<XmlCinema> Cinemas { get; set; }

        /// <summary>
        /// Дата сеанса
        /// </summary>  
        [XmlElement("DataTime")]
       
        public DateTime? NextArrivalDate { get; set; }

        /// <summary>
        /// Время сеанса
        /// </summary>  
        [XmlElement("KinoTime")]
        public DateTime? KinoTime { get; set; }

        /// <summary>
        /// Обложка
        /// </summary>    
        [XmlElement("FilmCover")]
        public XmlFilmCover FilmCover { get; set; }


    }
}