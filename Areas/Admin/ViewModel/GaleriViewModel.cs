using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nakliyat.Models;
namespace Nakliyat.Areas.Admin.ViewModel
{
    public class GaleriViewModel
    {
        public List<Resimler> ResimlerV { get; set; }
        public List<ResimKategorileri> ResimKategorileriV { get; set; }
    }
}