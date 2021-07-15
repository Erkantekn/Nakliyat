using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nakliyat.Models;
namespace Nakliyat.ViewModels
{
    public class DepolamaViewModel
    {
        public sirketBilgileri sirketBilgileriV { get; set; }
        public List<depolama> depolamaV { get; set; }
    }
}