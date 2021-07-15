using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nakliyat.Models;

namespace Nakliyat.ViewModels
{
    public class HakkimizdaViewModel
    {
        public sirketBilgileri sirketBilgileriV { get; set; }
        public pageHakkimizdaa pageHakkimizdaV { get; set; }
        public List<Resimler> ResimlerV { get; set; }
    }
}