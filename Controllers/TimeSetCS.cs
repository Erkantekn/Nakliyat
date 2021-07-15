using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nakliyat.Models;
namespace Nakliyat.Controllers
{
    /*
        veritabanından gelen veriyi sunucu zamanına ekler.
        using ekledikten sonra yeni nesne üreterek kullanabilirsin.



     */
    public  class TimeSetCS
    {
        nakliyatEntities db = new nakliyatEntities();
        public  DateTime getTime()
        {
            int serverDakika = new nakliyatEntities().sirketBilgileri.FirstOrDefault().serverDakikaTamamlama;
            DateTime time = DateTime.Now;
            time = time.AddMinutes(serverDakika);
            return time;
        }
    }
}