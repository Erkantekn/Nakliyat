using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nakliyat.Models;
namespace Nakliyat.Controllers
{
    public class SitemapGenerator
    {
        nakliyatEntities db = new nakliyatEntities();
        /*
        
            Kullanımı:
      


            SitemapGenerator sitemap = new SitemapGenerator();
          
            foreach (var item in db.sitemapUrl.ToList())
            {
                sitemap.setFillParametres(item.Yol, item.Lastmod);
            }

            sitemap.generate(db.sirketBilgileri.FirstOrDefault().URL_Sitemap_icin, Server.MapPath("~/"));




        */
        struct parametre
        {
            public string loc, lastmod;
        };
        List<parametre> parametres =new List<parametre>();
        public void generate(string URL,string sitemapYolu)
        {
           
            string Yazilacak = "";
            Yazilacak += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">";
            foreach (var item in parametres)
            {
                Yazilacak += "\n<url>";
                Yazilacak += "\n<loc>" + URL + item.loc +"</loc>";
                Yazilacak += "\n<lastmod>" +item.lastmod + "</lastmod>";
                Yazilacak += "\n</url>";
            }
            Yazilacak += "\n</urlset>";

            dosyayaYaz(Yazilacak, sitemapYolu);
            setRemoveParametres();
        }
        public void setFillParametres(string loc,string lastmod)
        {
            parametre newParametre;
            newParametre.loc = loc;
            newParametre.lastmod = lastmod;
            parametres.Add(newParametre);
        }
         void setRemoveParametres()
        {
            parametres = new List<parametre>();
        }
        private  void dosyayaYaz(string yaz,string yol)
        {
            string dosya_yolu = yol+"sitemap.txt";
            if (File.Exists(dosya_yolu))
                File.Delete(dosya_yolu);
            
            FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(yaz);
            sw.Flush();
            sw.Close();
            fs.Close();
            if (File.Exists(yol + "sitemap.xml"))
                File.Delete(yol + "sitemap.xml");
            System.IO.File.Move(yol + "sitemap.txt", yol + "sitemap.xml");
        }
    }
}
 