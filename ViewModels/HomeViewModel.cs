using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nakliyat.Models;
namespace Nakliyat.ViewModels
{
    public class HomeViewModel
    {
        public sirketBilgileri sirketBilgileriV { get;set; }
        public  List<Resimler> resimlerV { get; set; }
    }
}