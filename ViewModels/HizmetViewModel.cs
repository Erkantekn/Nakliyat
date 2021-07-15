using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nakliyat.Models;

namespace Nakliyat.ViewModels
{
    public class HizmetViewModel
    {
        public sirketBilgileri sirketBilgileriV { get; set; }
        public List<pagesHizmetEkleme> pagesHizmetEklemeV { get; set; }
    }
}